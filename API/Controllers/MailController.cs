using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly ISendMailService _sendMailService;

        public MailController(ISendMailService sendMailService)
        {
            _sendMailService = sendMailService;
        }

        [HttpPost("otp/email")]
        public async Task<IActionResult> SendMail([FromBody] SendOTPRequest sendOTPRequest)
        {
            var checkEmail = await _sendMailService.SendEmailTokenAsync(sendOTPRequest.Email.Trim());
            if (checkEmail is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 409) { return Conflict(new {Message = "Email đã xác thực trước đó" }); }
                    else if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created, new {Message = "Gửi mã xác thực thành công" }); }
                }
            }
            else if (checkEmail is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

    }
}
