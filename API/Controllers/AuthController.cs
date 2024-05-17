﻿using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, [FromQuery] int role)
        {
            try
            {
                var checkLogin = await _userService.Login(loginRequest, role);
                if (checkLogin is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Tài khoản không tồn tại"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Vui lòng đăng xuất ở thiết bị khác trước khi đăng nhập"); }
                    else if (statusCodeResult.StatusCode == 403) { return StatusCode(403, "Tài khoản đã bị khóa"); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok("Đăng nhập thành công"); }
                    else { return BadRequest("Đăng nhập thất bại"); }
                }
                else if (checkLogin is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
                else { return BadRequest("Đăng nhập thất bại"); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail(string email, string otp)
        {
            try
            {
                var checkConfirm = await _userService.ConfirmOTP(email, otp);
                if (checkConfirm is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Tài khoản không tồn tại, hoặc không có yêu cầu gửi OTP trước đó"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Email đã được xác thực trước đó"); }
                    else if (statusCodeResult.StatusCode == 403) { return StatusCode(403, "Tài khoản đã bị khóa"); }
                    else if(statusCodeResult.StatusCode == 408) { return StatusCode(408, "Mã OTP đã hết hạn"); }
                    else if (statusCodeResult.StatusCode == 200) { return Ok("Xác thực OTP thành công"); }
                    else { return BadRequest("Xác thực email thất bại"); }
                }
                else { return BadRequest("Xác thực email thất bại"); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
