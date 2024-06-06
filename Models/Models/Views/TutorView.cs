using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorView
    {
        public Guid TutorId { get; set; }
        public Guid UserId { get; set; }
        public string IdentityNumber { get; set; }
        public string Level { get; set; }
        public decimal? PricePerHour { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string? VideoUrl { get; set; }
    }
}
