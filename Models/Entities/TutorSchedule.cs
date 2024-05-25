using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorSchedule
    {
        public Guid TutorScheduleId { get; set; }
        public Guid TutorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ActualEndTime { get; set; }
        public bool IsBooked { get; set; }
        public virtual Tutor TutorNavigation { get; set; }
    }
}
