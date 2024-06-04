using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorFeedBackResponse
    {
        public Guid TutorRatingId { get; set; }
        public Guid TutorID { get; set; }
        public Guid StudentID { get; set; }
        public Guid BookingID { get;set; }
        public string StudentAvatar { get; set; }
        public string StudentName {get; set;}
        public DateTime CreateAt { get;set; }
        public int RatePoints { get; set; }
        public string Content { get; set; } 
    }
}
    