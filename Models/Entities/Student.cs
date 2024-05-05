using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Student
    {
        public Guid StudentId {  get; set; }
        public Guid UserId { get; set; }

        public virtual User? UserNavigation { get; set; }
        public virtual ICollection<StudentCourse>? StudentCoursesNavigation { get; set; }
        public virtual ICollection<Booking>? BookingsNavigation {  get; set; }
        public virtual ICollection<TutorRating>? TutorRatingsNavigation {  get; set; } 
    }
}
