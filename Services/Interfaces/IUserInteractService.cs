using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserInteractService
    {
        Task<IActionResult> FollowUser(UserInteractRequest request);
        Task<IActionResult> UnfollowUser(UserInteractRequest request);
        Task<IActionResult> BlockUser(UserInteractRequest request);
        Task<IActionResult> UnblockUser(UserInteractRequest request);
        Task<ActionResult<List<UserBlock>>> GetAllUserBlocks();
        Task<ActionResult<List<UserBlock>>> GetAllBlockByCreateUserId(Guid id);
        Task<ActionResult<List<UserBlock>>> GetAllBlockByTargetUserId(Guid id);
        Task<ActionResult<List<UserFollow>>> GetAllUserFollows();
        Task<ActionResult<List<UserFollow>>> GetAllFollowsByCreateUserId(Guid id);
        Task<ActionResult<List<UserFollow>>> GetAllFollowsByTargetUserId(Guid id);
        Task<ActionResult<PageResults<UserBlock>>> GetAllUserBlocksPaging(PagingRequest request);
        Task<ActionResult<PageResults<UserBlock>>> GetAllBlockByCreateUserIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<PageResults<UserBlock>>> GetAllBlockByTargetUserIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<PageResults<UserFollow>>> GetAllUserFollowsPaging(PagingRequest request);
        Task<ActionResult<PageResults<UserFollow>>> GetAllFollowsByCreateUserIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<PageResults<UserFollow>>> GetAllFollowsByTargetUserIdPaging(Guid id, PagingRequest request);
    }
}
