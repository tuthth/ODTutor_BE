using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ReportController(IReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        /// <summary>
        ///         Report enum: 1 (Finished), 2 (Processing), 3 (Cancelled)
        /// </summary>

        [HttpPost("create")]
        public async Task<IActionResult> CreateReport(ReportRequest reportRequest) {
            var result = await _reportService.CreateReport(reportRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) return Ok(new {Message = "Không tìm thấy báo cáo" });
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new {Message = "Tạo báo cáo thành công"});
                }
            }
            if(result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateReport(UpdateReportRequest updateReportRequest)
        {
            var result = await _reportService.UpdateReport(updateReportRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) return Ok(new {Message = "Không tìm thấy báo cáo" });
                    if (statusCodeResult.StatusCode == 406) return StatusCode(StatusCodes.Status406NotAcceptable, new {Message = "Trạng thái báo cáo không hợp lệ" } );
                    if (statusCodeResult.StatusCode == 409) return StatusCode(StatusCodes.Status409Conflict, new {Message = "Báo cáo đã được xử lý" });
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, new {Message = "Cập nhật báo cáo thành công" } );
                }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        ///             Report action enum: 1 (SevenDays), 2 (ThirtyDays), 3 (NinetyDays), 4 (Lifetime - 30 years)
        /// </summary>
       
        [HttpPost("action")]
        public async Task<IActionResult> MakeActionReport(ReportAction action)
        {
            var result = await _reportService.MakeActionReport(action);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) return Ok(new {Message = "Không tìm thấy báo cáo" });
                    if (statusCodeResult.StatusCode == 409) return StatusCode(StatusCodes.Status409Conflict, "Chỉ chấp thuận các report đã được xử lý");
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, "Xử lý báo cáo thành công");
                }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/all")]
        public async Task<ActionResult<List<ReportView>>> GetAllReports()
        {
            var result = await _reportService.GetAllReports();
            if (result is ActionResult<List<Report>> reports && result.Value != null)
            {
                var reportViews = _mapper.Map<List<ReportView>>(reports.Value);
                var reportIn30Days = reportViews.Where(report => report.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var reportInPrevious30Days = reportViews.Where(report => report.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && report.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = reportInPrevious30Days.Count == 0 ? 0 : (reportIn30Days.Count - reportInPrevious30Days.Count) / reportInPrevious30Days.Count * 100;
                return Ok(new { ReportIn30Days = reportIn30Days, ReportInPrevious30Days = reportInPrevious30Days, PercentageChange = percentageChange });
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy báo cáo" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/{reportID}")]
        public async Task<ActionResult<ReportView>> GetReport(Guid reportID)
        {
            var result = await _reportService.GetReport(reportID);
            if (result is ActionResult<Report> report && result.Value != null)
            {
                var reportView = _mapper.Map<ReportView>(report.Value);
                return Ok(reportView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy báo cáo" }); }

            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/create/{userID}")]
        public async Task<ActionResult<List<ReportView>>> GetReportsByUserID(Guid userID)
        {
            var result = await _reportService.GetReportsByUserId(userID);
            if (result is ActionResult<List<Report>> reports && result.Value != null)
            {
                var reportViews = _mapper.Map<List<ReportView>>(reports.Value);
                var reportIn30Days = reportViews.Where(report => report.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var reportInPrevious30Days = reportViews.Where(report => report.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && report.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = reportInPrevious30Days.Count == 0 ? 0 : (reportIn30Days.Count - reportInPrevious30Days.Count) / reportInPrevious30Days.Count * 100;
                return Ok(new { ReportIn30Days = reportIn30Days, ReportInPrevious30Days = reportInPrevious30Days, PercentageChange = percentageChange });
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy báo cáo" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/target/{reporterID}")]
        public async Task<ActionResult<List<ReportView>>> GetReportsByReporterID(Guid reporterID)
        {
            var result = await _reportService.GetReportsByReporterId(reporterID);
            if (result is ActionResult<List<Report>> reports && result.Value != null)
            {
                var reportViews = _mapper.Map<List<ReportView>>(reports.Value);
                var reportIn30Days = reportViews.Where(report => report.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var reportInPrevious30Days = reportViews.Where(report => report.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && report.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = reportInPrevious30Days.Count == 0 ? 0 : (reportIn30Days.Count - reportInPrevious30Days.Count) / reportInPrevious30Days.Count * 100;
                return Ok(new { ReportIn30Days = reportIn30Days, ReportInPrevious30Days = reportInPrevious30Days, PercentageChange = percentageChange });
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy báo cáo" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
