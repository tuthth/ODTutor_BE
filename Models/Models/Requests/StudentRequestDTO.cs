using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class StudentRequestDTO
    {
        public Guid StudentRequestId { get; set; }
        public Guid SubjectId { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
