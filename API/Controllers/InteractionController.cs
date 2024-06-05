using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    public class InteractionController : ControllerBase
    {
        private readonly IUserInteractService _userInteractService;
        public InteractionController(IUserInteractService userInteractService)
        {
            _userInteractService = userInteractService;
        }
        [HttpPost("follow")]
        public async Task<IActionResult> FollowUser(UserInteractRequest request)
        {
            var result = await _userInteractService.FollowUser(request);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 403) { return StatusCode(StatusCodes.Status403Forbidden, new { Message = "Không thể theo dõi tài khoản" }); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Theo dõi tài khoản thành công" });
                }
                if (actionResult is Exception exception) return StatusCode(500, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("unfollow")]
        public async Task<IActionResult> UnfollowUser(UserInteractRequest request)
        {
            var result = await _userInteractService.UnfollowUser(request);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return StatusCode(StatusCodes.Status404NotFound, new { Message = "Bạn chưa follow tài khoản này" }); }
                    if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Bỏ theo dõi tài khoản thành công" });
                }
                if (actionResult is Exception exception) return StatusCode(500, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("block")]
        public async Task<IActionResult> BlockUser(UserInteractRequest request)
        {
            var result = await _userInteractService.BlockUser(request);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Chặn tài khoản thành công" });
                }
                if (actionResult is Exception exception) return StatusCode(500, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("unblock")]
        public async Task<IActionResult> UnblockUser(UserInteractRequest request)
        {
            var result = await _userInteractService.UnblockUser(request);
            if (result is IActionResult actionResult)
            {
                if (actionResult is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return StatusCode(StatusCodes.Status404NotFound, new { Message = "Không tìm thấy tài khoản bị bạn chặn" }); }
                    if (statusCodeResult.StatusCode == 204) return StatusCode(StatusCodes.Status204NoContent, new { Message = "Bỏ chặn tài khoản thành công" });
                }
                if (actionResult is Exception exception) return StatusCode(500, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks")]
        public async Task<ActionResult<List<UserBlock>>> GetAllUserBlocks()
        {
            var result = await _userInteractService.GetAllUserBlocks();
            if (result is ActionResult<List<UserBlock>> userBlocks)
            {
                return Ok(userBlocks.Value);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-block/create/{userBlockID}")]
        public async Task<ActionResult<List<UserBlock>>> GetUserBlock(Guid userBlockID)
        {
            var result = await _userInteractService.GetAllBlockByCreateUserId(userBlockID);
            if (result is ActionResult<List<UserBlock>> userBlock)
            {
                return Ok(userBlock.Value);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }

            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-blocks/target/{targetID}")]
        public async Task<ActionResult<List<UserBlock>>> GetUserBlocksByTargetID(Guid targetID)
        {
            var result = await _userInteractService.GetAllBlockByTargetUserId(targetID);
            if (result is ActionResult<List<UserBlock>> userBlocks)
            {
                return Ok(userBlocks.Value);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-follows")]
        public async Task<ActionResult<List<UserFollow>>> GetAllUserFollows()
        {
            var result = await _userInteractService.GetAllUserFollows();
            if (result is ActionResult<List<UserFollow>> userFollows)
            {
                return Ok(userFollows.Value);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-follow/create/{userFollowID}")]
        public async Task<ActionResult<List<UserFollow>>> GetUserFollow(Guid userFollowID)
        {
            var result = await _userInteractService.GetAllFollowsByCreateUserId(userFollowID);
            if (result is ActionResult<List<UserFollow>> userFollow)
            {
                return Ok(userFollow.Value);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-follows/target/{targetID}")]
        public async Task<ActionResult<List<UserFollow>>> GetUserFollowsByTargetID(Guid targetID)
        {
            var result = await _userInteractService.GetAllFollowsByTargetUserId(targetID);
            if (result is ActionResult<List<UserFollow>> userFollows)
            {
                return Ok(userFollows.Value);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

    }
}
