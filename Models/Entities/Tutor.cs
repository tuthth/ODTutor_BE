using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Tutor
    {   
        // Tutor Information Basic 
        public Guid TutorId { get; set; }
        public Guid UserId { get; set; }
        public string IdentityNumber {  get; set; }
        public decimal? PricePerHour { get; set; }
        public string? Description { get; set;}
        public int Status {get; set;}
        public string? EducationExperience { get; set; }
        public string? Motivation { get; set; }
        public string? AttractiveTitle { get; set; }
        public DateTime CreateAt {get;set;}
        public DateTime UpdateAt {get; set;}
        public string? VideoUrl { get; set; }

        // Tutor Tutort Package
        public bool HasBoughtSubscription { get; set; } // Có mua gói dịch vụ hay không 
        public DateTime? SubcriptionStartDate { get; set; } // Ngày bắt đầu gói dịch vụ
        public DateTime? SubcriptionEndDate { get; set; } // Ngày kết thúc gói dịch vụ
        public int? SubcriptionType { get; set; } // Loại gói dịch vụ
        // Check số lần kiểm tra nhắn tin của tutor 
        public int? CountMessageChat { get; set; }

        // Navigation properties
        public virtual User? UserNavigation {get; set;}
        public virtual ICollection<Promotion> PromotionsNavigation { get; set; }
        public virtual ICollection<Course>? CoursesNavigation { get; set; }
        public virtual ICollection<TutorCertificate>? TutorCertificatesNavigation { get; set; }
        public virtual ICollection<TutorSubject>? TutorSubjectsNavigation { get; set; }
        public virtual ICollection<TutorRating>? TutorRatingsNavigation { get; set; }
        public virtual ICollection<TutorAction>? TutorActionsNavigation { get; set; }
        public virtual ICollection<TutorExperience>? TutorExperiencesNavigation { get; set; }
        public virtual ICollection<TutorWeekAvailable>? TutorWeekAvailablesNavigation { get; set; }
        public virtual ICollection<TutorSlotAvailable>? TutorSlotAvailablesNavigation { get; set; }
        public virtual ICollection<TutorDateAvailable>? TutorDateAvailablesNavigation { get; set; }
        public virtual ICollection<Booking>? BookingsNavigation {get; set;}
    }
}
