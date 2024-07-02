using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class CourseSlotView
    {
        public Guid CourseSlotId { get; set; }
        public Guid CourseId { get; set; }
        public int SlotNumber { get; set; }
        public string? Description { get; set; }
    }
}
