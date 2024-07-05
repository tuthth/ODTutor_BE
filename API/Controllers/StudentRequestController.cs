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
    public class StudentRequestController : ControllerBase
    {
        private readonly IStudentRequestService _studentRequestService;
        private readonly IMapper _mapper;

        public StudentRequestController(IStudentRequestService studentRequestService, IMapper mapper)
        {
            _studentRequestService = studentRequestService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudentRequest([FromBody] CreateStudentRequest request)
        {
            var response = await _studentRequestService.CreateStudentRequest(request);
            if (response is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy học sinh hoặc môn học" }); }
                if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Học sinh đang bị đình chỉ khỏi hệ thống" }); }
                if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo yêu cầu thành công" }); }
            }
            if (response is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// Student request status: 1 - Pending, 2 - Accepted
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
                if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật yêu cầu thành công" }); }
            }
            if (response is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        // Update student request V2 
        [HttpPost("update/v2")]
        public async Task<IActionResult> UpdateStudentRequestV2([FromBody] UpdateStudentRequest request)
        {
            var response = await _studentRequestService.UpdateStudentRequest(request);
            if (response is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Không tìm thấy học sinh hoặc môn học" }); }
                if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Học sinh đang bị đình chỉ khỏi hệ thống" }); }
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu" }); }
                if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Yêu cầu không thể cập nhật" }); }
                if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật yêu cầu thành công" }); }
            }
            if (response is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/all")]
        public async Task<ActionResult<List<StudentRequestView>>> GetAllStudentRequests()
        {
            var result = await _studentRequestService.GetAllStudentRequests();
            if (result is ActionResult<List<StudentRequest>> studentRequests && result.Value != null)
            {
                var studentRequestViews = _mapper.Map<List<StudentRequestView>>(studentRequests.Value);
                return Ok(studentRequestViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/all/paging")]
        public async Task<ActionResult<PageResults<StudentRequestView>>> GetAllStudentRequestsPaging([FromQuery] PagingRequest request)
        {
            var result = await _studentRequestService.GetAllStudentRequestsPaging(request);
            if (result is ActionResult<PageResults<StudentRequest>> studentRequests && result.Value != null)
            {
                var studentRequestViews = _mapper.Map<PageResults<StudentRequestView>>(studentRequests.Value);
                return Ok(studentRequestViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/{studentRequestID}")]
        public async Task<ActionResult<StudentRequestView>> GetStudentRequest(Guid studentRequestID)
        {
            var result = await _studentRequestService.GetStudentRequest(studentRequestID);
            if (result is ActionResult<StudentRequest> studentRequest && result.Value != null)
            {
                var studentRequestView = _mapper.Map<StudentRequestView>(studentRequest.Value);
                return Ok(studentRequestView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/student/{studentID}")]
        public async Task<ActionResult<List<StudentRequestView>>> GetStudentRequestsByStudentID(Guid studentID)
        {
            var result = await _studentRequestService.GetStudentRequestsByStudentId(studentID);
            if (result is ActionResult<List<StudentRequest>> studentRequests && result.Value != null)
            {
                var studentRequestViews = _mapper.Map<List<StudentRequestView>>(studentRequests.Value);
                return Ok(studentRequestViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student/{studentID}/paging")]
        public async Task<ActionResult<PageResults<StudentRequestView>>> GetStudentRequestsByStudentIDPaging(Guid studentID, [FromQuery] PagingRequest request)
        {
            var result = await _studentRequestService.GetStudentRequestsByStudentIdPaging(studentID, request);
            if (result is ActionResult<PageResults<StudentRequest>> studentRequests && result.Value != null)
            {
                var studentRequestViews = _mapper.Map<PageResults<StudentRequestView>>(studentRequests.Value);
                return Ok(studentRequestViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/subject/{subjectID}")]
        public async Task<ActionResult<List<StudentRequestView>>> GetStudentRequestsBySubjectID(Guid subjectID)
        {
            var result = await _studentRequestService.GetStudentRequestsBySubjectId(subjectID);
            if (result is ActionResult<List<StudentRequest>> studentRequests && result.Value != null)
            {
                var studentRequestViews = _mapper.Map<List<StudentRequestView>>(studentRequests.Value);
                return Ok(studentRequestViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }

            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/subject/{subjectID}/paging")]
        public async Task<ActionResult<PageResults<StudentRequestView>>> GetStudentRequestsBySubjectIDPaging(Guid subjectID, [FromQuery] PagingRequest request)
        {
            var result = await _studentRequestService.GetStudentRequestsBySubjectIdPaging(subjectID, request);
            if (result is ActionResult<PageResults<StudentRequest>> studentRequests && result.Value != null)
            {
                var studentRequestViews = _mapper.Map<PageResults<StudentRequestView>>(studentRequests.Value);
                return Ok(studentRequestViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu sinh viên" }); }

            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        // Delete student request
        [HttpPost("delete/{studentRequestID}")]
        public async Task<IActionResult> DeleteStudentRequest(Guid studentRequestID)
        {
            var response = await _studentRequestService.DeleteStudentRequest(studentRequestID);
            if (response is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy yêu cầu" }); }
                if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Yêu cầu không thể xóa" }); }
                if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Xóa yêu cầu thành công" });}
            }
            if (response is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        // Get student requests
        [HttpGet("get/status")]
        public async Task<ActionResult<List<StudentRequestView>>> GetStudentRequestsByStatus()
        {
                var result = await _studentRequestService.GetStudentRequestsByStatus();
            return Ok(result);
        }
        [HttpGet("get/status/paging")]
        public async Task<ActionResult<PageResults<StudentRequestView>>> GetStudentRequestsByStatusPaging([FromQuery] PagingRequest request)
        {
            var result = await _studentRequestService.GetStudentRequestsByStatusPaging(request);
            return Ok(result);
        }

    }
}
