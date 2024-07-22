using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class ScheduleRequest
    {
        public Guid StudentCourseId { get; set; }
        public DateTime StartAt { get; set; }
    }
    public class RescheduleRequest
    {
        public Guid ScheduleId { get; set; }
        public DateTime StartAt { get; set; }
    }
}
