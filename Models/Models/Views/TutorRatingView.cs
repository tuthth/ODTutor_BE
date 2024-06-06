using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRatingView
    {
        public Guid TutorRatingId { get; set; }
        public Guid TutorId { get; set; }
        public Guid StudentId { get; set; }
        public int RatePoints { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid BookingId { get; set; }
    }
}
