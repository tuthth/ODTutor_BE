using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorExperienceRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string? imageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndYear { get; set; }
    }
    public class UpdateTutorExperienceRequest : TutorExperienceRequest
    {
        public Guid TutorExperienceId { get; set; }
    }
}
