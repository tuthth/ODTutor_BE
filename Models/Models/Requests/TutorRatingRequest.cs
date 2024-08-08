using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorRatingRequest
    {
        public Guid TutorId { get; set; }
        public Guid StudentId { get; set; }
        public int RatePoints { get; set; }
        public string Content { get; set; }
        public Guid BookingId { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
    public class UpdateTutorRatingRequest : TutorRatingRequest
    {
        public Guid TutorRatingId { get; set; }
    }
}
