using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRegisterStep5Reponse
    {   
        public DateTime? date { get; set; }
        public int? dayOfWeek { get; set; }
        public TimeSpan startTime { get; set; }
    }
}
