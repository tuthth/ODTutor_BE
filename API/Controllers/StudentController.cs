using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        public StudentController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }
        [HttpGet("get/students")]
        public async Task<ActionResult<List<StudentView>>> GetAllStudents()
        {
            var result = await _studentService.GetAllStudents();
            if (result is ActionResult<List<Student>> students && result.Value != null)
            {
                var studentViews = _mapper.Map<List<StudentView>>(students.Value);
                return Ok(studentViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/students/paging")]
        public async Task<ActionResult<PageResults<StudentView>>> GetAllStudentsPaging(int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _studentService.GetAllStudentsPaging(request);
            if (result is ActionResult<PageResults<Student>> users && result.Value != null)
            {
                var userViews = _mapper.Map<PageResults<Student>, PageResults<StudentView>>(users.Value);
                return Ok(userViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy người dùng" }); }
                if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/student/{studentID}")]
        public async Task<ActionResult<StudentView>> GetStudent(Guid studentID)
        {
            var result = await _studentService.GetStudent(studentID);
            if (result is ActionResult<Student> student && result.Value != null)
            {
                var studentView = _mapper.Map<StudentView>(student.Value);
                return Ok(studentView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy sinh viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
