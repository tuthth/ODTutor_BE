using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        //private readonly IUserService _userService;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        /// <summary>
        ///         Transaction choice: 1 ( Deposit ), 2 ( Withdraw )
        /// </summary>
        [HttpPost("wallet")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> DepositVnpayBooking([FromBody] WalletTransactionCreate transactionCreate)
        {
            var transaction = await _transactionService.CreateDepositToAccount(transactionCreate);
            if (transaction is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    {
                        if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                        else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Giao dịch không rõ trạng thái" }); }
                        else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Số dư tài khoản không đủ thực hiện giao dịch" }); }
                    }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        ///         Transaction choice: 3 ( Premium ), not equal 3 == error 406
        /// </summary>
        [HttpPost("premium")]
        public async Task<IActionResult> UpgradeAccount([FromBody] WalletTransactionCreate transactionCreate)
        {
            var transaction = await _transactionService.UpgradeAccount(transactionCreate);
            if (transaction is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    {
                        if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                        else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Chỉ phục vụ giao dịch nâng cấp tài khoản"); }
                        else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Số dư tài khoản không đủ thực hiện giao dịch" }); }

                    }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("booking")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> DepositVnpayBooking([FromBody] BookingTransactionCreate transactionCreate)
        {
            var transaction = await _transactionService.CreateDepositVnPayBooking(transactionCreate);
            if (transaction is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Giao dịch không rõ trạng thái" }); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Số dư tài khoản không đủ thực hiện giao dịch" }); }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        [HttpPost("course")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> DepositVnpayCourse([FromBody] CourseTransactionCreate transactionCreate)
        {
            var transaction = await _transactionService.CreateDepositVnPayCourse(transactionCreate);
            if (transaction is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Giao dịch không rõ trạng thái" }); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Số dư tài khoản không đủ thực hiện giao dịch" }); }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// Update choice: 1 ( Booking ), 2 ( Course ), 3 ( Wallet ); Update status: 0 ( Approve ), 1 ( Pending ) (Auto error), 2 ( Reject )
        /// </summary>

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionRequest updateTransactionRequest)
        {
            var transaction = await _transactionService.UpdateTransaction(updateTransactionRequest.TransactionId, updateTransactionRequest.Choice, updateTransactionRequest.UpdateStatus);
            if (transaction is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch" }); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Giao dịch không rõ trạng thái" }); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Giao dịch đã được xử lý" }); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật giao dịch thành công" }); }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        /// <summary>
        /// VNpay Type status for wallet transaction: 0 (Approve), 1 (Pending), 2 (Reject)
        /// </summary>
        [HttpGet("get/course-transactions/paging")]
        public async Task<ActionResult<PageResults<CourseTransactionView>>> GetAllCourseTransactionsPaging(int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetAllCourseTransactionsPaging(request);
            if (result is ActionResult<PageResults<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<PageResults<CourseTransaction>, PageResults<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions")]
        public async Task<ActionResult<List<CourseTransactionView>>> GetAllCourseTransactions()
        {
            var result = await _transactionService.GetAllCourseTransactions();
            if (result is ActionResult<List<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<List<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transaction/{courseTransactionID}/paging")]
        public async Task<ActionResult<PageResults<CourseTransactionView>>> GetCourseTransactionsBySenderIdPaging(Guid courseTransactionID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetCourseTransactionBySenderIdPaging(courseTransactionID, request);
            if (result is ActionResult<PageResults<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<PageResults<CourseTransaction>, PageResults<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transaction/{courseTransactionID}")]
        public async Task<ActionResult<CourseTransactionView>> GetCourseTransaction(Guid courseTransactionID)
        {
            var result = await _transactionService.GetCourseTransaction(courseTransactionID);
            if (result is ActionResult<CourseTransaction> courseTransaction && result.Value != null)
            {
                var courseTransactionView = _mapper.Map<CourseTransactionView>(courseTransaction.Value);
                return Ok(courseTransactionView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        
        [HttpGet("get/course-transactions/course/{courseID}")]
        public async Task<ActionResult<List<CourseTransactionView>>> GetCourseTransactionsByCourseID(Guid courseID)
        {
            var result = await _transactionService.GetCourseTransactionsByCourseId(courseID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<List<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/course/{courseID}/paging")]
        public async Task<ActionResult<PageResults<CourseTransactionView>>> GetCourseTransactionsByCourseIDPaging(Guid courseID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetCourseTransactionsByCourseIdPaging(courseID, request);
            if (result is ActionResult<PageResults<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<PageResults<CourseTransaction>, PageResults<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<CourseTransactionView>>> GetCourseTransactionsBySenderID(Guid senderID)
        {
            var result = await _transactionService.GetCourseTransactionsBySenderId(senderID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<List<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/sender/{senderID}/paging")]
        public async Task<ActionResult<PageResults<CourseTransactionView>>> GetCourseTransactionsBySenderIDPaging(Guid senderID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetCourseTransactionBySenderIdPaging(senderID, request);
            if (result is ActionResult<PageResults<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<PageResults<CourseTransaction>, PageResults<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/course-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<CourseTransactionView>>> GetCourseTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _transactionService.GetCourseTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<List<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/receiver/{receiverID}/paging")]
        public async Task<ActionResult<PageResults<CourseTransactionView>>> GetCourseTransactionsByReceiverIDPaging(Guid receiverID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetCourseTransactionsByReceiverIdPaging(receiverID, request);
            if (result is ActionResult<PageResults<CourseTransaction>> courseTransactions && result.Value != null)
            {
                var courseTransactionViews = _mapper.Map<PageResults<CourseTransaction>, PageResults<CourseTransactionView>>(courseTransactions.Value);
                return Ok(courseTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/booking-transactions")]
        public async Task<ActionResult<List<BookingTransactionView>>> GetAllBookingTransactions()
        {
            var result = await _transactionService.GetAllBookingTransactions();
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions && result.Value != null)
            {
                var bookingTransactionViews = _mapper.Map<List<BookingTransactionView>>(bookingTransactions.Value);
                return Ok(bookingTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/paging")]
        public async Task<ActionResult<PageResults<BookingTransactionView>>> GetAllBookingTransactionsPaging(int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetAllBookingTransactionsPaging(request);
            if (result is ActionResult<PageResults<BookingTransaction>> bookingTransactions && result.Value != null)
            {
                var bookingTransactionViews = _mapper.Map<PageResults<BookingTransaction>, PageResults<BookingTransactionView>>(bookingTransactions.Value);
                return Ok(bookingTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transaction/{bookingTransactionID}/paging")]
        public async Task<ActionResult<PageResults<BookingTransactionView>>> GetBookingTransactionsByBookingIDPaging(Guid bookingTransactionID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetBookingTransactionsByBookingIdPaging(bookingTransactionID, request);
            if (result is ActionResult<PageResults<BookingTransaction>> bookingTransactions && result.Value != null)
            {
                var bookingTransactionViews = _mapper.Map<PageResults<BookingTransaction>, PageResults<BookingTransactionView>>(bookingTransactions.Value);
                return Ok(bookingTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/booking-transaction/{bookingTransactionID}")]
        public async Task<ActionResult<List<BookingTransactionView>>> GetBookingTransaction(Guid bookingTransactionID)
        {
            var result = await _transactionService.GetBookingTransactionsByBookingId(bookingTransactionID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransaction && result.Value != null)
            {
                var bookingTransactionView = _mapper.Map<List<BookingTransactionView>>(bookingTransaction.Value);
                return Ok(bookingTransactionView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/sender/{senderID}/paging")]
        public async Task<ActionResult<PageResults<BookingTransactionView>>> GetBookingTransactionsBySenderIDPaging(Guid senderID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetBookingTransactionsBySenderIdPaging(senderID, request);
            if (result is ActionResult<PageResults<BookingTransaction>> bookingTransactions && result.Value != null)
            {
                var bookingTransactionViews = _mapper.Map<PageResults<BookingTransaction>, PageResults<BookingTransactionView>>(bookingTransactions.Value);
                return Ok(bookingTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/booking-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<BookingTransactionView>>> GetBookingTransactionsBySenderID(Guid senderID)
        {
            var result = await _transactionService.GetBookingTransactionsBySenderId(senderID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions && result.Value != null)
            {
                var bookingTransactionViews = _mapper.Map<List<BookingTransactionView>>(bookingTransactions.Value);
                return Ok(bookingTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/receiver/{receiverID}/paging")]
        public async Task<ActionResult<PageResults<BookingTransactionView>>> GetBookingTransactionsByReceiverIDPaging(Guid receiverID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetBookingTransactionsByReceiverIdPaging(receiverID, request);
            if (result is ActionResult<PageResults<BookingTransaction>> bookingTransactions && result.Value != null)
            {
                var bookingTransactionViews = _mapper.Map<PageResults<BookingTransaction>, PageResults<BookingTransactionView>>(bookingTransactions.Value);
                return Ok(bookingTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/booking-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<BookingTransactionView>>> GetBookingTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _transactionService.GetBookingTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions && result.Value != null)
            {
                var bookingTransactionViews = _mapper.Map<List<BookingTransactionView>>(bookingTransactions.Value);
                return Ok(bookingTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/paging")]
        public async Task<ActionResult<PageResults<WalletTransactionView>>> GetAllWalletTransactionsPaging(int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetAllWalletTransactionsPaging(request);
            if (result is ActionResult<PageResults<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<PageResults<WalletTransaction>, PageResults<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions")]
        public async Task<ActionResult<List<WalletTransactionView>>> GetAllWalletTransactions()
        {
            var result = await _transactionService.GetAllWalletTransactions();
            if (result is ActionResult<List<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<List<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transaction/{walletTransactionID}/paging")]
        public async Task<ActionResult<PageResults<WalletTransactionView>>> GetWalletTransactionsByWalletTransactionIDPaging(Guid walletTransactionID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetWalletTransactionsByWalletTransactionIdPaging(walletTransactionID, request);
            if (result is ActionResult<PageResults<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<PageResults<WalletTransaction>, PageResults<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transaction/{walletTransactionID}")]
        public async Task<ActionResult<List<WalletTransactionView>>> GetWalletTransaction(Guid walletTransactionID)
        {
            var result = await _transactionService.GetWalletTransactionsByWalletTransactionId(walletTransactionID);
            if (result is ActionResult<List<WalletTransaction>> walletTransaction && result.Value != null)
            {
                var walletTransactionView = _mapper.Map<List<WalletTransactionView>>(walletTransaction.Value);
                return Ok(walletTransactionView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/wallet/{walletID}/paging")]
        public async Task<ActionResult<PageResults<WalletTransactionView>>> GetWalletTransactionsByWalletIDPaging(Guid walletID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetWalletTransactionByWalletIdPaging(walletID, request);
            if (result is ActionResult<PageResults<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<PageResults<WalletTransaction>, PageResults<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/wallet/{walletID}")]
        public async Task<ActionResult<List<WalletTransactionView>>> GetWalletTransactionsByWalletID(Guid walletID)
        {
            var result = await _transactionService.GetWalletTransactionByWalletId(walletID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<List<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/sender/{senderID}/paging")]
        public async Task<ActionResult<PageResults<WalletTransactionView>>> GetWalletTransactionsBySenderIDPaging(Guid senderID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetWalletTransactionsBySenderIdPaging(senderID, request);
            if (result is ActionResult<PageResults<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<PageResults<WalletTransaction>, PageResults<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<WalletTransactionView>>> GetWalletTransactionsBySenderID(Guid senderID)
        {
            var result = await _transactionService.GetWalletTransactionsBySenderId(senderID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<List<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/receiver/{receiverID}/paging")]
        public async Task<ActionResult<PageResults<WalletTransactionView>>> GetWalletTransactionsByReceiverIDPaging(Guid receiverID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetWalletTransactionsByReceiverIdPaging(receiverID, request);
            if (result is ActionResult<PageResults<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<PageResults<WalletTransaction>, PageResults<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<WalletTransactionView>>> GetWalletTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _transactionService.GetWalletTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = _mapper.Map<List<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/account/{accountID}")]
        public async Task<ActionResult<List<WalletTransactionView>>> GetWalletTransactionsByAccountID(Guid accountID)
        {
            var result = await _transactionService.GetWalletTransactionByAccountId(accountID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionViews = new List<WalletTransactionView>();
                foreach (var walletTransaction in walletTransactions.Value)
                {
                    var walletTransactionView = _mapper.Map<WalletTransaction, WalletTransactionView>(walletTransaction);
                    walletTransactionViews.Add(walletTransactionView);
                }
                return Ok(walletTransactionViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/account/{accountID}/paging")]
        public async Task<ActionResult<PageResults<WalletTransactionView>>> GetWalletTransactionsByAccountIDPaging(Guid accountID, int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _transactionService.GetWalletTransactionByAccountIdPaging(accountID, request);
            if (result is ActionResult<PageResults<WalletTransaction>> walletTransactions && result.Value != null)
            {
                var walletTransactionView = _mapper.Map<PageResults<WalletTransaction>, PageResults<WalletTransactionView>>(walletTransactions.Value);
                return Ok(walletTransactionView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
