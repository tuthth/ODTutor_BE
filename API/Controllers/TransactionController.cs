using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
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
                        if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                        else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Giao dịch không rõ trạng thái"); }
                        else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                        else if (statusCodeResult.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi hệ thống"); }
                    }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
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
                        if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                        else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Chỉ phục vụ giao dịch nâng cấp tài khoản"); }
                        else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                        else if (statusCodeResult.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi hệ thống"); }
                    }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
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
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Giao dịch không rõ trạng thái"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                    else if (statusCodeResult.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi hệ thống"); }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
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
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Giao dịch không rõ trạng thái"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                    else if (statusCodeResult.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi hệ thống"); }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
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
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch"); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Giao dịch không rõ trạng thái"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Giao dịch đã được xử lý"); }
                    else if (statusCodeResult.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi hệ thống"); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok("Cập nhật giao dịch thành công"); }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (transaction is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
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

        [HttpGet("get/booking/{id}")]
        [Authorize]
        public async Task<BookingTransaction> GetBooking(Guid id) => await _transactionService.GetBookingTransactionById(id);

        [HttpGet("get/course/{id}")]
        [Authorize]
        public async Task<CourseTransaction> GetCourse(Guid id) => await _transactionService.GetCourseTransactionById(id);

        [HttpGet("get/wallet/{id}")]
        [Authorize]
        public async Task<WalletTransaction> GetWallet(Guid id) => await _transactionService.GetWalletTransactionById(id);
    }
}
