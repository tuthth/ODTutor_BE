using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid StudentId { get; set; }
        public Guid TutorId { get; set; }
        public Guid TutorSubjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string ? BookingContent { get; set; }
        public string? Message { get; set; }
        public decimal ?TotalPrice { get; set; }
        public int Status { get; set; }
        public DateTime ?StudyTime { get;set; }
        public DateTime? RescheduledTime { get; set; }
        public bool? IsRescheduled { get;set; }
        public string? Description { get; set; }
        public string? GoogleMeetUrl {get; set;}
        public virtual Student? StudentNavigation {  get; set; }
        public virtual Tutor? TutorNavigation { get; set; }
        public virtual ICollection<TutorRating>? TutorRatingsNavigation { get; set; }
        public virtual BookingTransaction? BookingTransactionNavigation { get; set; }
        public virtual TutorSubject? TutorSubjectNavigation { get; set; }
    }
}
