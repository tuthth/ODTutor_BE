using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRatingImageView
    {
        public Guid TutorRatingImageId { get; set; }
        public Guid TutorRatingId { get; set; }
        public string ImageUrl { get; set; }
    }
}
