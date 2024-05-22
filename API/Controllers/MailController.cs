using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> SendMail([FromBody][Required][EmailAddress] string email)
        {
            try
            {
                var checkEmail = await _sendMailService.SendEmailTokenAsync(email.Trim());
                if (checkEmail is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 409) { return Conflict("Email đã được xác thực trước đó"); }
                    else if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created,"Gửi mã xác thực thành công"); }
                }
                if(checkEmail is Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                else { return BadRequest("Gửi mã xác thực thất bại"); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
