using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("deposit/vnpay")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> DepositVnpay([FromBody] TransactionCreate transactionCreate)
        {
            try
            {
                //var userId = _userService.GetUserId(HttpContext);
                var transaction = await _transactionService.CreateDepositVnPay(transactionCreate, Guid.NewGuid(), Guid.NewGuid());
                if (transaction is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
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


    }
}
