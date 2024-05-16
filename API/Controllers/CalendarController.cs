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
                    if (statusCodeResult.StatusCode == 201) { return Ok("Tạo sự kiện thành công"); }
                    else { return BadRequest("Tạo sự kiện thất bại"); }
                }
                else { return BadRequest("Tạo sự kiện thất bại"); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/
    }
}
