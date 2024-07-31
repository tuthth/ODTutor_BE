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
                if(statusCodeResult.StatusCode == 404) return Ok(new {Message = "Không tìm thấy slot cần xóa, vui lòng kiểm tra"});
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy slot cần xóa, vui lòng kiểm tra" });
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy slot cần xóa, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa các slot thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("remove/tutor-experience/{id}")]
        public async Task<IActionResult> DeleteTutorExperience(Guid id)
        {
            var result = await _tutorManageService.DeleteTutorExperience(id);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy nội dung cần xóa, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa kinh nghiệm giảng dạy thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("remove/tutor-subject/{id}")]
        public async Task<IActionResult> DeleteTutorSubject(Guid id)
        {
            var result = await _tutorManageService.DeleteTutorSubject(id);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy môn học cần xóa, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa môn học thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("remove/tutor-certificate/{id}")]
        public async Task<IActionResult> DeleteTutorCertificate(Guid id)
        {
            var result = await _tutorManageService.DeleteTutorCertificate(id);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy bằng cấp cần xóa, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa bằng cấp thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("remove/rating-image/{id}")]
        public async Task<IActionResult> DeleteRatingImage(Guid id)
        {
            var result = await _tutorManageService.DeleteRatingImage(id);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy ảnh cần xóa, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa ảnh thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update/tutor-experience")]
        public async Task<IActionResult> UpdateTutorExperience([FromBody] UpdateTutorExperienceRequest request)
        {
            var result = await _tutorManageService.UpdateTutorExperience(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy nội dung cần cập nhật, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Cập nhật kinh nghiệm giảng dạy thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update/tutor-subject")]
        public async Task<IActionResult> UpdateTutorSubject([FromBody] UpdateTutorSubjectRequest request)
        {
            var result = await _tutorManageService.UpdateTutorSubject(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy môn học cần cập nhật, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Cập nhật môn học thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update/tutor-certificate")]
        public async Task<IActionResult> UpdateTutorCertificate([FromBody] UpdateTutorCertificateRequest request)
        {
            var result = await _tutorManageService.UpdateTutorCertificate(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy bằng cấp cần cập nhật, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Cập nhật bằng cấp thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        /// <summary>
        ///  Update lại hết tất cả số đêm chat của tutor
        ///</summary>
        [HttpPut("update/all-count-chat")]
        public async Task<IActionResult> UpdateAllCountChatOfAllTutor()
        {
            var result = await _tutorManageService.UpdateAllCountChatOfAllTutor();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy tutor nào, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Cập nhật số đêm chat thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        /// <summary>
        /// Check and update tutor when end time of package is over
        /// </summary>
        [HttpPut("update/tutor-end-time-package")]
        public async Task<IActionResult> UpdateTutorWhenEndTimeOfPackageIsOver()
        {
            var result = await _tutorManageService.UpdateTutorWhenEndTimeOfPackageIsOver();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy tutor nào, vui lòng kiểm tra" });
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Cập nhật tutor thành công" });
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
