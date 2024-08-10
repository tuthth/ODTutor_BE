using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorSubjectListResponse
    {   
        public Guid TutorSubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpeireAt { get; set; }
        public int Status { get; set; }
    }
}
