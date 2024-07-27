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
                var tutorSlot = _context.TutorSlotAvailables.Include(x => x.TutorDateAvailable).FirstOrDefault(x => x.TutorSlotAvailableID == bookingRequest.TutorSlotAvalaibleID);
                if (student == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Student not found", "");
                }
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor not found", "");
                }
                if (tutorSlot == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor slot available not found", "");
                }
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
                if(tutorSlot.IsBooked == true)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Tutor slot available is booked", "");
                }
                if (tutorSlot.Status == (Int32)TutorSlotAvailabilityEnum.NotAvailable)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Tutor slot available is not available", "");
                }
                DateTime studyTime = new DateTime(tutorSlot.TutorDateAvailable.Date.Year, tutorSlot.TutorDateAvailable.Date.Month, tutorSlot.TutorDateAvailable.Date.Day, tutorSlot.StartTime.Hours, tutorSlot.StartTime.Minutes, tutorSlot.StartTime.Seconds);
                var booking = _mapper.Map<Booking>(bookingRequest);
                booking.StudyTime = studyTime;
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
        public async Task<IActionResult> RateBookingsWithoutImage(TutorRatingRequest tutorRatingRequest)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == tutorRatingRequest.BookingId);
            if (booking == null || booking.Status != (Int32)BookingEnum.Finished)
            {
                return new StatusCodeResult(404);
            }
            var student = _context.Users
                .Include(x => x.StudentNavigation)
                .FirstOrDefault(x => x.StudentNavigation.StudentId == tutorRatingRequest.StudentId);
            var tutor = _context.Users
                .Include(x => x.TutorNavigation)
                .FirstOrDefault(x => x.TutorNavigation.TutorId == tutorRatingRequest.TutorId);
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
        public async Task<IActionResult> RescheduleBooking(Guid bookingId, Guid senderId, Guid newSlotId, string message)
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
                // Get Time Booking of Slot and Date of TutorDateAvailable based on tutodateid of slot and create a new DAteTime 
                // Lấy thông tin slot mới
                TutorSlotAvailable tutorSlotAvailable = _context.TutorSlotAvailables.FirstOrDefault(x => x.TutorSlotAvailableID == newSlotId);
                if (tutorSlotAvailable == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor slot not found", "");
                }

                // Lấy thông tin ngày từ TutorDateAvailable
                TutorDateAvailable tutorDateAvailable = _context.TutorDateAvailables.FirstOrDefault(x => x.TutorDateAvailableID == tutorSlotAvailable.TutorDateAvailableID);
                if (tutorDateAvailable == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor date not found", "");
                }

                // Kết hợp ngày và giờ để tạo ra DateTime mới
                DateTime newTime = new DateTime(
                    tutorDateAvailable.Date.Year,
                    tutorDateAvailable.Date.Month,
                    tutorDateAvailable.Date.Day,
                    tutorSlotAvailable.StartTime.Hours,
                    tutorSlotAvailable.StartTime.Minutes,
                    tutorSlotAvailable.StartTime.Seconds
                );
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
                // Find user by StudnetId and TutorId
                var student = _context.Users
                    .Include(x => x.StudentNavigation)
                    .FirstOrDefault(x => x.StudentNavigation.StudentId == booking.StudentId);
                var tutor = _context.Users
                    .Include(x => x.TutorNavigation)
                    .FirstOrDefault(x => x.TutorNavigation.TutorId == booking.TutorId);
                if(student == null || tutor == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Student or Tutor not found", "");
                }
                // Add Notification For student and tutor 
                if (senderId == booking.StudentId    )
                {                     
                    NotificationDTO notification = new NotificationDTO();
                    notification.NotificationId = Guid.NewGuid();
                    notification.UserId = tutor.Id;
                    notification.Title = "Yêu cầu dời lịch học";
                    notification.Content = "Học sinh " + student.Name + " đã yêu cầu dời lịch học";
                    notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                    Notification noti = _mapper.Map<Notification>(notification);
                    _context.Notifications.Add(noti);
                    // Lưu notification vào firestore
                    _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);

                    // Send noti by email to student
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = student.Email,
                        Subject = "Yêu cầu xác nhận đổi lịch học",
                        Body = ". Vui lòng đợi sự phàn hồi từ tutor và kiểm tra email hoặc thông báo khi nhận được sự phản hồi. Chúc bạn có một ngày tốt lành"
                    });

                    // Send noti by email to tutor
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = tutor.Email,
                        Subject = "Yêu cầu xác nhận đổi lịch học",
                        Body = ". Học sinh " + student.Name + " đã yêu cầu dời lịch học. Vui lòng kiểm tra thông báo và email để xác nhận. Chúc bạn có một ngày tốt lành"
                    });
                }
                else if (senderId == booking.TutorId)
                {
                    NotificationDTO notification = new NotificationDTO();
                    notification.NotificationId = Guid.NewGuid();
                    notification.UserId = student.Id;
                    notification.Title = "Yêu cầu dời lịch học";
                    notification.Content = "Gia sư " + tutor.Name + " đã yêu cầu dời lịch học";
                    notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                    Notification noti = _mapper.Map<Notification>(notification);
                    _context.Notifications.Add(noti);
                    // Lưu notification vào firestore
                    _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);

                    // Send noti by email to tutor
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = tutor.Email,
                        Subject = "Yêu cầu xác nhận đổi lịch học",
                        Body = ". Vui lòng đợi sự phàn hồi từ học sinh và kiểm tra email hoặc thông báo khi nhận được sự phản hồi. Chúc bạn có một ngày tốt lành"
                    });

                    // Send noti by email to student
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = student.Email,
                        Subject = "Yêu cầu xác nhận đổi lịch học",
                        Body = ". Gia sư " + tutor.Name + " đã yêu cầu dời lịch học. Vui lòng kiểm tra thông báo và email để xác nhận. Chúc bạn có một ngày tốt lành"
                    });
                }
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

        // Đồng ý dời lịch học
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

                // Update Old Slot
                UpdateTutorSlotAvailability( bookingId,booking.StudyTime.Value, false);

                // Update Booking StudyTime to RescheduledTime
                booking.StudyTime = booking.RescheduledTime;

                // Update New Slot
                UpdateTutorSlotAvailability( bookingId,booking.RescheduledTime.Value, true);

                _context.Bookings.Update(booking);

                // Create a new notification for student and tutor
                var student = _context.Users
                    .Include(x => x.StudentNavigation)
                    .FirstOrDefault(x => x.StudentNavigation.StudentId == booking.StudentId);
                var tutor = _context.Users
                    .Include(x => x.TutorNavigation)
                    .FirstOrDefault(x => x.TutorNavigation.TutorId == booking.TutorId);
                if (student == null || tutor == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Student or Tutor not found", "");
                }
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = Guid.NewGuid();
                notification.UserId = student.Id;
                notification.Title = "Xác nhận dời lịch học";
                notification.Content = "Thời gian học đã xác nhận thay đổi vui lòng kiểm tra lại trong học phần của bạn";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                Notification noti = _mapper.Map<Notification>(notification);
                _context.Notifications.Add(noti);
                // Lưu notification vào firestore
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);

                NotificationDTO notification2 = new NotificationDTO();
                notification2.NotificationId = Guid.NewGuid();
                notification2.UserId = tutor.Id;
                notification2.Title = "Xác nhận dời lịch học";
                notification2.Content = "Thời gian học đã xác nhận thay đổi vui lòng kiểm tra lại trong lịch dạy của bạn";
                notification2.CreatedAt = DateTime.UtcNow.AddHours(7);
                Notification noti2 = _mapper.Map<Notification>(notification2);
                _context.Notifications.Add(noti2);
                // Lưu notification vào firestore
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);

                // Send confirm email to student and tutor 
                await _appExtension.SendMail(new MailContent()
                {
                    To = student.Email,
                    Subject = "Xác nhận dời lịch học",
                    Body = ". Thời gian học đã xác nhận thay đổi vui lòng kiểm tra lại trong học phần của bạn"
                });
                await _appExtension.SendMail(new MailContent()
                {
                    To = tutor.Email,
                    Subject = "Xác nhận dời lịch học",
                    Body = ". Thời gian học đã xác nhận thay đổi vui lòng kiểm tra lại trong lịch dạy của bạn"
                });
                await _context.SaveChangesAsync();
                return new OkObjectResult("Confirm reschedule booking successfully");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Từ chối dời lịch học 
        public async Task<IActionResult> RejectRescheduleBooking(Guid bookingId)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
                if (booking == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Booking not found", "");
                }
                booking.Status = (Int32)BookingEnum.Success;
                booking.IsRescheduled = false;
                booking.RescheduledTime = null;
                booking.Message = "";
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return new OkObjectResult("Reject reschedule booking successfully");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }   

        private void UpdateTutorSlotAvailability(Guid bookingId,DateTime dateTime, bool isBooked)
        {   
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
            DateTime bookingDate = booking.StudyTime.Value.Date;
            var tutorDateAvailables = _context.TutorDateAvailables
                .Where(x => x.TutorID == booking.TutorId && x.Date.Date == bookingDate)
                .Select(x => x.TutorDateAvailableID)
                .ToList();
            if (tutorDateAvailables == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tutor date available not found", "");
            }

            // Changre status slot 
            TimeSpan bookingTime = new TimeSpan(booking.StudyTime.Value.Hour, booking.StudyTime.Value.Minute, 0);
            var tutorSlotAvailables = _context.TutorSlotAvailables
                .Where(x => tutorDateAvailables.Contains(x.TutorDateAvailable.TutorDateAvailableID) && x.StartTime == bookingTime)
                .FirstOrDefault();
            if (tutorSlotAvailables == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tutor slot available not found", "");
            }
            tutorSlotAvailables.IsBooked = !tutorSlotAvailables.IsBooked;
            if (tutorSlotAvailables.Status == (Int32)TutorSlotAvailabilityEnum.Available)
            {
                tutorSlotAvailables.Status = (Int32)TutorSlotAvailabilityEnum.NotAvailable;
            }
            else
            {
                tutorSlotAvailables.Status = (Int32)TutorSlotAvailabilityEnum.Available;
            }
            _context.TutorSlotAvailables.Update(tutorSlotAvailables);
        }
    }
}
