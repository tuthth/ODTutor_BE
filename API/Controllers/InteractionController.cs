using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InteractionController : ControllerBase
    {
        private readonly IUserInteractService _userInteractService;
        private readonly IMapper _mapper;

        public InteractionController(IUserInteractService userInteractService, IMapper mapper)
        {
            _userInteractService = userInteractService;
            _mapper = mapper;
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
                    if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Bạn đã theo dõi tài khoản này" }); }
                    if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Theo dõi tài khoản thành công" });
                    if(statusCodeResult.StatusCode == 400) { return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Có tài khoản không tồn tại trong hệ thống, hoặc đã bị hệ thống đình chỉ" }); }
                }
                if (actionResult is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
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
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, new { Message = "Bỏ theo dõi tài khoản thành công" });
                    if (statusCodeResult.StatusCode == 400) { return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Có tài khoản không tồn tại trong hệ thống, hoặc đã bị hệ thống đình chỉ" }); }
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
                    if (statusCodeResult.StatusCode == 409) { return StatusCode(StatusCodes.Status409Conflict, new { Message = "Bạn đã chặn tài khoản này" }); }
                    if (statusCodeResult.StatusCode == 400) { return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Có tài khoản không tồn tại trong hệ thống, hoặc đã bị hệ thống đình chỉ" }); }
                    //if (statusCodeResult.StatusCode == 404) { return StatusCode(StatusCodes.Status404NotFound, new { Message = "Không tìm thấy tài khoản cần chặn" }); }
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
                    if (statusCodeResult.StatusCode == 200) return StatusCode(StatusCodes.Status200OK, new { Message = "Bỏ chặn tài khoản thành công" });
                    if (statusCodeResult.StatusCode == 400) { return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Có tài khoản không tồn tại trong hệ thống, hoặc đã bị hệ thống đình chỉ" }); }
                }
                if (actionResult is Exception exception) return StatusCode(500, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks")]
        public async Task<ActionResult<List<UserBlockView>>> GetAllUserBlocks()
        {
            var result = await _userInteractService.GetAllUserBlocks();
            if (result is ActionResult<List<UserBlock>> userBlocks && result.Value != null)
            {
                var userBlockViews = _mapper.Map<List<UserBlockView>>(userBlocks.Value);
                return Ok(userBlockViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks/paging")]
        public async Task<ActionResult<PageResults<UserBlockView>>> GetAllUserBlocksPaging([FromQuery] PagingRequest request)
        {
            var result = await _userInteractService.GetAllUserBlocksPaging(request);
            if (result is ActionResult<PageResults<UserBlock>> userBlocks && result.Value != null)
            {
                var userBlockViews = _mapper.Map<PageResults<UserBlockView>>(userBlocks.Value);
                return Ok(userBlockViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-block/create/{userBlockID}")]
        public async Task<ActionResult<List<UserBlockView>>> GetUserBlock(Guid userBlockID)
        {
            var result = await _userInteractService.GetAllBlockByCreateUserId(userBlockID);
            if (result is ActionResult<List<UserBlock>> userBlock && result.Value != null)
            {
                var userBlockViews = _mapper.Map<List<UserBlockView>>(userBlock.Value);
                return Ok(userBlockViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }

            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-block/create/{userBlockID}/paging")]
        public async Task<ActionResult<PageResults<UserBlockView>>> GetUserBlockPaging(Guid userBlockID, [FromQuery] PagingRequest request)
        {
            var result = await _userInteractService.GetAllBlockByCreateUserIdPaging(userBlockID, request);
            if (result is ActionResult<PageResults<UserBlock>> userBlock && result.Value != null)
            {
                var userBlockViews = _mapper.Map<PageResults<UserBlockView>>(userBlock.Value);
                return Ok(userBlockViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }

            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-blocks/target/{targetID}")]
        public async Task<ActionResult<List<UserBlockView>>> GetUserBlocksByTargetID(Guid targetID)
        {
            var result = await _userInteractService.GetAllBlockByTargetUserId(targetID);
            if (result is ActionResult<List<UserBlock>> userBlocks && result.Value != null)
            {
                var userBlockViews = _mapper.Map<List<UserBlockView>>(userBlocks.Value);
                return Ok(userBlockViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks/target/{targetID}/paging")]
        public async Task<ActionResult<PageResults<UserBlockView>>> GetUserBlocksByTargetIDPaging(Guid targetID, [FromQuery] PagingRequest request)
        {
            var result = await _userInteractService.GetAllBlockByTargetUserIdPaging(targetID, request);
            if (result is ActionResult<PageResults<UserBlock>> userBlocks && result.Value != null)
            {
                var userBlockViews = _mapper.Map<PageResults<UserBlockView>>(userBlocks.Value);
                return Ok(userBlockViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy khóa tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-follows")]
        public async Task<ActionResult<List<UserFollowView>>> GetAllUserFollows()
        {
            var result = await _userInteractService.GetAllUserFollows();
            if (result is ActionResult<List<UserFollow>> userFollows && result.Value != null)
            {
                var userFollowViews = _mapper.Map<List<UserFollowView>>(userFollows.Value);
                return Ok(userFollowViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follows/paging")]
        public async Task<ActionResult<PageResults<UserFollowView>>> GetAllUserFollowsPaging([FromQuery] PagingRequest request)
        {
            var result = await _userInteractService.GetAllUserFollowsPaging(request);
            if (result is ActionResult<PageResults<UserFollow>> userFollows && result.Value != null)
            {
                var userFollowViews = _mapper.Map<PageResults<UserFollowView>>(userFollows.Value);
                return Ok(userFollowViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-follow/create/{userFollowID}")]
        public async Task<ActionResult<List<UserFollowView>>> GetUserFollow(Guid userFollowID)
        {
            var result = await _userInteractService.GetAllFollowsByCreateUserId(userFollowID);
            if (result is ActionResult<List<UserFollow>> userFollow && result.Value != null)
            {
                var userFollowViews = _mapper.Map<List<UserFollowView>>(userFollow.Value);
                return Ok(userFollowViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follow/create/{userFollowID}/paging")]
        public async Task<ActionResult<PageResults<UserFollowView>>> GetUserFollowPaging(Guid userFollowID, [FromQuery] PagingRequest request)
        {
            var result = await _userInteractService.GetAllFollowsByCreateUserIdPaging(userFollowID, request);
            if (result is ActionResult<PageResults<UserFollow>> userFollow && result.Value != null)
            {
                var userFollowViews = _mapper.Map<PageResults<UserFollowView>>(userFollow.Value);
                return Ok(userFollowViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user-follows/target/{targetID}")]
        public async Task<ActionResult<List<UserFollowView>>> GetUserFollowsByTargetID(Guid targetID)
        {
            var result = await _userInteractService.GetAllFollowsByTargetUserId(targetID);
            if (result is ActionResult<List<UserFollow>> userFollows && result.Value != null)
            {
                var userFollowViews = _mapper.Map<List<UserFollowView>>(userFollows.Value);
                return Ok(userFollowViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy theo dõi tài khoản" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follows/target/{targetID}/paging")]
        public async Task<ActionResult<PageResults<UserFollowView>>> GetUserFollowsByTargetIDPaging(Guid targetID, [FromQuery] PagingRequest request)
        {
            var result = await _userInteractService.GetAllFollowsByTargetUserIdPaging(targetID, request);
            if (result is ActionResult<PageResults<UserFollow>> userFollows && result.Value != null)
            {
                var userFollowViews = _mapper.Map<PageResults<UserFollowView>>(userFollows.Value);
                return Ok(userFollowViews);
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
