using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class BookingReviewResponse
    {
        public string tutorName { get; set; }
        public string imgeUrl { get; set; }
        public DateTime StudyTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalRating { get; set; }
        public string ratingStart {get; set;}
        public List<string> tutorSubjectList { get; set; }
        public List<TutorRatingView> tutorRatingtList { get; set; }

    }
}
