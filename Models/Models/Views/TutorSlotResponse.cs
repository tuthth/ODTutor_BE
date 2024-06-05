using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorSlotResponse
    {
        public Guid TutorSlotID { get; set;}
        public DateTime Date { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool IsBooked { get; set; }
        public int? Status { get; set; }
    }
}
