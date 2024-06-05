using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorScheduleResponse
    {
        public Guid TutorID { get; set; }
        public DateTime StartTime { get;set; }
        public DateTime EndTime { get; set; }
        public List<TutorSlotResponse> TutorSlots { get; set; }
    }
}
