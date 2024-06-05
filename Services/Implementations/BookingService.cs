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

namespace Services.Implementations
{
    public class BookingService : BaseService, IBookingService
    {
        public BookingService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> CreateBooking(BookingRequest bookingRequest)
        {
            var student = _context.Users.FirstOrDefault(x => x.Id == bookingRequest.StudentId);
            var tutor = _context.Users.FirstOrDefault(x => x.Id == bookingRequest.TutorId);
            if(student.Banned == true || tutor.Banned == true)
            {
                return new StatusCodeResult(406);
            }
            if(student.Active == false || tutor.Active == false)
            {
                return new StatusCodeResult(406);
            }
            if(bookingRequest.Status != (Int32)BookingEnum.Pending)
            {
                return new StatusCodeResult(409);
            }
            var booking = _mapper.Map<Booking>(bookingRequest);
            booking.BookingId = Guid.NewGuid();
            booking.CreatedAt = DateTime.UtcNow;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = student.Email,
                Subject = "[ODTutor] Tạo buổi học thành công",
                Body = "Chúc mừng bạn đã tạo buổi học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học."
            });
            await _appExtension.SendMail(new MailContent()
            {
                To = tutor.Email,
                Subject = "[ODTutor] Tạo buổi học thành công",
                Body = "Chúc mừng bạn đã tạo buổi học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học."
            });
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateBooking(UpdateBookingRequest updateBookingRequest)
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
            if(booking.Status == (Int32)BookingEnum.Deleted || booking.Status == (Int32)BookingEnum.Finished || booking.Status == (Int32)BookingEnum.Unknown)
            {
                return new StatusCodeResult(409);
            }
            if(updateBookingRequest.Status!=null && updateBookingRequest.Status!=booking.Status)
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
                var booking = await _context.Bookings.FirstOrDefaultAsync(c => c.BookingId == id);
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
                var bookings = await _context.Bookings.Where(c => c.StudentId == id).ToListAsync();
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
                var bookings = await _context.Bookings.Where(c => c.TutorId == id).ToListAsync();
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
    }
}
