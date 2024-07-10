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
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingController(IBookingService bookingService, IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpGet("get/bookings")]
        public async Task<ActionResult<List<BookingView>>> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookings();
            if (result is ActionResult<List<Booking>> bookings && result.Value != null)
            {
                var bookingViews = _mapper.Map<List<BookingView>>(bookings.Value);
                return Ok(bookingViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch đặt" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/booking/{bookingID}")]
        public async Task<ActionResult<BookingView>> GetBooking(Guid bookingID)
        {
            var result = await _bookingService.GetBooking(bookingID);
            if (result is ActionResult<Booking> booking && result.Value != null)
            {
                var bookingView = _mapper.Map<BookingView>(booking.Value);
                return Ok(bookingView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch đặt" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/bookings/student/{studentID}")]
        public async Task<ActionResult<List<BookingView>>> GetBookingsByStudentID(Guid studentID)
        {
            var result = await _bookingService.GetBookingsByStudentId(studentID);
            if (result is ActionResult<List<Booking>> bookings && result.Value != null)
            {
                var bookingViews = _mapper.Map<List<BookingView>>(bookings.Value);
                return Ok(bookingViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch đặt" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/bookings/tutor/{tutorID}")]
        public async Task<ActionResult<List<BookingView>>> GetBookingsByTutorID(Guid tutorID)
        {
            var result = await _bookingService.GetBookingsByTutorId(tutorID);
            if (result is ActionResult<List<Booking>> bookings && result.Value != null)
            {
                var bookingViews = _mapper.Map<List<BookingView>>(bookings.Value);
                return Ok(bookingViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch đặt" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        /// <summary>
        ///         Booking status: 1 ( Learning ), 2 ( Finsihed ), 3 ( Deleted ), 4 ( Success ), 0 ( Wait for payment ). Dùng 0 với 4 cho lúc thanh toán.
        /// </summary>
        [HttpPost("create/booking")]
        public async Task<IActionResult> CreateBooking(BookingRequest bookingRequest)
        {
            var result = await _bookingService.CreateBooking(bookingRequest);
            if (result is BookingStep1Response response) return StatusCode(StatusCodes.Status201Created, new { Message = "Đặt lịch học thành công, vui lòng đến mục Thanh toán", BookingId = response.BookingId });
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Có tài khoản đang bị đình chỉ bởi hệ thống" }); }
                    //if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đặt lịch học thành công, vui lòng đến mục Thanh toán" });
                }
                if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        /*        [HttpPut("update/booking")]
                public async Task<IActionResult> UpdateBooking(UpdateBookingRequest bookingRequest)
                {
                    var result = await _bookingService.UpdateBooking(bookingRequest);
                    if (result is IActionResult actionResult)
                    {
                        if (actionResult is StatusCodeResult statusCodeResult)
                        {
                            if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                            if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Có tài khoản đang bị đình chỉ bởi hệ thống" }); }
                            if (statusCodeResult.StatusCode == 200) return Ok();
                        }
                        if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
                    }
                    throw new Exception("Lỗi không xác định");
                }*/
        [HttpPost("rate/booking/create")]
        public async Task<IActionResult> RateBooking(TutorRatingRequest ratingRequest)
        {
            var result = await _bookingService.RateBookings(ratingRequest);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt đã hoàn thành" });
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Tài khoản student đang bị đình chỉ bởi hệ thống" }); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đánh giá thành công" });
                }
                if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
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
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt đã hoàn thành" });
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Tài khoản student đang bị đình chỉ bởi hệ thống" }); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đánh giá thành công" });
                }
                if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("start/booking/{bookingID}")]
        public async Task<IActionResult> StartLearning(Guid bookingID)
        {
            var result = await _bookingService.StartLearning(bookingID);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt" });
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Tài khoản student đang bị đình chỉ bởi hệ thống" }); }
                    if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Buổi học hiện không diễn ra" }); }
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, new { Message = "Bắt đầu buổi học thành công" });
                }
                if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("finish/booking/{bookingID}")]
        public async Task<IActionResult> FinishBooking(Guid bookingID)
        {
            var result = await _bookingService.FinishBooking(bookingID);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt" });
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Tài khoản student đang bị đình chỉ bởi hệ thống" }); }
                    if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Buổi học hiện không diễn ra" }); }
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, new { Message = "Kết thúc buổi học thành công" });
                }
                if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
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
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt đã hoàn thành" });
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Tài khoản student đang bị đình chỉ bởi hệ thống" }); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đánh giá thành công" });
                }
                if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
