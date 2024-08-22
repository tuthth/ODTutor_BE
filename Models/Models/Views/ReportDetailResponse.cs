using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class ReportDetailResponse
    {
        public Guid ReportId { get; set; }
        public Guid BookingId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderAvatar { get; set; }
        public string? TutorName { get; set; }
        public string? TutorAvatar { get; set; }
        public DateTime ? CreatedAt { get; set; }
        public string? Content { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public List<string>? Images { get; set; }
        // Booking Detail
        public DateTime? BookingCreateAt { get; set; }
        public string? BookingContent { get; set; }
        public string? BookingMessage { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? StudyTime { get; set; }
        public DateTime? RescheduledTime { get; set; }
        public bool? IsRescheduled { get; set; }
        public string? Description { get; set; }
        public string? GoogleMeetUrl { get; set; }
        public bool? IsRated { get; set; }
        public int? BookingStatus { get; set; }
        // Report Detail
        public int? NumberOfTutorReport { get; set; }
    }
}
