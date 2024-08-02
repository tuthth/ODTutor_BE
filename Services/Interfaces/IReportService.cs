using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IReportService
    {
        Task<IActionResult> CreateReport(ReportRequest reportRequest);
        Task<IActionResult> UpdateReport(UpdateReportRequest updateReportRequest);
        Task<IActionResult> MakeActionReportWithUser(ReportAction action);
        Task<ActionResult<List<Report>>> GetAllReports();
        Task<ActionResult<Report>> GetReport(Guid id);
        Task<ActionResult<List<Report>>> GetReportsByUserId(Guid id);
        Task<ActionResult<List<Report>>> GetReportsByReporterId(Guid id);
    }
}
