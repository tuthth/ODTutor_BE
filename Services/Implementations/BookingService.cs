using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Emails;
using Models.Models.Requests;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net;
using Emgu.CV.XPhoto;
using Models.Models.Views;

namespace Services.Implementations
{
    public class BookingService : BaseService, IBookingService
    {
        private TutorDataService _tds;
        private readonly IFirebaseRealtimeDatabaseService _firebaseRealtimeDatabaseService;
        private readonly ITransactionService _transactionService;
        public BookingService(ODTutorContext context, IFirebaseRealtimeDatabaseService firebaseRealtimeDatabaseService, ITransactionService transactionService, IMapper mapper) : base(context, mapper)
        {
            _firebaseRealtimeDatabaseService = firebaseRealtimeDatabaseService;
            _transactionService = transactionService;
        }
        // Step 1: Create Booing (By Choose from Calendar Tutor)
        public async Task<BookingStep1Response> CreateBooking(BookingRequest bookingRequest)
        {
            try
            {
                BookingStep1Response response = new BookingStep1Response();
                var student = _context.Users.Include(x => x.StudentNavigation).FirstOrDefault(x => x.StudentNavigation.StudentId == bookingRequest.StudentId);
                var tutor = _context.Users.Include(x => x.TutorNavigation).FirstOrDefault(x => x.TutorNavigation.TutorId == bookingRequest.TutorId);
                if (student.Banned == true || tutor.Banned == true)
                {
                    throw new CrudException(HttpStatusCode.Forbidden, "User is banned", "");
                }
                if (student.Active == false || tutor.Active == false)
                {
                    throw new CrudException(HttpStatusCode.Forbidden, "User is not active", "");
                }
                if (student.EmailConfirmed == false)
                {
                    throw new CrudException(HttpStatusCode.Forbidden, "Student is not confirmed email", "");
                }
                if (tutor.EmailConfirmed == false)
                {
                    throw new CrudException(HttpStatusCode.Forbidden, "Tutor is not confirmed email", "");
                }
                var booking = _mapper.Map<Booking>(bookingRequest);
                booking.BookingId = Guid.NewGuid();
                booking.CreatedAt = DateTime.UtcNow.AddHours(7);
                booking.Status = (Int32)BookingEnum.WaitingPayment;
                booking.Duration = TimeSpan.FromHours(1);
                booking.TotalPrice = tutor.TutorNavigation.PricePerHour * 1;
                booking.GoogleMeetUrl = "";
                booking.Message = "";
                booking.Description = "Lịch học của học sinh " + student.Name + " với gia sư " + tutor.Name;
                response.BookingId = booking.BookingId;
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Step 2 : Payment Booking
        public async Task<IActionResult> PaymentForBooking (Guid bookingID)
        {
            try
            {
                Booking booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingID);
                if (booking == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Booking not found", "");
                }
                if (booking.Status != (Int32)BookingEnum.WaitingPayment)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Booking is not waiting for payment", "");
                }
                TimeSpan bookingTime = new TimeSpan(booking.StudyTime.Value.Hour, booking.StudyTime.Value.Minute, 0);
                Student student = _context.Students.FirstOrDefault(x => x.StudentId == booking.StudentId);
                if (student == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Student not found", "");
                }
                Tutor tutor = _context.Tutors.FirstOrDefault(x => x.TutorId == booking.TutorId);
                Wallet studentWallet = _context.Wallets.FirstOrDefault(x => x.UserId == student.UserId);
                Wallet tutorWallet = _context.Wallets.FirstOrDefault(x => x.UserId == tutor.UserId);
                if (studentWallet == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Student wallet not found", "");
                }
                if (tutorWallet == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor wallet not found", "");
                }
                if (studentWallet.AvalaibleAmount < booking.TotalPrice)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Student wallet is not enough", "");
                }
                // Find the tutor available slot
                TutorDateAvailable tutorDateAvailable = _context.TutorDateAvailables.FirstOrDefault(x => x.TutorID == tutor.TutorId && x.Date.Date == booking.StudyTime);
                if(tutorDateAvailable == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor date available not found", "");
                }
                // Find the tutor slot available match the booking time
                TutorSlotAvailable tutorSlotAvailable = _context.TutorSlotAvailables.FirstOrDefault(x => x.TutorDateAvailableID == tutorDateAvailable.TutorDateAvailableID && x.StartTime == bookingTime);
                if (tutorSlotAvailable == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor slot available not found", "");
                }
                if (tutorSlotAvailable.IsBooked == true)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Tutor slot available is booked", "");
                }
                tutorSlotAvailable.IsBooked = true;
                booking.Status = (Int32)TutorSlotAvailabilityEnum.NotAvailable;

                // Xử lý booking Transaction
                var bookingTransactionCreate = new BookingTransactionCreate
                {
                    Amount = (decimal)booking.TotalPrice,
                    BookingId = booking.BookingId,
                    RedirectUrl = "https://localhost:3000/payment-success",  //cai nay de choi thoi, khong phai nap rut redirect lam gi
                    SenderId = studentWallet.WalletId,
                    ReceiverId = tutorWallet.WalletId
                };
                await _transactionService.CreateDepositVnPayBooking(bookingTransactionCreate);
                // Xử lý notification 
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = Guid.NewGuid();
                notification.UserId = student.UserId;
                notification.Title = "Đặt lịch thành công";
                notification.Content = "Bạn đã đặt lịch học thành công";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                Notification noti = _mapper.Map<Notification>(notification);
                _context.Notifications.Add(noti);
                // Lưu notification vào firestore
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                // Lưu tất cả thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.Created, "Payment for booking successfully", "");
            } catch(CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /*public async Task<IActionResult> UpdateBooking(UpdateBookingRequest updateBookingRequest)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == updateBookingRequest.BookingId);
            if (booking == null)
            {
                return new StatusCodeResult(404);
            }
            var student = _context.Users.FirstOrDefault(x => x.Id == updateBookingRequest.StudentId);
            var tutor = _context.Users.FirstOrDefault(x => x.Id == updateBookingRequest.TutorId);
            if (student.Banned == true || tutor.Banned == true)
            {
                return new StatusCodeResult(406);
            }
            if (student.Active == false || tutor.Active == false)
            {
                return new StatusCodeResult(406);
            }
            if (booking.Status == (Int32)BookingEnum.Deleted || booking.Status == (Int32)BookingEnum.Finished || booking.Status == (Int32)BookingEnum.Unknown)
            {
                return new StatusCodeResult(409);
            }
            if (updateBookingRequest.Status != null && updateBookingRequest.Status != booking.Status)
            {
                booking.Status = updateBookingRequest.Status;
            }
            if (updateBookingRequest.Duration != null && updateBookingRequest.Duration != booking.Duration)
            {
                booking.Duration = updateBookingRequest.Duration;
            }
            if (updateBookingRequest.ActualEndTime != null && updateBookingRequest.ActualEndTime != booking.ActualEndTime)
            {
                booking.ActualEndTime = updateBookingRequest.ActualEndTime;
            }
            if (updateBookingRequest.Message != null && updateBookingRequest.Message != updateBookingRequest.Message)
            {
                booking.Message = updateBookingRequest.Message;
            }
            if (updateBookingRequest.TotalPrice != null && updateBookingRequest.TotalPrice != booking.TotalPrice)
            {
                booking.TotalPrice = updateBookingRequest.TotalPrice;
            }
            if (updateBookingRequest.Description != null && updateBookingRequest.Description != booking.Description)
            {
                booking.Description = updateBookingRequest.Description;
            }
            if (updateBookingRequest.GoogleMeetUrl != null && updateBookingRequest.GoogleMeetUrl != booking.GoogleMeetUrl)
            {
                booking.GoogleMeetUrl = updateBookingRequest.GoogleMeetUrl;
            }
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }*/

        /*        public async Task<IActionResult> UpdateBooking(UpdateBookingRequest updateBookingRequest)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == updateBookingRequest.BookingId);
            if (booking == null)
            {
                return new StatusCodeResult(404);
            }
            var student = _context.Users.FirstOrDefault(x => x.Id == updateBookingRequest.StudentId);
            var tutor = _context.Users.FirstOrDefault(x => x.Id == updateBookingRequest.TutorId);
            if (student.Banned == true || tutor.Banned == true)
            {
                return new StatusCodeResult(406);
            }
            if (student.Active == false || tutor.Active == false)
            {
                return new StatusCodeResult(406);
            }
            
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }*/
        public async Task<IActionResult> RateBookings(TutorRatingRequest tutorRatingRequest)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == tutorRatingRequest.BookingId);
            if (booking == null || booking.Status != (Int32)BookingEnum.Finished)
            {
                return new StatusCodeResult(404);
            }
            var student = _context.Users.FirstOrDefault(x => x.Id == tutorRatingRequest.StudentId);
            var tutor = _context.Users.FirstOrDefault(x => x.Id == tutorRatingRequest.TutorId);
            if (student.Banned == true)
            {
                return new StatusCodeResult(406);
            }
            if (student.Active == false)
            {
                return new StatusCodeResult(406);
            }
            var tutorRating = _mapper.Map<TutorRating>(tutorRatingRequest);
            tutorRating.TutorRatingId = Guid.NewGuid();
            _context.TutorRatings.Add(tutorRating);
            var tutorRatingImages = await _appExtension.UploadImagesToImgBB(tutorRatingRequest.ImageFiles);
            foreach (var imageUrl in tutorRatingImages)
            {
                var tutorRatingImage = new TutorRatingImage
                {
                    TutorRatingImageId = Guid.NewGuid(),
                    TutorRatingId = tutorRating.TutorRatingId,
                    ImageUrl = imageUrl
                };
                _context.TutorRatingImages.Add(tutorRatingImage);
            }
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateRating(UpdateTutorRatingRequest request)
        {
            var tutorRating = _context.TutorRatings.FirstOrDefault(x => x.TutorRatingId == request.TutorRatingId);
            var student = _context.Users.FirstOrDefault(x => x.Id == request.StudentId);
            var tutor = _context.Users.FirstOrDefault(x => x.Id == request.TutorId);
            if (student.Banned == true)
            {
                return new StatusCodeResult(406);
            }
            if (student.Active == false)
            {
                return new StatusCodeResult(406);
            }
            if (tutorRating == null)
            {
                return new StatusCodeResult(404);
            }
            if (request.RatePoints != null && request.RatePoints != tutorRating.RatePoints)
            {
                tutorRating.RatePoints = request.RatePoints;
            }
            if (request.Content != null && request.Content != tutorRating.Content)
            {
                tutorRating.Content = request.Content;
            }
            _context.TutorRatings.Update(tutorRating);
            var tutorRatingImages = await _appExtension.UploadImagesToImgBB(request.ImageFiles);
            foreach (var imageUrl in tutorRatingImages)
            {
                var tutorRatingImage = new TutorRatingImage
                {
                    TutorRatingImageId = Guid.NewGuid(),
                    TutorRatingId = tutorRating.TutorRatingId,
                    ImageUrl = imageUrl
                };
                _context.TutorRatingImages.Add(tutorRatingImage);
            }
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> RemoveRating(Guid id)
        {
            var tutorRating = _context.TutorRatings.FirstOrDefault(x => x.TutorRatingId == id);
            var student = _context.Users.FirstOrDefault(x => x.Id == tutorRating.StudentId);
            var tutor = _context.Users.FirstOrDefault(x => x.Id == tutorRating.TutorId);
            if (student.Banned == true)
            {
                return new StatusCodeResult(406);
            }
            if (student.Active == false)
            {
                return new StatusCodeResult(406);
            }
            if (tutorRating == null)
            {
                return new StatusCodeResult(404);
            }
            _context.TutorRatings.Remove(tutorRating);
            var tutorRatingImages = _context.TutorRatingImages.Where(x => x.TutorRatingId == id).ToList();
            _context.TutorRatingImages.RemoveRange(tutorRatingImages);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(204);
        }
        public async Task<IActionResult> StartLearning(Guid id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == id);
            if (booking == null)
            {
                return new StatusCodeResult(404);
            }
            if (booking.Status != (Int32)BookingEnum.Success)
            {
                return new StatusCodeResult(409);
            }
            booking.Status = (Int32)BookingEnum.Learning;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> FinishBooking(Guid id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == id);
            if (booking == null)
            {
                return new StatusCodeResult(404);
            }
            if(booking.Status != (Int32)BookingEnum.Learning)
            {
                return new StatusCodeResult(409);
            }
            booking.Status = (Int32)BookingEnum.Finished;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<ActionResult<List<Booking>>> GetAllBookings()
        {
            try
            {
                var bookings = await _context.Bookings.ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Booking>> GetBooking(Guid id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.BookingTransactionNavigation)
                    .FirstOrDefaultAsync(c => c.BookingId == id);
                if (booking == null)
                {
                    return new StatusCodeResult(404);
                }
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Booking>>> GetBookingsByStudentId(Guid id)
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.BookingTransactionNavigation)
                    .Where(c => c.StudentId == id).ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Booking>>> GetBookingsByTutorId(Guid id)
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.BookingTransactionNavigation)
                    .Where(c => c.TutorId == id).ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Add API Link To Booking 
        public async Task<IActionResult> AddGoogleMeetUrl(Guid bookingId, string meetingLink)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
                if (booking == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Booking not found", "");
                }
                string decodedLink = Uri.UnescapeDataString(meetingLink);
                booking.GoogleMeetUrl = decodedLink;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Add Google Meet Url successfully", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "", "");
            }
        }

        // Get Link Meeting 
        public async Task<ActionResult<string>> GetGoogleMeetUrl(Guid bookingId)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
                if (booking == null)
                {
                    return new StatusCodeResult(404);
                }
                return booking.GoogleMeetUrl;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Update Booking Link Meeting 
        public async Task<IActionResult> UpdateGoogleMeetUrl(Guid bookingId, string meetingLink)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
                if (booking == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Booking not found", "");
                }
                string decodeUrl = Uri.UnescapeDataString(meetingLink);
                booking.GoogleMeetUrl = decodeUrl;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Update Google Meet Url successfully", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "", "");
            }
        }

        // Dời lịch học
        public async Task<IActionResult> RescheduleBooking(Guid bookingId, Guid senderId, DateTime newTime, string message)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
                if (booking == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Booking not found", "");
                }
                if (booking.Status == (Int32)BookingEnum.Finished)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Booking is finished", "");
                }
                if(booking.Status == (Int32)BookingEnum.WaittingConfirmRescheduleForTutor || booking.Status == (Int32)BookingEnum.WaittingConfirmRescheduleForStudent)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Booking is waiting for confirm reschedule", "");
                } if(booking.Status == (Int32)BookingEnum.Cancelled)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Booking is cancelled", "");
                }
                if(booking.Status == (Int32)BookingEnum.WaitingPayment)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Booking is waiting for payment", "");
                }
                if (booking.IsRescheduled == true)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Booking is already rescheduled", "");
                }
                if (senderId == booking.StudentId)
                {
                    booking.Status = (Int32)BookingEnum.WaittingConfirmRescheduleForTutor;
                }
                else if (senderId == booking.TutorId)
                {
                    booking.Status = (Int32)BookingEnum.WaittingConfirmRescheduleForStudent;
                }
                booking.RescheduledTime = newTime;
                booking.IsRescheduled = true;
                booking.Message = message;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Reschedule booking successfully", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "", "");
            }
        }

        // Xác nhận đổi lịch học 
        public async Task<IActionResult> ConfirmRescheduleBooking(Guid bookingId)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
                if (booking == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Booking not found", "");
                }
                booking.Status = (Int32)BookingEnum.Success;
                // Set Old Slot available for tutor when change booking time
                TimeSpan bookingTime = new TimeSpan(booking.StudyTime.Value.Hour, booking.StudyTime.Value.Minute, 0);
                Tutor tutor1 = _context.Tutors.FirstOrDefault(x => x.TutorId == booking.TutorId);
                TutorDateAvailable tutorDateAvailable1 = _context.TutorDateAvailables.FirstOrDefault(x => x.TutorID == tutor1.TutorId && x.Date.Date == booking.StudyTime.Value.Date);
                if (tutorDateAvailable1 == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor date available not found", "");
                }
                TutorSlotAvailable tutorSlotAvailable1 = _context.TutorSlotAvailables.FirstOrDefault(x => x.TutorDateAvailableID == tutorDateAvailable1.TutorDateAvailableID && x.StartTime == bookingTime);
                if (tutorSlotAvailable1 == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor slot available not found", "");
                }
                tutorSlotAvailable1.IsBooked = false;
                tutorSlotAvailable1.Status = (Int32)TutorSlotAvailabilityEnum.Available;
                _context.TutorSlotAvailables.Update(tutorSlotAvailable1);
                booking.StudyTime = booking.RescheduledTime;
                _context.Bookings.Update(booking);
                // Set Slot available for tutor when change booking time
                Tutor tutor = _context.Tutors.FirstOrDefault(x => x.TutorId == booking.TutorId);
                TutorDateAvailable tutorDateAvailable = _context.TutorDateAvailables.FirstOrDefault(x => x.TutorID == tutor.TutorId && x.Date.Date == booking.StudyTime.Value.Date);
                if (tutorDateAvailable == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor date available not found", "");
                }
                TutorSlotAvailable tutorSlotAvailable = _context.TutorSlotAvailables.FirstOrDefault(x => x.TutorDateAvailableID == tutorDateAvailable.TutorDateAvailableID && x.StartTime == booking.StudyTime.Value.TimeOfDay);
                if (tutorSlotAvailable == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor slot available not found", "");
                }
                tutorSlotAvailable.IsBooked = true;
                tutorSlotAvailable.Status = (Int32)TutorSlotAvailabilityEnum.NotAvailable;

                _context.TutorSlotAvailables.Update(tutorSlotAvailable);
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Confirm reschedule booking successfully", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "", "");
            }
        }
    }
}
