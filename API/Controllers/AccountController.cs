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
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // Đăng kí tài khoản
        [HttpPost("register")]
        public async Task<IActionResult> registerAccount([FromBody] AccountRegisterRequest accountRegisterRequest)
        {
            var rs = await _accountService.createAccount(accountRegisterRequest);
            if (rs is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created, new { Message = "Tài khoản được tạo thành công"}); }
                else if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Tài khoản đã tồn tại" }); }
                else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Email đã đăng kí trước đó, vui lòng kiểm tra" }); }
            }
            if (rs is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }

        // Tìm theo StudentID
        [HttpGet("get/{userID}")]
        public async Task<ActionResult<AccountResponse>> getUserInfo(Guid userID)
        {

            var rs = await _accountService.GetStudentInformation(userID);
            if (rs is ActionResult<AccountResponse> accountResponse)
            {
                if (rs.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy tài khoản"); }
                }
                if ((IActionResult)rs.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                return Ok(accountResponse);
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update")]
        public async Task<IActionResult> updateUser(UpdateAccountRequest request)
        {
            var rs = await _accountService.updateUserAccount(request);
            if (rs is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy tài khoản" }); }
                else if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Cập nhật thất bại" }); }
                else if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật thành công" }); }
            }
            if (rs is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/all")]
        public async Task<ActionResult<List<UserAccountResponse>>> GetAll()
        {
            var rs = await _accountService.GetAllUser();
            if (rs is List<UserAccountResponse> userAccountResponses && rs.Count > 0)
            {
                var users = userAccountResponses;
                var userIn30Days = users.Where(x => x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30)).ToList();
                var userInPrevious30Days = users.Where(x => x.CreatedAt < DateTime.UtcNow.AddHours(7).AddDays(-30) && x.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-60)).ToList();
                var percentageChange = userInPrevious30Days.Count == 0 ? 100 : (userIn30Days.Count - userInPrevious30Days.Count) / userInPrevious30Days.Count * 100;
                return Ok(new { Users = userAccountResponses, Total = users.Count, UserIn30Days = userIn30Days.Count, PercentageChange = percentageChange });
            }
            if ((IActionResult)rs is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

    }
}
