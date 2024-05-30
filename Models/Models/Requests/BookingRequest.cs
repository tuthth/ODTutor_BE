using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class BookingRequest
    {
        public Guid StudentId { get; set; }
        public Guid TutorId { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime ActualEndTime { get; set; }
        public string? Message { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public string? Description { get; set; }
        [RegularExpression(@"^https:\/\/meet\.google\.com\/[a-zA-Z0-9\-]{12}$", ErrorMessage = "Google Meet URL không hợp lệ")]
        public string? GoogleMeetUrl { get; set; }
    }
    public class UpdateBookingRequest : BookingRequest
    {
        public Guid BookingId { get; set; }
    }
}
