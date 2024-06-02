using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorDateAvailable
    {
        public Guid TutorDateAvailableID { get; set; }
        public Guid TutorID { get; set; }
        public Guid TutorWeekAvailableID { get; set; }
        public DateTime Date { get; set; }
        public int DayOfWeek { get; set; } // 2: Monday ... 0: Sunday
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        //Navigation properties
        public virtual Tutor Tutor { get; set; }
        public virtual TutorWeekAvailable TutorWeekAvailable { get; set; }
        public ICollection<TutorSlotAvailable> TutorSlotAvailables { get; set; }
    }
}
