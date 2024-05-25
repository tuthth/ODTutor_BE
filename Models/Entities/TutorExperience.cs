using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorExperience
    {
        public Guid TutorExperienceId { get; set; }
        public Guid TutorId { get; set; }
        public Guid SubjectId { get; set; }
        public string Description {get; set;}
    }
}
