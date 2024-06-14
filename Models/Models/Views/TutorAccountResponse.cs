using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorAccountResponse
    {
        public Guid TutorId { get; set; }
        public Guid UserId { get; set; }
        //public string Level { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string? VideoUrl { get; set; }
        public List<Course> Courses { get; set; }
        public List<TutorCertificate> TutorCertificates { get; set; }
        public List<TutorSubject> Subjects { get; set; }
        public List<TutorRating> Ratings { get; set; }
    }
}
