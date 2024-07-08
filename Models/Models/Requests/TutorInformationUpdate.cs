using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorInformationUpdate
    {   
        public Guid TutorId { get; set; }
        public decimal? PricePerHour { get; set; }
        public string?IdentityNumber { get; set; }
        public string? Description { get; set; }
        public string? EducationExperience { get; set; }
        public string? Motivation { get; set; }
        public string? AttractiveTitle { get; set; }
    }
}
