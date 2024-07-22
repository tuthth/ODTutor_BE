using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class CourseSlotRequest
    {
        public Guid CourseId { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateCourseSlotRequest : CourseSlotRequest
    {
        public Guid CourseSlotId { get; set; }
    }
    public class CourseSlotSwapRequest
    {
        public Guid CourseSlotId1 { get; set; }
        public Guid CourseSlotId2 { get; set; }
    }
}
