using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorRating
    {
        public Guid TutorRatingId { get; set; }
        public Guid TutorId { get; set; }
        public Guid StudentId { get; set; }
        public int RatePoints { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid BookingId { get; set; }

        public virtual Tutor? TutorNavigation {  get; set; }
        public virtual Student? StudentNavigation { get; set; }
        public virtual Booking? BookingNavigation { get; set; }
        public virtual ICollection<TutorRatingImage> TutorRatingImagesNavigation {  get; set; }
    }
}
