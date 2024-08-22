using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class BookingView
    {
        public Guid BookingId { get; set; }
        public Guid StudentId { get; set; }
        public Guid TutorId { get; set; }
        public Guid TutorSubjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime ActualEndTime { get; set; }
        public DateTime StudyTime { get; set; }
        public string? Message { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public string? Description { get; set; }
        public string? GoogleMeetUrl { get; set; }
        public Guid? BookingTransactionId { get; set; }
        public DateTime? RescheduledTime { get; set; }
        public bool IsRescheduled { get; set; }
        public bool IsRated { get; set; }
        public string? SubjectName { get; set; }
    }
}
