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
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public ScheduleController(IScheduleService scheduleService, IMapper mapper)
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
        }
        [HttpGet("get/all")]
        public async Task<ActionResult<List<ScheduleView>>> GetAllSchedules()
        {
            var result = await _scheduleService.GetAllSchedules();
            if (result is ActionResult<List<Schedule>> schedules && result.Value != null)
            {
                var scheduleViews = _mapper.Map<List<ScheduleView>>(schedules.Value);
                return Ok(scheduleViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/{scheduleID}")]
        public async Task<ActionResult<ScheduleView>> GetSchedule(Guid scheduleID)
        {
            var result = await _scheduleService.GetSchedule(scheduleID);
            if (result is ActionResult<Schedule> schedule && result.Value != null)
            {
                var scheduleView = _mapper.Map<ScheduleView>(schedule.Value);
                return Ok(scheduleView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/student-course/{studentCourseID}")]
        public async Task<ActionResult<List<ScheduleView>>> GetSchedulesByStudentCourseID(Guid studentCourseID)
        {
            var result = await _scheduleService.GetSchedulesByStudentCourseId(studentCourseID);
            if (result is ActionResult<List<Schedule>> schedules && result.Value != null)
            {
                var scheduleViews = _mapper.Map<List<ScheduleView>>(schedules.Value);
                return Ok(scheduleViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
