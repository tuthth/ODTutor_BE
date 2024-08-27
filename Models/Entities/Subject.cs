using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Subject
    {
        public Guid SubjectId { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Note {  get; set; }
        public bool? Status { get;set; } = true;

        public virtual ICollection<TutorSubject> TutorSubjectNavigation { get; set; }
        public virtual ICollection<StudentRequest>? StudentRequestsNavigation { get; set; }
    }
}
