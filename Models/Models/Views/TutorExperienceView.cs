using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorExperienceView
    {
        public Guid TutorExperienceId { get; set; }
        public Guid TutorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndYear { get; set; }
        public bool IsVerified { get; set; }
    }
}
