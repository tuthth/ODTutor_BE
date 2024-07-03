using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class CreateStudentRequest
    {
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
        public string? Message { get; set; }
    }
    public class UpdateStudentRequest
    {
        public Guid StudentRequestId { get; set; }
        public Guid SubjectId { get; set; }
        public string? Message { get; set; }
    }
}
