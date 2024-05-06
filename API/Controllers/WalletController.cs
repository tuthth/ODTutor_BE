using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Enumerables;
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

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyWallet()
        {
            try
            {
                //var userId = _userService.GetUserId(HttpContext);
                var wallet = await _walletService.GetWallet(Guid.Empty);

                if (wallet is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                }
                else if (wallet is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
                return BadRequest("Lấy ví thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vnpay/callback")]
        public async Task<IActionResult> WalletVnpayCallBack()
        {
            string returnContent = string.Empty;

            try
            {
                var vnpayData = new Dictionary<string, string>();

                foreach (var key in Request.Query.Keys)
                {
                    var values = Request.Query[key];
                    if (values.Count > 0)
                    {
                        vnpayData[key] = values[0];
                    }
                }

                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (var entry in vnpayData)
                {
                    string key = entry.Key;
                    string value = entry.Value;

                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value);
                    }
                }

                Guid orderId = Guid.Parse(vnpay.GetResponseData("vnp_TxnRef"));
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = Request.Query["vnp_SecureHash"].ToString();

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnPaySetting.VnPay_HashSecret.ToString());


                var trans = await _transactionService.GetWalletTransactionById(orderId);
                if (trans == null) return StatusCode(StatusCodes.Status404NotFound, "Không tìm thấy giao dịch");

                if (checkSignature)
                {
                    if (trans != null)
                    {
                        if (trans.Amount == vnp_Amount)
                        {
                            if (trans.Status == (int)VNPayType.PENDING)
                            {
                                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                                {
                                    trans.Status = (int)VNPayType.APPROVE;
                                    trans.WalletNavigation.Amount += trans.Amount;
                                    returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";

                                }
                                else
                                {
                                    trans.Status = (int)VNPayType.REJECT;
                                    returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                                }

                                await _transactionService.UpdateWalletTransactionInfoInDatabase(trans);
                            }
                            else
                            {
                                returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                            }
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
                        }
                    }
                    else
                    {
                        returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    }
                }
                else
                {
                    returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                returnContent = "{\"RspCode\":\"99\",\"Message\":\"An error occurred\"}";
            }

            return Content(returnContent, "application/json");
        }
    }
}
