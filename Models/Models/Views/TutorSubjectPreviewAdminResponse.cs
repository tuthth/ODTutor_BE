using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorSubjectPreviewAdminResponse
    {
        public Guid TutorId { get; set; }
        public Guid TutorSubjectId { get; set; }
        public string TutorName { get; set; }
        public string SubjectName { get; set; }
        public string TutorAvatar {get; set;}
    }
}
