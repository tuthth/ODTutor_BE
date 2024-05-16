using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.Google.Calendar
{
    public class GGCalendarEventSetting
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string TimeZone { get; set; }
    }
}
