using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorProcess
    {
        public Guid UserProcessId { get; set; }
        public Guid TutorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ReponseAt { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
