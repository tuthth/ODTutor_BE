using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
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
    }
}
