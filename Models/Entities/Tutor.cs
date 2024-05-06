using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Tutor
    {
        public Guid TutorId { get; set; }
        public Guid UserId { get; set; }
        public string IdentityNumber {  get; set; }
        public string Level { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }

        public virtual User? UserNavigation {  get; set; }
        public virtual ICollection<Course>? CoursesNavigation { get; set; }
        public virtual ICollection<TutorCertificate>? TutorCertificatesNavigation { get; set; }
        public virtual ICollection<TutorSubject>? TutorSubjectsNavigation { get; set; }
        public virtual ICollection<TutorRating>? TutorRatingsNavigation { get; set; }
        public virtual ICollection<TutorRatingImage>? TutorRatingsImagesNavigation { get; set; }
        public virtual ICollection<Booking>? BookingsNavigation { get; set; }
    }
}
