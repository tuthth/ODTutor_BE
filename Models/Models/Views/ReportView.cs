using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class ReportView
    {
        public Guid ReportId { get; set; }
        public Guid SenderUserId { get; set; }
        public Guid TargetId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Content { get; set; }
        public int Status { get; set; }
    }
}
