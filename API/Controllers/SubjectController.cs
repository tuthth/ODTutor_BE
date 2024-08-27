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
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;
        public SubjectController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }

        [HttpGet("get/all")]
        public async Task<ActionResult<List<SubjectView>>> GetAllSubjects()
        {
            var result = await _subjectService.GetAllSubjects();
            if (result is ActionResult<List<Subject>> subjects && result.Value != null)
            {
                var subjectViews = _mapper.Map<List<SubjectView>>(subjects.Value);
                return Ok(subjectViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }


        [HttpGet("get/{subjectID}")]
        public async Task<ActionResult<SubjectView>> GetSubject(Guid subjectID)
        {
            var result = await _subjectService.GetSubject(subjectID);
            if (result is ActionResult<Subject> subject && result.Value != null)
            {
                var subjectView = _mapper.Map<SubjectView>(subject.Value);
                return Ok(subjectView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        // Add New Subject
        [HttpPost("add")]
        public async Task<IActionResult> AddNewSubject([FromBody] SubjectAddNewRequest subject)
        {
            var result = await _subjectService.AddNewSubject(subject);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Môn học đã tồn tại" }); }
                    if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Thêm môn học thành công" }); }
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateSubject([FromBody] UpdateSubject subject)
        {
            var result = await _subjectService.UpdateSubject(subject);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học" }); }
                    if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật môn học thành công" }); }
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("delete/{subjectId}")]
        public async Task<IActionResult> DeleteSubject(Guid subjectId)
        {
            var result = await _subjectService.DeleteSubject(subjectId);
            if(result is IActionResult actionResult)
            {
                if(actionResult is StatusCodeResult statusCodeResult)
                {
                    if(statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học" }); }
                    if(statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Không thể xóa môn học do có người dạy môn này, hoặc đã có request dạy môn này" }); }
                    if(statusCodeResult.StatusCode == 204) { return NoContent(); }
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        /// <summary>
        /// Lấy tên của môn học trong booking nè 
        /// </summary>
        [HttpGet("get/tutorSubjectBooking")]
        public async Task<ActionResult<TutorSubjectResponse>> GetTutorSubject(Guid tutorSubjectId)
        {
            var result = await _subjectService.GetTutorSubject(tutorSubjectId);
            if (result is ActionResult<TutorSubjectResponse> tutorSubject && result.Value != null)
            {
                return Ok(tutorSubject.Value);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        /// <summary>
        /// Active or Inactive subject 
        /// </summary>
        [HttpPost("active-inactive/{subjectId}")]
        public async Task<IActionResult> ActiveAndInActiveSubject(Guid subjectId)
        {
            var result = await _subjectService.ActiveAndInActiveSubject(subjectId);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học" }); }
                    if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật trạng thái môn học thành công" }); }
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
