using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class StudentStatisticView
    {
        public Guid StudentId { get; set; }
        public string StudentAvatar { get; set; }
        public string StudentName { get; set; }
        public double TotalHours { get; set; }
    }
}
