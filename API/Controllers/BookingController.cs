﻿using AutoMapper;
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
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public BookingController(IBookingService bookingService,IReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
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
                foreach (var booking in bookingViews)
                {   
                    var reportCount = await _reportService.GetReportByTargetId(booking.BookingId);
                    booking.IsReported = reportCount.Value != null;
                }
                var bookingIn30Days = bookingViews.Where(b => b.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var bookingInPrevious30Days = bookingViews.Where(b => b.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && b.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = bookingInPrevious30Days.Count == 0 ? 0 : (bookingIn30Days.Count - bookingInPrevious30Days.Count) / bookingInPrevious30Days.Count * 100;
                return Ok(new {Bookings = bookingViews, Last30Days = bookingIn30Days.Count, PercentageChange = percentageChange});
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
                    var reportCount = await _reportService.GetReportByTargetId(bookingView.BookingId);
                    bookingView.IsReported = reportCount.Value != null;
                
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
                foreach (var booking in bookingViews)
                {
                    // Lấy Report bằng TargetId (BookingId)
                    var reportResult = await _reportService.GetReportByTargetId(booking.BookingId);

                    if (reportResult.Value != null)
                    {
                        booking.ReportId = reportResult.Value.ReportId; // Gán ReportId từ Report vào BookingView
                        booking.IsReported = true;
                    }
                    else
                    {
                        booking.ReportId = null; // Hoặc Guid.Empty nếu bạn muốn giữ định dạng Guid
                        booking.IsReported = false;
                    }
                }
                var bookingIn30Days = bookingViews.Where(b => b.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var bookingInPrevious30Days = bookingViews.Where(b => b.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && b.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = bookingInPrevious30Days.Count == 0 ? 0 : (bookingIn30Days.Count - bookingInPrevious30Days.Count) / bookingInPrevious30Days.Count * 100;
                return Ok(new { Bookings = bookingViews, Count = bookingViews.Count, Last30Days = bookingIn30Days.Count, PercentageChange = percentageChange });
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
                foreach (var booking in bookingViews)
                {
                    var reportCount = await _reportService.GetReportByTargetId(booking.BookingId);
                    booking.IsReported = reportCount.Value != null;
                }
                var bookingIn30Days = bookingViews.Where(b => b.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var bookingInPrevious30Days = bookingViews.Where(b => b.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && b.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = bookingInPrevious30Days.Count == 0 ? 0 : (bookingIn30Days.Count - bookingInPrevious30Days.Count) / bookingInPrevious30Days.Count * 100;
                return Ok(new { Bookings = bookingViews, Count = bookingViews.Count, Last30Days = bookingIn30Days.Count, PercentageChange = percentageChange });
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch đặt" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        /// <summary>
        /// Booking status: 1 ( Learning ), 2 ( Finsihed ), 3 ( Deleted ), 4 ( Success ), 0 ( Wait for payment ), 5 ( WaitingReschdeduleForStudent), 6 ( WaitingReschdeuleForTutor). Dùng 0 với 4 cho lúc thanh toán.
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

        // Add Google Meet URL into Booking 
        ////    
        ///<summary>
        /// Gắn đường link meet vô booking    
        /// </summary>
        [HttpPost("add/googlemeet/{bookingID}/{link}")]
        public async Task<IActionResult> AddLinkMeeting(Guid bookingID, string link)
        {
            var result = await _bookingService.AddGoogleMeetUrl(bookingID, link);
            return result;
        }

        ///<summary>
        /// Lấy link booking 
        /// </summary>

        [HttpGet("get/googlemeet/{bookingID}")]
        public async Task<ActionResult<string>> GetLinkMeeting(Guid bookingID)
        {
            var result = await _bookingService.GetGoogleMeetUrl(bookingID);
            return result;
        }

        ///<summary>
        /// Cập nhật link booking
        /// </summary>
        [HttpPut("update/googlemeet/{bookingID}/{link}")]
        public async Task<IActionResult> UpdateLinkMeeting(Guid bookingID, string link)
        {
            var result = await _bookingService.UpdateGoogleMeetUrl(bookingID, link);
            return result;
        }

        ///<summary>
        /// Dời Lịch học 
        ///// <summary>
        [HttpPost("reschedule/booking/{bookingID}/{senderID}/{newSlotId}/{message}")]
        public async Task<IActionResult> RescheduleBooking(Guid bookingID, Guid senderID, Guid newSlotId, string message)
        {
            var result = await _bookingService.RescheduleBooking(bookingID, senderID, newSlotId, message);
            return result;
        }

        ///<summary>
        /// Xác nhận dời lịch học 
        /// </summary>
        [HttpPut("confirm/reschedule/{bookingID}")]
        public async Task<IActionResult> ConfirmRescheduleBooking(Guid bookingID)
        {
            var result = await _bookingService.ConfirmRescheduleBooking(bookingID);
            return result;
        }

        ///<summary>
        /// Rating không cần hình
        /// </summary>
        [HttpPost("rate/booking/create/withoutimage")]
        public async Task<IActionResult> RateBookingWithoutImage(  TutorRatingRequest ratingRequest)
        {
            var result = await _bookingService.RateBookingsWithoutImage(ratingRequest);
            return result;
        }

        ///</summary>
        /// Rating không cần hình version 2
        /// </summary>
        [HttpPost("rate/booking/create/withoutimage/v2")]
        public async Task<IActionResult> RateBookingWithoutImageV2([FromBody]TutorRatingRequest ratingRequest)
        {
            var result = await _bookingService.RateBookingsWithoutImage(ratingRequest);
            if(result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt đã hoàn thành" });
                    if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Tài khoản student đang bị đình chỉ bởi hệ thống" }); }
                    if(statusCodeResult.StatusCode ==409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Đánh giá đã tồn tại" }); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đánh giá thành công" });
                }
                if (actionResult is Exception exception) StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            return result;
        }

        ///<summary>
        /// Từ chối dời lịch 
        /// </summary>
        [HttpPut("reject/reschedule/{bookingID}")]
        public async Task<IActionResult> RejectRescheduleBooking(Guid bookingID)
        {
            var result = await _bookingService.RejectRescheduleBooking(bookingID);
            return result;
        }

        /// <summary>
        /// Tạo booking version 2 ( Sau review ngày thứ 7, 27/07/2024)
        /// </summary>
        [HttpPost("create/booking/v2")]
        public async Task<IActionResult> CreateBookingVersion2(BookingRequestV2 bookingRequest)
        {
            var result = await _bookingService.CreateBookingVersion2(bookingRequest);
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

        ///<summary>
        /// Cập nhật trạng thái bắt đầu học của tất cả booking
        /// </summary>
        [HttpPut("update/startlearning/all")]
        public async Task<IActionResult> UpdateStartLearningOfAllBooking()
        {
            var result = await _bookingService.UpdateStartLearningOfAllBoooking();
            if(result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật trạng thái bắt đầu học thành công" });
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi không xác định" });
        }

        ///<summary>
        /// Cập nhật trạng thái kết thúc học của tất cả booking
        /// </summary>
        [HttpPut("update/finishbooking/all")]
        public async Task<IActionResult> UpdateFinishBookingOfAllBooking()
        {
            var result = await _bookingService.UpdateFinishBookingOfAllBooking();
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật trạng thái kết thúc học thành công" });
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi không xác định" });
        }

        // 
        ///<summary>
        ///Get Tutor's booking history
        /// </summary>
        [HttpGet("get/bookinghistory/tutor/{tutorID}")]
        public async Task<ActionResult<PageResults<BookingHistoryResponse>>> GetBookingHistory(Guid tutorID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var response = await _bookingService.GetBookingHistoryTutor(tutorID, request);
            if (response is ActionResult<PageResults<BookingHistoryResponse>> result && result.Value != null)
            {
                return Ok(result.Value);
            }
            if ((IActionResult)response.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy lịch sử đặt lịch" }); }
            }
            if ((IActionResult)response.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }


        ///<summary>
        /// Check Booking When is in Waiting for Schedule but do not have accept
        /// </summary>
        [HttpPut("check/allBooking/isWaiting")]
        public async Task<IActionResult> CheckBookingIsRequiredChangeSchedule()
        {
            var result = await _bookingService.CheckBookingIsRequiredChangeSchedule();
            if(result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Cập nhật trạng thái bắt đầu học thành công" });
                    if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy lịch đặt như yêu cầu" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi không xác định" });
        }
    }
}
