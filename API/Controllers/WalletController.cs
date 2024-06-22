using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Models.Models.Views;
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
        //private readonly IUserService _userServic;
        private readonly VNPaySetting vnPaySetting;
        private readonly IMapper _mapper;

        public WalletController(IWalletService walletService, IOptions<VNPaySetting> options, ITransactionService transactionService, IMapper mapper)
        {
            _walletService = walletService;
            //_userService = userService;
            vnPaySetting = options.Value;
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpGet("get/all")]
        public async Task<ActionResult<List<WalletView>>> GetAllWallets()
        {
            var result = await _walletService.GetAllWallets();
            if (result is ActionResult<List<Wallet>> wallets && result.Value != null)
            {
                var walletViews = new List<WalletView>();
                foreach (var wallet in wallets.Value)
                {
                    var walletView = _mapper.Map<Wallet, WalletView>(wallet);
                    walletViews.Add(walletView);
                }
                return Ok(walletViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/{walletID}")]
        public async Task<ActionResult<Wallet>> GetWallet(Guid walletID)
        {
            var result = await _walletService.GetWalletByWalletId(walletID);
            if (result is ActionResult<Wallet> wallet && result.Value != null)
            {
                var walletView = _mapper.Map<Wallet, WalletView>(wallet.Value);
                return Ok(walletView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user/{userID}")]
        public async Task<ActionResult<Wallet>> GetWalletByUserID(Guid userID)
        {
            var result = await _walletService.GetWalletByUserId(userID);
            if (result is ActionResult<Wallet> wallet && result.Value != null)
            {
                var walletView = _mapper.Map<Wallet, WalletView>(wallet.Value);
                return Ok(walletView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/paging")]
        public async Task<ActionResult<PageResults<WalletView>>> GetAll(int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _walletService.GetAllWalletsPaging(request);
            if (result is ActionResult<PageResults<Wallet>> wallets && result.Value != null)
            {
                var walletViews = _mapper.Map<PageResults<Wallet>, PageResults<WalletView>>(wallets.Value);
                return Ok(walletViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy ví" }); }
                if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/last-transaction/{walletID}")]
        public async Task<ActionResult<WalletTransactionView>> GetLastTransaction(Guid walletID)
        {
            var result = await _walletService.GetLastTransaction(walletID);
            if (result is ActionResult<WalletTransaction> walletTransaction && result.Value != null)
            {
                var walletTransactionView = _mapper.Map<WalletTransaction, WalletTransactionView>(walletTransaction.Value);
                return Ok(walletTransactionView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy giao dịch" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
