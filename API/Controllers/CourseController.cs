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
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CourseController(ICourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });
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
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Khoá học đã được gỡ khỏi tìm kiếm trước đó" });

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
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa khóa học và các thông tin liên quan thành công" });
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Khóa học được xóa khỏi tìm kiếm. Các mã giảm giá liên quan được xóa. Lịch học liên quan đến khóa học đã được hủy" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Khóa học có giao dịch và đã được gỡ khỏi tìm kiếm trước đó" });

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
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo đề cương khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học đang hoạt động" });

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
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật đề cương khóa học thành công" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Đề cương khóa học đã được gỡ khỏi tìm kiếm trước đó, không thể cập nhật" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

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
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa đề cương khóa học thành công" });
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Đề cương khóa học được xóa khỏi tìm kiếm, những tài khoản đã lưu tài liệu vẫn có thể download" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Khóa học liên quan đã được gỡ khỏi tìm kiếm trước đó" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy đề cương khóa học" });

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
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo khuyến mãi khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

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
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa khuyến mãi khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

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
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật khuyến mãi khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

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
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo khuyến mãi thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

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
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật khuyến mãi thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("create/course-slot")]
        public async Task<IActionResult> CreateCourseSlot([FromBody] CourseSlotRequest request)
        {
            var result = await _courseService.CreateCourseSlot(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo slot khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Có buổi học trùng nội dung" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update/course-slot")]
        public async Task<IActionResult> UpdateCourseSlot([FromBody] UpdateCourseSlotRequest request)
        {
            var result = await _courseService.UpdateCourseSlot(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật slot khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("delete/course-slot/{ID}")]
        public async Task<IActionResult> DeleteCourseSlot(Guid ID)
        {
            var result = await _courseService.DeleteCourseSlot(ID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Xóa slot khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });

            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("swap-slot-number")]
        public async Task<IActionResult> SwapSlotNumber([FromBody] CourseSlotSwapRequest request)
        {
            var result = await _courseService.SwapSlotNumber(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Đổi số thứ tự slot khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Không thể đổi số thứ tự slot khóa học" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/courses")]
        public async Task<ActionResult<List<CourseView>>> GetAllCourses()
        {
            var result = await _courseService.GetAllCourses();
            if (result is ActionResult<List<Course>> courses && result.Value != null)
            {
                var courseViews = _mapper.Map<List<CourseView>>(courses.Value);
                var courseIn30Days = courseViews.Where(x => x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var courseInPrevious30Days = courseViews.Where(x => x.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = courseInPrevious30Days.Count == 0 ? 100 : (courseIn30Days.Count - courseInPrevious30Days.Count) / courseInPrevious30Days.Count * 100;
                return Ok(new { Courses = courseViews, Count = courseViews.Count, Last30Days = courseIn30Days.Count, PercentageChange = percentageChange });
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/course/{courseID}")]
        public async Task<ActionResult<CourseView>> GetCourse(Guid courseID)
        {
            var result = await _courseService.GetCourse(courseID);
            if (result is ActionResult<Course> course && result.Value != null)
            {
                var courseView = _mapper.Map<CourseView>(course.Value);
                return Ok(courseView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/course-outlines")]
        public async Task<ActionResult<List<CourseOutlineView>>> GetAllCourseOutlines()
        {
            var result = await _courseService.GetAllCourseOutlines();
            if (result is ActionResult<List<CourseOutline>> courseOutlines && result.Value != null)
            {
                var courseOutlineViews = _mapper.Map<List<CourseOutlineView>>(courseOutlines.Value);
                return Ok(new {CourseOutlines = courseOutlineViews, Count = courseOutlineViews.Count});
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chương trình học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/course-outline/{courseOutlineID}")]
        public async Task<ActionResult<CourseOutlineView>> GetCourseOutline(Guid courseOutlineID)
        {
            var result = await _courseService.GetCourseOutline(courseOutlineID);
            if (result is ActionResult<CourseOutline> courseOutline && result.Value != null)
            {
                var courseOutlineView = _mapper.Map<CourseOutlineView>(courseOutline.Value);
                return Ok(courseOutlineView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chương trình học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/course-promotions")]
        public async Task<ActionResult<List<CoursePromotionView>>> GetAllCoursePromotions()
        {
            var result = await _courseService.GetAllCoursePromotions();
            if (result is ActionResult<List<CoursePromotion>> coursePromotions && result.Value != null)
            {
                var coursePromotionViews = _mapper.Map<List<CoursePromotionView>>(coursePromotions.Value);
                var coursePromotionIn30Days = coursePromotionViews.Where(x => x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var coursePromotionInPrevious30Days = coursePromotionViews.Where(x => x.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = coursePromotionInPrevious30Days.Count == 0 ? 100 : (coursePromotionIn30Days.Count - coursePromotionInPrevious30Days.Count) / coursePromotionInPrevious30Days.Count * 100;
                return Ok(new {CoursePromotions = coursePromotionViews, Count = coursePromotionViews.Count, Last30Days = coursePromotionIn30Days.Count, PercentageChange = percentageChange});
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/course-promotion/{coursePromotionID}")]
        public async Task<ActionResult<CoursePromotionView>> GetCoursePromotion(Guid coursePromotionID)
        {
            var result = await _courseService.GetCoursePromotion(coursePromotionID);
            if (result is ActionResult<CoursePromotion> coursePromotion && result.Value != null)
            {
                var coursePromotionView = _mapper.Map<CoursePromotionView>(coursePromotion.Value);
                return Ok(coursePromotionView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/promotions")]
        public async Task<ActionResult<List<PromotionView>>> GetAllPromotions()
        {
            var result = await _courseService.GetAllPromotions();
            if (result is ActionResult<List<Promotion>> promotions && result.Value != null)
            {
                var promotionViews = _mapper.Map<List<PromotionView>>(promotions.Value);
                var promotionIn30Days = promotionViews.Where(x => x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var promotionInPrevious30Days = promotionViews.Where(x => x.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = promotionInPrevious30Days.Count == 0 ? 100 : (promotionIn30Days.Count - promotionInPrevious30Days.Count) / promotionInPrevious30Days.Count * 100;
                return Ok(new { Promotions = promotionViews, Count = promotionViews.Count, Last30Days = promotionIn30Days.Count, PercentageChange = percentageChange });
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/promotions/{tutorID}")]
        public async Task<ActionResult<List<PromotionView>>> GetPromotionsByTutorID(Guid tutorID)
        {
            var result = await _courseService.GetPromotionsByTutorId(tutorID);
            if (result is ActionResult<List<Promotion>> promotions && result.Value != null)
            {
                var promotionViews = _mapper.Map<List<PromotionView>>(promotions.Value);
                var promotionIn30Days = promotionViews.Where(x => x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var promotionInPrevious30Days = promotionViews.Where(x => x.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = promotionInPrevious30Days.Count == 0 ? 100 : (promotionIn30Days.Count - promotionInPrevious30Days.Count) / promotionInPrevious30Days.Count * 100;
                return Ok(new { Promotions = promotionViews, Count = promotionViews.Count, Last30Days = promotionIn30Days.Count, PercentageChange = percentageChange });
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/promotion/{promotionID}")]
        public async Task<ActionResult<PromotionView>> GetPromotion(Guid promotionID)
        {
            var result = await _courseService.GetPromotion(promotionID);
            if (result is ActionResult<Promotion> promotion && result.Value != null)
            {
                var promotionView = _mapper.Map<PromotionView>(promotion.Value);
                return Ok(promotionView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khuyến mãi"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-slots")]
        public async Task<ActionResult<List<CourseSlotView>> > GetAllCourseSlots()
        {
            var result = await _courseService.GetAllCourseSlots();
            if (result is ActionResult<List<CourseSlot>> courseSlots && result.Value != null)
            {
                var courseSlotViews = _mapper.Map<List<CourseSlotView>>(courseSlots.Value);
                return Ok(new { CourseSlots = courseSlotViews, Count = courseSlotViews.Count});
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy slot khóa học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-slot/{courseSlotID}")]
        public async Task<ActionResult<CourseSlotView>> GetCourseSlot(Guid courseSlotID)
        {
            var result = await _courseService.GetCourseSlot(courseSlotID);
            if (result is ActionResult<CourseSlot> courseSlot && result.Value != null)
            {
                var courseSlotView = _mapper.Map<CourseSlotView>(courseSlot.Value);
                return Ok(courseSlotView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy slot khóa học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-slots/{courseID}")]
        public async Task<ActionResult<List<CourseSlotView>>> GetCourseSlotsByCourseID(Guid courseID)
        {
            var result = await _courseService.GetCourseSlotsByCourseId(courseID);
            if (result is ActionResult<List<CourseSlot>> courseSlots && result.Value != null)
            {
                var courseSlotViews = _mapper.Map<List<CourseSlotView>>(courseSlots.Value);
                return Ok(courseSlotViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy slot khóa học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        // Student register course
        [HttpPost("register-course")]
        public async Task<IActionResult> RegisterCourse([FromBody] RegisterCourseRequest request)
        {
            var result = await _courseService.RegisterCourse(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đăng ký khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Số lượng học sinh đã đủ" });
                if (statusCodeResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi không xác định" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        // Cancle course 
        [HttpPost("cancel-course")]
        public async Task<IActionResult> CancelCourse([FromBody] CancleCourseRequest request)
        {
            var result = await _courseService.CancelCourse(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Hủy khóa học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Dữ liệu không hợp lệ" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy khóa học" });
                if (statusCodeResult.StatusCode == 409) return Conflict(new { Message = "Khóa học đã bắt đầu không thể hủy" });
                if (statusCodeResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi không xác định" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
