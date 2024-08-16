using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class ReportResponse
    {
        public Guid ReportId { get; set; }
        public Guid BookingId { get; set; }
        public Guid SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderAvatar { get; set; }
        public string? TutorName { get; set; }
        public string? TutorAvatar { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public List<string> ReportImages { get; set; }
    }
}
