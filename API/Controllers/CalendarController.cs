using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Settings.Google.Calendar;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IGoogleCalendarService _googleCalendarService;

        public CalendarController(IGoogleCalendarService googleCalendarService)
        {
            _googleCalendarService = googleCalendarService;
        }

        [HttpPost("event/create")]
        public async Task<IActionResult> CreateCalendarEvent(GGCalendarEventSetting setting)
        {
            try
            {
                var checkCreateEvent = await _googleCalendarService.CreateCalendarEvent(setting);
                if (checkCreateEvent is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest("Không nhận được thông tin email người tham dự, vui lòng kiểm tra lại thông tin"); }
                    if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden ,"Không nhận được thông tin RedirectUri, vui lòng kiểm tra lại thông tin"); }
                    if (statusCodeResult.StatusCode == 404) { return StatusCode(StatusCodes.Status404NotFound ,"Email người tham dự không tồn tại hoặc đã bị cấm, vui lòng kiểm tra lại thông tin"); }
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable ,"Thời gian kết thúc sự kiện không thể trước thời gian bắt đầu sự kiện, vui lòng kiểm tra lại thông tin"); }
                    if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict ,"Thời gian bắt đầu sự kiện không thể trước thời gian hiện tại, vui lòng kiểm tra lại thông tin"); }
                }
                if(checkCreateEvent is JsonResult jsonResult) { return Ok(jsonResult.Value); }
                if(checkCreateEvent is Exception ex){ return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString()); }
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
