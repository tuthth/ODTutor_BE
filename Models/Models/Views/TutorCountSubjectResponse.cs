using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorCountSubjectResponse
    {
        public int TotalSubject { get; set; }
        public int TutorSubjectActive { get;set; }
        public int TutorSubjectInactive { get; set; }
        public int TutorSubjectPending { get; set; }
    }
}
