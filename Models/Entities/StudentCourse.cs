using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class StudentCourse
    {
        public Guid StudentCourseId { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public int Status {  get; set; }
        public DateTime CreatedAt {  get; set; } = DateTime.UtcNow;
        public string? GoogleMeetUrl { get; set; }
        public virtual Course? CourseNavigation { get; set; }
        public virtual Student? StudentNavigation { get; set; }
        public virtual ICollection<Schedule>? SchedulesNavigations { get; set; }
    }
}
