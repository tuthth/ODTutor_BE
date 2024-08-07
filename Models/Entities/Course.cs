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
        public Guid TutorSubjectId { get; set; }
        public DateTime CreatedAt {  get; set; }
        public string? Description { get; set; }
        public decimal TotalMoney { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        public DateTime? StudyTime { get; set; }
        public string? ImageUrl { get; set; }
        public virtual Tutor? TutorNavigation { get; set; }
        public int TotalStudent { get; set; }
        public string? GoogleMeetUrl { get; set; }

        public virtual ICollection<StudentCourse>? StudentCoursesNavigation { get; set; }
        public virtual ICollection<CourseOutline>? CourseOutlinesNavigation { get; set; }
        public virtual ICollection<CoursePromotion>? CoursePromotionsNavigation { get; set; }
        public virtual ICollection<CourseSlot>? CourseSlotsNavigation { get; set; }
        public virtual CourseTransaction? CourseTransactionNavigation {  get; set; }
        public virtual TutorSubject? TutorSubjectNavigation { get; set; }
    }
}
