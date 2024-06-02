using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorRegistDate
    {
        public DateTime Date { get; set; }
        public int DayOfWeek { get; set; } // 2: Monday ... 0: Sunday
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
