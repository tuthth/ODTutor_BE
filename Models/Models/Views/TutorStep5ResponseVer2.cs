using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorStep5ResponseVer2
    {
            public int DayOfWeek { get; set; }
            public DateTime? Date { get; set; }
            public Dictionary<string, string> TimeDuration { get; set; }
    }
}
