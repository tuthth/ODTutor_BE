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
        [HttpPost("wallet")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> DepositVnpayBooking([FromBody] WalletTransactionCreate transactionCreate)
        {
            try
            {
                //var userId = _userService.GetUserId(HttpContext);
                var transaction = await _transactionService.CreateDepositToAccount(transactionCreate);
                if (transaction is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    else if(statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Giao dịch không rõ trạng thái"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                    else if(statusCodeResult.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi hệ thống"); }
                }
                else if (transaction is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
                return BadRequest("Lấy thông tin thanh toán thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("booking")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> DepositVnpayBooking([FromBody] BookingTransactionCreate transactionCreate)
        {
            try
            {
                //var userId = _userService.GetUserId(HttpContext);
                var transaction = await _transactionService.CreateDepositVnPayBooking(transactionCreate, Guid.NewGuid(), Guid.NewGuid());
                if (transaction is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                }
                else if (transaction is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
                return BadRequest("Lấy thông tin thanh toán thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("course")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> DepositVnpayCourse([FromBody] CourseTransactionCreate transactionCreate)
        {
            try
            {
                //var userId = _userService.GetUserId(HttpContext);
                var transaction = await _transactionService.CreateDepositVnPayCourse(transactionCreate, Guid.NewGuid(), Guid.NewGuid());
                if (transaction is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                }
                else if (transaction is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
                return BadRequest("Lấy thông tin thanh toán thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionRequest updateTransactionRequest)
        {
            try
            {
                var transaction = await _transactionService.UpdateTransaction(updateTransactionRequest.TransactionId, updateTransactionRequest.Choice, updateTransactionRequest.UpdateStatus);
                if (transaction is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Giao dịch đã được xử lý"); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Giao dịch không rõ trạng thái"); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok("Cập nhật giao dịch thành công"); }
                }
                else if (transaction is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
                return BadRequest("Cập nhật giao dịch thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
