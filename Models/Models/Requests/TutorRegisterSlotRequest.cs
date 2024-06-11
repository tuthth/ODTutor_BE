using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorRegisterSlotRequest
    {
        public int DayOfWeek { get; set; }
        public List<TutorStartTimeEndTimRegisterRequest> TutorStartTimeEndTimRegisterRequests { get; set; }
    }
}
