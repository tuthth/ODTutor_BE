using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        //private readonly IUserService _userService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            //_userService = userService;
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
                        if (statusCodeResult.StatusCode == 404) { return NotFound(new {Message = "Không tìm thấy ví"}); }
                        else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new {Message = "Giao dịch không rõ trạng thái" } ); }
                        else if (statusCodeResult.StatusCode == 409) { return Conflict(new {Message = "Số dư tài khoản không đủ thực hiện giao dịch" }); }
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
                        if (statusCodeResult.StatusCode == 404) { return NotFound(new {Message = "Không tìm thấy ví"}); }
                        else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Chỉ phục vụ giao dịch nâng cấp tài khoản"); }
                        else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Số dư tài khoản không đủ thực hiện giao dịch"}); }
 
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
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new {Message = "Không tìm thấy ví"}); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new {Message = "Giao dịch không rõ trạng thái" } ); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new {Message = "Số dư tài khoản không đủ thực hiện giao dịch" }); }
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
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new {Message = "Không tìm thấy ví"}); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new {Message = "Giao dịch không rõ trạng thái" } ); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new {Message = "Số dư tài khoản không đủ thực hiện giao dịch"}); }
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

        [HttpPost("update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionRequest updateTransactionRequest)
        {
            var transaction = await _transactionService.UpdateTransaction(updateTransactionRequest.TransactionId, updateTransactionRequest.Choice, updateTransactionRequest.UpdateStatus);
            if (transaction is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new {Message = "Không tìm thấy giao dịch" }); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new {Message = "Giao dịch không rõ trạng thái" } ); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new {Message = "Giao dịch đã được xử lý" }); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok(new {Message = "Cập nhật giao dịch thành công" }); }
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

        [HttpGet("get/booking")]
        [Authorize(Roles = "Admin")]
        public async Task<List<BookingTransaction>> GetAll() => await _transactionService.GetAllBooking();

        [HttpGet("get/course")]
        [Authorize(Roles = "Admin")]
        public async Task<List<CourseTransaction>> GetAllCourse() => await _transactionService.GetAllCourse();

        [HttpGet("get/wallet")]
        [Authorize(Roles = "Admin")]
        public async Task<List<WalletTransaction>> GetAllWallet() => await _transactionService.GetAllWallet();

        [HttpGet("get/course-transactions")]
        public async Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions()
        {
            var result = await _transactionService.GetAllCourseTransactions();
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transaction/{courseTransactionID}")]
        public async Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid courseTransactionID)
        {
            var result = await _transactionService.GetCourseTransaction(courseTransactionID);
            if (result is ActionResult<CourseTransaction> courseTransaction)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransaction);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderID(Guid senderID)
        {
            var result = await _transactionService.GetCourseTransactionsBySenderId(senderID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _transactionService.GetCourseTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/course/{courseID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseID(Guid courseID)
        {
            var result = await _transactionService.GetCourseTransactionsByCourseId(courseID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch khóa học" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
         [HttpGet("get/booking-transactions")]
        public async Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions()
        {
            var result = await _transactionService.GetAllBookingTransactions();
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transaction/{bookingTransactionID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransaction(Guid bookingTransactionID)
        {
            var result = await _transactionService.GetBookingTransactionsByBookingId(bookingTransactionID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransaction)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransaction);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderID(Guid senderID)
        {
            var result = await _transactionService.GetBookingTransactionsBySenderId(senderID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _transactionService.GetBookingTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch đặt lịch" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions")]
        public async Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions()
        {
            var result = await _transactionService.GetAllWalletTransactions();
            if (result is ActionResult<List<WalletTransaction>> walletTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transaction/{walletTransactionID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransaction(Guid walletTransactionID)
        {
            var result = await _transactionService.GetWalletTransactionsByWalletTransactionId(walletTransactionID);
            if (result is ActionResult<List<WalletTransaction>> walletTransaction)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransaction);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderID(Guid senderID)
        {
            var result = await _transactionService.GetWalletTransactionsBySenderId(senderID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _transactionService.GetWalletTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch ví" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
