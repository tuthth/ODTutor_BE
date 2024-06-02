using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorWeekAvailable
    {
        public Guid TutorWeekAvailableId { get; set; }
        public Guid TutorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation properties
        public virtual Tutor Tutor { get; set; }
        public ICollection<TutorDateAvailable> TutorDateAvailables { get; set; }
    }
}
