using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorAccountResponse
    {
        public Guid TutorId { get; set; }
        public Guid UserId { get; set; }
        //public string Level { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string? ActiveTitle { get; set; }
        public string? EducationExperience { get; set; }
        public string? Motivation { get; set; }
        public string? VideoUrl { get; set; }

        // Tutor Tutort Package
        public bool HasBoughtSubscription { get; set; } // Có mua gói dịch vụ hay không 
        public bool HasBoughtExperiencedPackage { get; set; } // Có mua gói dịch vụ kinh nghiệm hay không
        public DateTime? SubcriptionStartDate { get; set; } // Ngày bắt đầu gói dịch vụ
        public DateTime? SubcriptionEndDate { get; set; } // Ngày kết thúc gói dịch vụ
        public int? SubcriptionType { get; set; } // Loại gói dịch vụ

        public List<Course> Courses { get; set; }
        public List<TutorCertificate> TutorCertificates { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<TutorSubject> TutorSubjects { get; set; }
        public List<TutorRating> Ratings { get; set; }
    }
}
