using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class ReportRequest
    {
        public Guid SenderUserId { get; set; }
        public Guid TargetId { get; set; }
        public string? Content { get; set; }
    }
    public class UpdateReportRequest : ReportRequest
    {
        public Guid ReportId { get; set; }
        public int Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Reason { get; set; }
    }
    public class ReportAction
    {
        public Guid ReportId { get; set; }
        public int Status { get; set; }
    }
}
