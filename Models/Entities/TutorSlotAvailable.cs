using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorSlotAvailable
    {
        public Guid TutorSlotAvailableID {get; set;}
        public Guid TutorDateAvailableID { get; set; }
        public Guid TutorID { get; set; }
        public TimeSpan StartTime { get; set;}
        public bool IsBooked { get; set; }
        public int? Status { get; set; }

        // Navigation properties
        public virtual TutorDateAvailable TutorDateAvailable { get; set; }
        public virtual Tutor Tutor { get; set; }
    }
}
