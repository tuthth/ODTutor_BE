using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRatingResponse
    {
        public Guid TutorId { get; set; }
        public double TotalEndRating { get; set; }
        public int TotalRatingNumber { get; set; }
        public int TotalRatingNumberOneStart {get; set; } 
        public int TotalRatingNumberTwoStart {get; set; }
        public int TotalRatingNumberThreeStart { get; set; }
        public int TotalRatingNumberFourStart { get; set; }
        public int TotalRatingNumberFiveStart { get; set; }
    }
}
