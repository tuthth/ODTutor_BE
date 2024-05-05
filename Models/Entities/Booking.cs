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
        public DateTime CreatedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime ActualEndTime { get; set; }
        public string Message { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string GoogleMeetUrl { get; set; }

        public virtual Student? StudentNavigation {  get; set; }
        public virtual Tutor? TutorNavigation { get; set; }
        public virtual ICollection<TutorRating>? TutorRatingsNavigation { get; set; }
        public virtual BookingTransaction? BookingTransactionNavigation { get; set; }

    }
}
