using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class BookingHistoryResponse
    {
        public Guid BookingId { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Message { get; set;}
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public DateTime StudyTime { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? TutorName { get; set; }
        public string? TutorAvatar { get; set; }
        public string? StudentName { get; set; }
        public string? StudentAvatar { get; set; }
        public string? SubjectName { get; set; }
        public bool? IsRated { get; set; }
        public int? RatePoints { get; set; }
        public string? Content { get; set; }
        public DateTime? DateRating { get; set; }
    }
}
