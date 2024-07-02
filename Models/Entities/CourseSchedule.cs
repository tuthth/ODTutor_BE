using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class CourseSchedule
    {
        public Guid CourseSlotId { get; set; }
        public Guid ScheduleId { get; set; }

        public virtual CourseSlot? CourseSlotNavigation { get; set; }
        public virtual Schedule? ScheduleNavigation { get; set; }
    }
}
