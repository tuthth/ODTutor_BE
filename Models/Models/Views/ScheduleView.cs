using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class ScheduleView
    {
        public Guid ScheduleId { get; set; }
        public Guid StudentCourseId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int Status { get; set; }
    }
}
