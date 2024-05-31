using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateReport(ReportRequest reportRequest) {
            var result = await _reportService.CreateReport(reportRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) return NotFound("Không tìm thấy báo cáo");
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, "Tạo báo cáo thành công");
                }
            }
            if(result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
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
                    if (statusCodeResult.StatusCode == 404) return NotFound("Không tìm thấy báo cáo");
                    if (statusCodeResult.StatusCode == 406) return StatusCode(StatusCodes.Status406NotAcceptable, "Trạng thái báo cáo không hợp lệ");
                    if (statusCodeResult.StatusCode == 409) return StatusCode(StatusCodes.Status409Conflict, "Báo cáo đã được xử lý");
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, "Cập nhật báo cáo thành công");
                }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("action")]
        public async Task<IActionResult> MakeActionReport(ReportAction action)
        {
            var result = await _reportService.MakeActionReport(action);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) return NotFound("Không tìm thấy báo cáo");
                    if (statusCodeResult.StatusCode == 409) return StatusCode(StatusCodes.Status409Conflict, "Chỉ chấp thuận các report đã được xử lý");
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, "Xử lý báo cáo thành công");
                }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/all")]
        public async Task<ActionResult<List<Report>>> GetAllReports()
        {
            var result = await _reportService.GetAllReports();
            if (result is ActionResult<List<Report>> reports)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy báo cáo"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(reports);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/{reportID}")]
        public async Task<ActionResult<Report>> GetReport(Guid reportID)
        {
            var result = await _reportService.GetReport(reportID);
            if (result is ActionResult<Report> report)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy báo cáo"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(report);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/create/{userID}")]
        public async Task<ActionResult<List<Report>>> GetReportsByUserID(Guid userID)
        {
            var result = await _reportService.GetReportsByUserId(userID);
            if (result is ActionResult<List<Report>> reports)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy báo cáo"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(reports);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/target/{reporterID}")]
        public async Task<ActionResult<List<Report>>> GetReportsByReporterID(Guid reporterID)
        {
            var result = await _reportService.GetReportsByReporterId(reporterID);
            if (result is ActionResult<List<Report>> reports)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy báo cáo"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(reports);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
