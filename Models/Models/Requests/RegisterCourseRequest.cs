using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class RegisterCourseRequest
    {
        public Guid StudentId { get;set; }
        public Guid CourseId { get; set; }
    }
    public class CancleCourseRequest : RegisterCourseRequest
    {
        public Guid RegisterCourseId { get; set; }
    }
}
