using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorSubjectResponse
    {
        public Guid TutorSubjectId { get; set; }
        public Guid TutorId { get; set; }
        public string Title { get; set; }
    }
}
