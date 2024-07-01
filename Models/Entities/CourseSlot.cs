using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class CourseSlot
    {
        public Guid CourseSlotId { get; set; }
        public Guid CourseId { get; set; }
        public int SlotNumber { get; set; }
        public string? Description { get; set; }

        public virtual Course? CourseNavigation { get; set; }
        public virtual ICollection<CourseSchedule>? CourseSchedulesNavigation { get; set; }
    }
}
