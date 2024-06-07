using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorManageController : ControllerBase
    {
       private readonly ITutorManageService _tutorManageService;
        public TutorManageController(ITutorManageService tutorManageService)
        {
            _tutorManageService = tutorManageService;
        }
        /// <summary>
        /// Xóa các slot theo danh sách ID ( FE có thể dùng select )
        /// </summary>
       
        [HttpDelete("remove/slot/ids")]
        public async Task<IActionResult> DeleteTutorSlots([FromBody] List<Guid> tutorSlotAvailableIDs)
        {
            var result = await _tutorManageService.DeleteTutorSlots(tutorSlotAvailableIDs);
            if(result is StatusCodeResult statusCodeResult)
            {
                if(statusCodeResult.StatusCode == 404) return NotFound(new {Message = "Không tìm thấy slot cần xóa, vui lòng kiểm tra"});
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa các slot thành công" });
            }
            if(result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// Xóa các slot theo khoảng thời gian trong 1 ngày
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpDelete("remove/slot/date")]
        public async Task<IActionResult> DeleteSlotInTimeRangeInDate([FromBody] TutorDateRemoveSlotRequest request)
        {
            var result = await _tutorManageService.DeleteSlotInTimeRangeInDate(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy slot cần xóa, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa các slot thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// Xóa các slot theo khoảng thời gian, cái này chỉ cần nhìn ngày và giờ, khuyến nghị test bằng Postman
        /// </summary>
        [HttpDelete("remove/slot/date/range")]
        public async Task<IActionResult> DeleteSlotInTimeRange([FromBody] TutorDateRemoveRangeRequest request)
        {
            var result = await _tutorManageService.DeleteSlotInTimeRange(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy slot cần xóa, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa các slot thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
