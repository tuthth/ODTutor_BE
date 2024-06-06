using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class StudentRequestView
    {
        public Guid StudentRequestId { get; set; }
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
    }
}
