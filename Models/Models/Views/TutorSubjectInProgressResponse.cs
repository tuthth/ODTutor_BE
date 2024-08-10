using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorSubjectInProgressResponse
    {
        public Guid TutorId { get; set; }
        public string TutorName {get;set;}
        public string TutorAvatar { get; set; }
        public int Status { get; set; }
        public string TutorEmail { get; set; }
    }
}
