using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorSubjectView
    {
        public Guid TutorSubjectId { get; set; }
        public Guid TutorId { get; set; }
        public Guid SubjectId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
