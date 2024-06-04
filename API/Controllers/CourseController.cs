using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        /// <summary>
        ///         Course status: 1 ( Active ), 2 (Inactive), 3 (Deleted)
        /// </summary>

        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequest courseRequest)
        {
            var result = await _courseService.CreateCourse(courseRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCourse([FromBody] UpdateCourseRequest courseRequest)
        {
            var result = await _courseService.UpdateCourse(courseRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new {Message = "Cập nhật khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new {Message = "Khoá học đã được gỡ khỏi tìm kiếm trước đó" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("delete/{ID}")]
        public async Task<IActionResult> DeleteCourse(Guid ID)
        {
            var result = await _courseService.DeleteCourse(ID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new {Message = "Xóa khóa học và các thông tin liên quan thành công" } );
                if (statusCodeResult.StatusCode == 200) return Ok(new {Message = "Khóa học được xóa khỏi tìm kiếm. Các mã giảm giá liên quan được xóa. Lịch học liên quan đến khóa học đã được hủy" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new {Message = "Khóa học có giao dịch và đã được gỡ khỏi tìm kiếm trước đó" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("create-outline")]
        public async Task<IActionResult> CreateCourseOutline([FromBody] CourseOutlineRequest courseOutlineRequest)
        {
            var result = await _courseService.CreateCourseOutline(courseOutlineRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new {Message = "Tạo đề cương khóa học thành công"} );
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new {Message = "Không tìm thấy khóa học đang hoạt động" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update-outline")]
        public async Task<IActionResult> UpdateCourseOutline([FromBody] UpdateCourseOutlineRequest courseOutlineRequest)
        {
            var result = await _courseService.UpdateCourseOutline(courseOutlineRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new {Message = "Cập nhật đề cương khóa học thành công" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new {Message = "Đề cương khóa học đã được gỡ khỏi tìm kiếm trước đó, không thể cập nhật" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("delete-outline/{ID}")]
        public async Task<IActionResult> DeleteCourseOutline(Guid ID)
        {
            var result = await _courseService.DeleteCourseOutline(ID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new {Message = "Xóa đề cương khóa học thành công" } );
                if (statusCodeResult.StatusCode == 200) return Ok(new {Message = "Đề cương khóa học được xóa khỏi tìm kiếm, những tài khoản đã lưu tài liệu vẫn có thể download" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new {Message = "Khóa học liên quan đã được gỡ khỏi tìm kiếm trước đó" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new {Message = "Không tìm thấy đề cương khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("create/course-promotion")]
        public async Task<IActionResult> CreateCoursePromotion([FromBody] CoursePromotionRequest coursePromotionRequest)
        {
            var result = await _courseService.CreateCoursePromotion(coursePromotionRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created,new {Message = "Tạo khuyến mãi khóa học thành công" } );
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("delete/course-promotion")]
        public async Task<IActionResult> DeleteCoursePromotion([FromBody] CoursePromotionRequest coursePromotionRequest)
        {
            var result = await _courseService.DeleteCoursePromotion(coursePromotionRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new {Message = "Xóa khuyến mãi khóa học thành công" } );
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update/course-promotion")]
        public async Task<IActionResult> UpdateCoursePromotion([FromBody] CoursePromotionRequest coursePromotionRequest)
        {
            var result = await _courseService.UpdateCoursePromotion(coursePromotionRequest);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new {Message = "Cập nhật khuyến mãi khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("create/promotion")]
        public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotion createPromotion)
        {
            var result = await _courseService.CreatePromotion(createPromotion);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new {Message =  "Tạo khuyến mãi thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update/promotion")]
        public async Task<IActionResult> UpdatePromotion([FromBody] UpdatePromotion updatePromotion)
        {
            var result = await _courseService.UpdatePromotion(updatePromotion);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new {Message = "Cập nhật khuyến mãi thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("delete/promotion/{ID}")]
        public async Task<IActionResult> DeletePromotion(Guid ID)
        {
            var result = await _courseService.DeletePromotion(ID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa khuyến mãi thành công }" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/courses")]
        public async Task<ActionResult<List<Course>>> GetAllCourses()
        {
            var result = await _courseService.GetAllCourses();
            if (result is ActionResult<List<Course>> courses)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courses);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course/{courseID}")]
        public async Task<ActionResult<Course>> GetCourse(Guid courseID)
        {
            var result = await _courseService.GetCourse(courseID);
            if (result is ActionResult<Course> course)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(course);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-outlines")]
        public async Task<ActionResult<List<CourseOutline>>> GetAllCourseOutlines()
        {
            var result = await _courseService.GetAllCourseOutlines();
            if (result is ActionResult<List<CourseOutline>> courseOutlines)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chương trình học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseOutlines);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-outline/{courseOutlineID}")]
        public async Task<ActionResult<CourseOutline>> GetCourseOutline(Guid courseOutlineID)
        {
            var result = await _courseService.GetCourseOutline(courseOutlineID);
            if (result is ActionResult<CourseOutline> courseOutline)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chương trình học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseOutline);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-promotions")]
        public async Task<ActionResult<List<CoursePromotion>>> GetAllCoursePromotions()
        {
            var result = await _courseService.GetAllCoursePromotions();
            if (result is ActionResult<List<CoursePromotion>> coursePromotions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(coursePromotions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-promotion/{coursePromotionID}")]
        public async Task<ActionResult<CoursePromotion>> GetCoursePromotion(Guid coursePromotionID)
        {
            var result = await _courseService.GetCoursePromotion(coursePromotionID);
            if (result is ActionResult<CoursePromotion> coursePromotion)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(coursePromotion);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/promotions")]
        public async Task<ActionResult<List<Promotion>>> GetAllPromotions()
        {
            var result = await _courseService.GetAllPromotions();
            if (result is ActionResult<List<Promotion>> promotions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(promotions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/promotion/{promotionID}")]
        public async Task<ActionResult<Promotion>> GetPromotion(Guid promotionID)
        {
            var result = await _courseService.GetPromotion(promotionID);
            if (result is ActionResult<Promotion> promotion)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(promotion);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
