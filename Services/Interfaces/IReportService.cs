using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
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
        Task<IActionResult> MakeActionReportBooking(ReportAction action);
        Task<IActionResult> CreateReportBooking(ReportRequest reportRequest);
        Task<PageResults<ReportResponse>> GetAllReportBookingReport(PagingRequest request);
        Task<ActionResult<ReportDetailResponse>> GetReportDetailByReportId(Guid reportId);
        Task<IActionResult> HandleReportOfTutor(Guid ReportId, Guid ApprovalId);
        Task<IActionResult> DenyReportOfTutor(Guid ReportId, Guid ApprovalId);
        Task<ActionResult<Report>> GetReportByTargetId(Guid id);
    }
}
