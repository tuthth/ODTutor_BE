using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
       private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet("get/bookings")]
        public async Task<ActionResult<List<Booking>>> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookings();
            if (result is ActionResult<List<Booking>> bookings)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch đặt"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookings);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking/{bookingID}")]
        public async Task<ActionResult<Booking>> GetBooking(Guid bookingID)
        {
            var result = await _bookingService.GetBooking(bookingID);
            if (result is ActionResult<Booking> booking)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch đặt"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(booking);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/bookings/student/{studentID}")]
        public async Task<ActionResult<List<Booking>>> GetBookingsByStudentID(Guid studentID)
        {
            var result = await _bookingService.GetBookingsByStudentId(studentID);
            if (result is ActionResult<List<Booking>> bookings)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch đặt"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookings);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/bookings/tutor/{tutorID}")]
        public async Task<ActionResult<List<Booking>>> GetBookingsByTutorID(Guid tutorID)
        {
            var result = await _bookingService.GetBookingsByTutorId(tutorID);
            if (result is ActionResult<List<Booking>> bookings)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch đặt"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookings);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        ///         Booking status: 1 ( Learning ), 2 ( Finsihed ), 3 ( Deleted ), 4 ( Pending )
        /// </summary>
        [HttpPost("create/booking")]
        public async Task<IActionResult> CreateBooking(BookingRequest bookingRequest)
        {
            var result = await _bookingService.CreateBooking(bookingRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest("Dữ liệu không hợp lệ"); }
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Có tài khoản đang bị đình chỉ bởi hệ thống"); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, "Đặt lịch học thành công, vui lòng đến mục Thanh toán");
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update/booking")]
        public async Task<IActionResult> UpdateBooking(UpdateBookingRequest bookingRequest)
        {
            var result = await _bookingService.UpdateBooking(bookingRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest("Dữ liệu không hợp lệ"); }
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Có tài khoản đang bị đình chỉ bởi hệ thống"); }
                    if (statusCodeResult.StatusCode == 200) return Ok();
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("rate/booking/create")]
        public async Task<IActionResult> RateBooking(TutorRatingRequest ratingRequest)
        {
            var result = await _bookingService.RateBookings(ratingRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest("Dữ liệu không hợp lệ"); }
                    if(statusCodeResult.StatusCode == 404) return NotFound("Không tìm thấy lịch đặt đã hoàn thành");
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Tài khoản student đang bị đình chỉ bởi hệ thống"); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, "Đánh giá thành công");
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("rate/booking/update")]
        public async Task<IActionResult> UpdateRating(UpdateTutorRatingRequest ratingRequest)
        {
            var result = await _bookingService.UpdateRating(ratingRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest("Dữ liệu không hợp lệ"); }
                    if (statusCodeResult.StatusCode == 404) return NotFound("Không tìm thấy lịch đặt đã hoàn thành");
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Tài khoản student đang bị đình chỉ bởi hệ thống"); }
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, "Cập nhật đánh giá thành công");
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("rate/booking/delete")]
        public async Task<IActionResult> DeleteRating(Guid bookingID)
        {
            var result = await _bookingService.RemoveRating(bookingID);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) return NotFound("Không tìm thấy lịch đặt đã hoàn thành");
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Tài khoản student đang bị đình chỉ bởi hệ thống"); }
                    if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, "Xóa đánh giá thành công");
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
