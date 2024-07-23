using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRatingListResponse
    {   
        public Guid TutorRatingId { get; set; }
        public string Image { get; set; }
        public string FullName { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set;}
    }
}
