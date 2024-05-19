using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {   
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService )
        {
            _accountService = accountService;
        }

        // Đăng kí tài khoản
        [HttpPost("register-account")]
        public async Task<ActionResult<AccountResponse>> registerAccount([FromBody] AccountRegisterRequest accountRegisterRequest)
        {
                var rs = await _accountService.createAccount(accountRegisterRequest);
                return Ok(rs);
        }

        // Tìm theo StudentID
        [HttpGet("get-user-information")]
        public async Task<ActionResult<AccountResponse>> getUserInfo([FromQuery] Guid userID)
        {
            var rs = await _accountService.GetStudentInformation(userID);
            return Ok(rs);
        }
    }
}
