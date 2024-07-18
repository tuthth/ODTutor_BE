using Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorRegistScheduleRequest
    {
        public Guid TutorID { get; set; }
        public DateTime StartTime { get; set; } // spend for Week
        public DateTime EndTime { get; set; } // spend for Week
        public List<TutorRegistDate> dateList { get; set;}

    }
}
