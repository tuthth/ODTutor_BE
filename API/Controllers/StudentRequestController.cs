using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    public class StudentRequestController : ControllerBase
    {
        private readonly IStudentRequestService _studentRequestService;
        public StudentRequestController(IStudentRequestService studentRequestService)
        {
            _studentRequestService = studentRequestService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateStudentRequest([FromBody] CreateStudentRequest request)
        {
            var response = await _studentRequestService.CreateStudentRequest(request);
            if(response is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy học sinh hoặc môn học" }); }
                if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Học sinh đang bị đình chỉ khỏi hệ thống" }); }
                if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo yêu cầu thành công" }); }
            }
            if(response is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// Student request status: 1 - Pending, 2 - Accepted, 3 - Rejected
        /// </summary>

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudentRequest([FromBody] UpdateStudentRequest request)
        {
            var response = await _studentRequestService.UpdateStudentRequest(request);
            if (response is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Không tìm thấy học sinh hoặc môn học" }); }
                if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Học sinh đang bị đình chỉ khỏi hệ thống" }); }
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu" }); }
                if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Yêu cầu không thể cập nhật" }); }
            }
            if (response is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/all")]
        public async Task<ActionResult<List<StudentRequest>>> GetAllStudentRequests()
        {
            var result = await _studentRequestService.GetAllStudentRequests();
            if (result is ActionResult<List<StudentRequest>> studentRequests)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequests);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/{studentRequestID}")]
        public async Task<ActionResult<StudentRequest>> GetStudentRequest(Guid studentRequestID)
        {
            var result = await _studentRequestService.GetStudentRequest(studentRequestID);
            if (result is ActionResult<StudentRequest> studentRequest)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequest);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/student/{studentID}")]
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsByStudentID(Guid studentID)
        {
            var result = await _studentRequestService.GetStudentRequestsByStudentId(studentID);
            if (result is ActionResult<List<StudentRequest>> studentRequests)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequests);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/subject/{subjectID}")]
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsBySubjectID(Guid subjectID)
        {
            var result = await _studentRequestService.GetStudentRequestsBySubjectId(subjectID);
            if (result is ActionResult<List<StudentRequest>> studentRequests)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequests);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

    }
}
