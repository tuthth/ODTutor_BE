using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorRegistDate
    {
        public DateTime datee { get; set; }
        public int DayOfWeek { get; set; } // 2: Monday ... 0: Sunday
        public List<TutorStartTimeEndTimRegisterRequest> timeinDate { get; set; }
    }
}
