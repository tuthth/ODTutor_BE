using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class StudentCourseView
    {
        public Guid StudentCourseId { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public string? GoogleMeetUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
