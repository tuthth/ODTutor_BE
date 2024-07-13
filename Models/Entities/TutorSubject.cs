using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.Entities
{
    public class TutorSubject
    {
        public Guid TutorSubjectId { get; set; }
        public Guid TutorId { get; set; }
        public Guid SubjectId { get; set; } 
        public DateTime CreatedAt {  get; set; }
        public virtual Tutor? TutorNavigation {  get; set; }
        public virtual Subject? SubjectNavigation {  get; set; }
    }
}
