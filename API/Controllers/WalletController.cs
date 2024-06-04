using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Entities;
using Models.Enumerables;
using Services.Implementations;
using Services.Interfaces;
using Settings.VNPay;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        //private readonly IUserService _userService;
        private readonly VNPaySetting vnPaySetting;

        public WalletController(IWalletService walletService, IOptions<VNPaySetting> options, ITransactionService transactionService)
        {
            _walletService = walletService;
            //_userService = userService;
            vnPaySetting = options.Value;
            _transactionService = transactionService;
        }

        [HttpGet("get/wallets")]
        public async Task<ActionResult<List<Wallet>>> GetAllWallets()
        {
            var result = await _walletService.GetAllWallets();
            if (result is ActionResult<List<Wallet>> wallets)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(wallets);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/wallet/{walletID}")]
        public async Task<ActionResult<Wallet>> GetWallet(Guid walletID)
        {
            var result = await _walletService.GetWalletByWalletId(walletID);
            if (result is ActionResult<Wallet> wallet)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(wallet);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/wallet/user/{userID}")]
        public async Task<ActionResult<Wallet>> GetWalletByUserID(Guid userID)
        {
            var result = await _walletService.GetWalletByUserId(userID);
            if (result is ActionResult<Wallet> wallet)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                    if (statusCodeResult.StatusCode == 200) return Ok(wallet);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
