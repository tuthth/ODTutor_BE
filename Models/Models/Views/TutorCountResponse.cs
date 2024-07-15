using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorCountResponse
    {
        public int TutorStudent { get; set; }
        public decimal TutorMoney { get; set; }
        public double TutorHour { get; set; }
        public int TutorCourse { get; set; }
    }
}
