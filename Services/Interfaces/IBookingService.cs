using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingStep1Response> CreateBooking(BookingRequest bookingRequest);
        /*Task<IActionResult> UpdateBooking(UpdateBookingRequest updateBookingRequest);*/
        Task<ActionResult<List<Booking>>> GetAllBookings();
        Task<ActionResult<Booking>> GetBooking(Guid id);
        Task<ActionResult<List<Booking>>> GetBookingsByStudentId(Guid id);
        Task<ActionResult<List<Booking>>> GetBookingsByTutorId(Guid id);
        Task<IActionResult> RateBookings(TutorRatingRequest tutorRatingRequest);
        Task<IActionResult> UpdateRating(UpdateTutorRatingRequest request);
        Task<IActionResult> RemoveRating(Guid id);
    }
}
