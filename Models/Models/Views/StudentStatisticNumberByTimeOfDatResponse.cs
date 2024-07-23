using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class StudentStatisticNumberByTimeOfDatResponse
    {
        public decimal Total { get; set; } = 100;
        public decimal MoringNumber { get; set; }
        public decimal AfternoonNumber { get; set;}
        public decimal EveningNumber { get; set; }
    }
}
