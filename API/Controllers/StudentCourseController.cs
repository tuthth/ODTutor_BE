using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Views;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCourseController : ControllerBase
    {
        private readonly IStudentCourseService _studentCourseService;
        private readonly IMapper _mapper;

        public StudentCourseController(IStudentCourseService studentCourseService, IMapper mapper)
        {
            _studentCourseService = studentCourseService;
            _mapper = mapper;
        }
        [HttpGet("get/all")]
        public async Task<ActionResult<List<StudentCourseView>>> GetAllStudentCourses()
        {
            var result = await _studentCourseService.GetAllStudentCourses();
            if (result is ActionResult<List<StudentCourse>> studentCourses && result.Value != null)
            {
                var studentCourseViews = _mapper.Map<List<StudentCourseView>>(studentCourses.Value);
                return Ok(studentCourseViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/{studentCourseID}")]
        public async Task<ActionResult<StudentCourseView>> GetStudentCourse(Guid studentCourseID)
        {
            var result = await _studentCourseService.GetStudentCourse(studentCourseID);
            if (result is ActionResult<StudentCourse> studentCourse && result.Value != null)
            {
                var studentCourseView = _mapper.Map<StudentCourseView>(studentCourse.Value);
                return Ok(studentCourseView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/course/{courseID}")]
        public async Task<ActionResult<List<StudentCourseView>>> GetStudentCoursesByCourseID(Guid courseID)
        {
            var result = await _studentCourseService.GetStudentCoursesByCourseId(courseID);
            if (result is ActionResult<List<StudentCourse>> studentCourses && result.Value != null)
            {
                var studentCourseViews = _mapper.Map<List<StudentCourseView>>(studentCourses.Value);
                return Ok(studentCourseViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/student/{studentID}")]
        public async Task<ActionResult<List<StudentCourseView>>> GetStudentCoursesByStudentID(Guid studentID)
        {
            var result = await _studentCourseService.GetStudentCoursesByStudentId(studentID);
            if (result is ActionResult<List<StudentCourse>> studentCourses && result.Value != null)
            {
                var studentCourseViews = _mapper.Map<List<StudentCourseView>>(studentCourses.Value);
                return Ok(studentCourseViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

    }
}
