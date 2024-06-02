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
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    else if (statusCodeResult.StatusCode == 406) { return StatusCode(StatusCodes.Status406NotAcceptable, "Giao dịch không rõ trạng thái"); }
                    else if (statusCodeResult.StatusCode == 409) { return Conflict("Số dư tài khoản không đủ thực hiện giao dịch"); }
                    else if (statusCodeResult.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi hệ thống"); }
                    else if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created, "Gửi mã xác thực thành công"); }
                    else if (statusCodeResult.StatusCode == 204) { return NoContent(); }
                }
                if (actionResult is JsonResult okObjectResult)
                {
                    return Ok(okObjectResult.Value);
                }
            }
            else if (checkEmail is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
            throw new Exception("Lỗi không xác định");
        }

    }
}
