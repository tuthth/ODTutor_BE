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
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public ScheduleController(IScheduleService scheduleService, IMapper mapper)
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
        }
        /// <summary>
        /// schedule status: 0 - pending, 1 - started, 2 - finished, 3 - cancelled
        /// </summary>
        /// <param name="scheduleRequest"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("create")]
        public async Task<IActionResult> CreateSchedulesForStudentCourse(ScheduleRequest scheduleRequest)
        {
            var result = await _scheduleService.CreateSchedulesForStudentCourse(scheduleRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) { return StatusCode(201, new { Message = "Tạo lịch học thành công" }); }
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học" }); }
                if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Khóa học đã kết thúc" }); }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// reschedule toàn bộ course, không thể thực hiện trước khi bắt đầu slot đầu tiên trong 24h
        /// </summary>
        /// <param name="scheduleRequest"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut("reschedule/student-course")]
        public async Task<IActionResult> ReScheduleForStudentCourse(ScheduleRequest scheduleRequest)
        {
            var result = await _scheduleService.ReScheduleForStudentCourse(scheduleRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Đổi lịch học thành công" }); }
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch học" }); }
                if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Không thể thay đổi lịch học trước khi bắt đầu slot đầu tiên trong 24h" });}
                if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Khóa học đã kết thúc" }); }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("reschedule/slot")]
        public async Task<IActionResult> RescheduleSlot(RescheduleRequest rescheduleRequest)
        {
            var result = await _scheduleService.RescheduleSlot(rescheduleRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Đổi lịch học thành công" }); }
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch học" }); }
                if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Không thể thay đổi lịch học trước khi bắt đầu slot đầu tiên trong 24h" });}
                if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Khóa học đã kết thúc" }); }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("start/{scheduleID}")]
        public async Task<IActionResult> StartSchedule(Guid scheduleID)
        {
            var result = await _scheduleService.StartSchedule(scheduleID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Bắt đầu lịch học thành công" }); }
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch học" }); }
                if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Lịch học đã bắt đầu hoặc đã kết thúc" }); }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("finish/{scheduleID}")]
        public async Task<IActionResult> FinishSchedule(Guid scheduleID)
        {
            var result = await _scheduleService.FinishSchedule(scheduleID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Kết thúc lịch học thành công" }); }
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch học" }); }
                if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Lịch học chưa bắt đầu hoặc đã kết thúc" }); }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
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
