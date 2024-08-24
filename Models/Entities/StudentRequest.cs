using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class StudentRequest
    {
        public Guid StudentRequestId { get; set; }
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        public string ? SubjectName { get; set; }
        public virtual Student StudentNavigation { get; set; }
        public virtual Subject SubjectNavigation { get; set; }
    }
}
