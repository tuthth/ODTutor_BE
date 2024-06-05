using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // Login V2
        [HttpPost("login-v2")]
        public async Task<LoginAccountResponse> LoginV2([FromBody] LoginRequest user)
        {   
            LoginAccountResponse response = await _userService.LoginV2(user);
            return (response);
        }

        // Login By Admin
        /// <summary>
        /// Đăng nhập bằng tài khoản admin
        /// </summary>
        /// <param name="confirmEmailRequest"></param>
        /// <returns></returns>
        // Confirm Email
        [HttpPost("login-by-admin")]
        public async Task<LoginAccountResponse> LoginByAdmin([FromBody] LoginRequest user)
        {
            LoginAccountResponse response = await _userService.LoginByAdmin(user);
            return (response);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailRequest confirmEmailRequest)
        {
            try
            {
                var checkConfirm = await _userService.ConfirmOTP(confirmEmailRequest.Email, confirmEmailRequest.OTP);
                if (checkConfirm is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Tài khoản không tồn tại, hoặc không có yêu cầu gửi OTP trước đó"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Email đã được xác thực trước đó"); }
                    else if (statusCodeResult.StatusCode == 403) { return StatusCode(403, "Tài khoản đã bị khóa"); }
                    else if(statusCodeResult.StatusCode == 408) { return StatusCode(408, "Mã OTP đã hết hạn"); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok("Xác thực OTP thành công"); }
                }
                if (checkConfirm is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("remove-expire")]
        public async Task<IActionResult> RemoveExpiredOTP() {
            try
            {
                var checkRemove = await _userService.RemoveExpiredOTP();
                if (checkRemove is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 200) { return Ok("Xóa mã OTP hết hạn thành công"); }
                    else if(statusCodeResult.StatusCode == 404) { return NotFound("Không có mã OTP hết hạn"); }
                }
                if (checkRemove is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("is-banned/{id}")]
        public async Task<IActionResult> IsBanned(Guid id)
        {
            try
            {
                var checkBanned = await _userService.IsUserBanned(id);
                if (checkBanned is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Tài khoản không tồn tại"); }
                    else if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, "Tài khoản đã bị khóa"); }
                }
                if (checkBanned is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
