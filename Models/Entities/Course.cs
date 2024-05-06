using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Course
    {
        public Guid CourseId { get; set; }
        public Guid TutorId { get; set; }
        public DateTime CreatedAt {  get; set; }
        public string? Description { get; set; }
        public decimal TotalMoney { get; set; }
        public int TotalSlots { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }

        public virtual Tutor? TutorNavigation { get; set; }
        public virtual ICollection<StudentCourse>? StudentCoursesNavigation { get; set; }
        public virtual ICollection<CourseOutline>? CourseOutlinesNavigation { get; set; }
        public virtual ICollection<CoursePromotion>? CoursePromotionsNavigation { get; set; }
        public virtual CourseTransaction? CourseTransactionNavigation {  get; set; }
    }
}
