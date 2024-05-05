using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorRatingImage
    {
        public Guid TutorRatingImageId {  get; set; }
        public Guid TutorId {  get; set; }
        public Guid TutorRatingId { get; set; }
        public string ImageUrl { get; set; }

        public virtual Tutor? TutorNavigation {  get; set; }
        public virtual TutorRating? TutorRatingNavigation {  get; set; }
    }
}
