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

        // Confirm Email
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailRequest confirmEmailRequest)
        {
            try
            {
                var checkConfirm = await _userService.ConfirmOTP(confirmEmailRequest.Email, confirmEmailRequest.OTP);
                if (checkConfirm is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Tài khoản không tồn tại, hoặc không có yêu cầu gửi OTP trước đó" }); }
                    if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Email đã được xác thực trước đó" }); }
                    if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Tài khoản đã bị khóa" }); }
                    if (statusCodeResult.StatusCode == 408) { return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Mã OTP đã hết hạn" }); }
                    if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Mã OTP không chính xác" }); }
                    if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Xác thực OTP thành công" }); }
                }
                if (checkConfirm is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(new {Message = ex.ToString()});
            }
        }
        [HttpPost("confirm-change-password")]
        public async Task<IActionResult> ConfirmChangePassword([FromBody] ChangePasswordRequest confirmEmailRequest)
        {
            try
            {
                var checkConfirm = await _userService.ConfirmOTPChangePassword(confirmEmailRequest.Email, confirmEmailRequest.OldPassword, confirmEmailRequest.NewPassword, confirmEmailRequest.ConfirmNewPassword ,confirmEmailRequest.OTP);
                if (checkConfirm is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Tài khoản không tồn tại, hoặc không có yêu cầu gửi OTP trước đó" }); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Email đã được xác thực trước đó"}); }
                    else if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Tài khoản đã bị khóa" }); }
                    else if (statusCodeResult.StatusCode == 408) { return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Mã OTP đã hết hạn"} ); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, new { Message = "Mật khẩu cũ không chính xác"} ); }
                    else if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Mật khẩu mới không khớp"});}
                    else if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Mã OTP không chính xác" }); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok(new {Message = "Xác thực OTP thành công" }); }
                }
                if (checkConfirm is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("confirm-forgot-password")]
        public async Task<IActionResult> ConfirmForgotPassword([FromBody] ForgetPasswordRequest confirmEmailRequest)
        {
            try
            {
                var checkConfirm = await _userService.ConfirmOTPForgotPassword(confirmEmailRequest.Email, confirmEmailRequest.NewPassword, confirmEmailRequest.ConfirmNewPassword, confirmEmailRequest.OTP);
                if (checkConfirm is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Tài khoản không tồn tại, hoặc không có yêu cầu gửi OTP trước đó" }); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Email đã được xác thực trước đó" }); }
                    else if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Tài khoản đã bị khóa" }); }
                    else if (statusCodeResult.StatusCode == 408) { return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Mã OTP đã hết hạn" }); }
                    else if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Mật khẩu mới không khớp" }); }
                    else if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Mã OTP không chính xác" }); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Xác thực OTP thành công" }); }
                }
                if (checkConfirm is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("remove-expire")]
        public async Task<IActionResult> RemoveExpiredOTP() {
            try
            {
                var checkRemove = await _userService.RemoveExpiredOTP();
                if (checkRemove is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 200) { return Ok(new {Message = "Xóa mã OTP hết hạn thành công" }); }
                    else if(statusCodeResult.StatusCode == 404) { return NotFound(new {Message = "Không có mã OTP hết hạn"}); }
                }
                if (checkRemove is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
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
                    if (statusCodeResult.StatusCode == 404) { return NotFound(new {Message = "Tài khoản không tồn tại"}); }
                    else if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new {Message = "Tài khoản đã bị khóa"}); }
                }
                if (checkBanned is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
                throw new Exception("Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
