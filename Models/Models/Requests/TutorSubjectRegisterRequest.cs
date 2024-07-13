using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorSubjectRegisterRequest
    {
        public Guid TutorId { get; set; }
        public List<Guid> SubjectList { get; set; }
    }
}
