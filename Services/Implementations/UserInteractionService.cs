using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Models.Requests;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class UserInteractionService : BaseService, IUserInteractService
    {
        public UserInteractionService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> FollowUser(UserInteractRequest request)
        {
            var isBlocked = _context.UserBlocks.Any(x => x.CreateUserId == request.CreateUserId && x.TargetUserId == request.TargetUserId);
            if (isBlocked)
            {
                return new StatusCodeResult(403);
            }
            var userFollow = _mapper.Map<UserFollow>(request);
            userFollow.CreatedAt = DateTime.UtcNow;
            _context.UserFollows.Add(userFollow);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UnfollowUser(UserInteractRequest request)
        {
            var userFollow = _context.UserFollows.FirstOrDefault(x => x.CreateUserId == request.CreateUserId && x.TargetUserId == request.TargetUserId);
            if (userFollow == null)
            {
                return new StatusCodeResult(404);
            }
            _context.UserFollows.Remove(userFollow);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(204);
        }
        public async Task<IActionResult> BlockUser(UserInteractRequest request)
        {
            var isFollowed = await _context.UserFollows.FirstOrDefaultAsync(x => x.CreateUserId == request.CreateUserId && x.TargetUserId == request.TargetUserId);
            if (isFollowed!=null)
            {
                _context.UserFollows.Remove(isFollowed);
                await _context.SaveChangesAsync();
            }
            var userBlock = _mapper.Map<UserBlock>(request);
            userBlock.CreatedAt = DateTime.UtcNow;
            _context.UserBlocks.Add(userBlock);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UnblockUser(UserInteractRequest request)
        {
            var userBlock = _context.UserBlocks.FirstOrDefault(x => x.CreateUserId == request.CreateUserId && x.TargetUserId == request.TargetUserId);
            if (userBlock == null)
            {
                return new StatusCodeResult(404);
            }
            _context.UserBlocks.Remove(userBlock);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(204);
        }
        public async Task<ActionResult<List<UserBlock>>> GetAllUserBlocks()
        {
            try
            {
                var userBlocks = await _context.UserBlocks.ToListAsync();
                if (userBlocks == null)
                {
                    return new StatusCodeResult(404);
                }
                return userBlocks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserBlock>>> GetAllBlockByCreateUserId(Guid id)
        {
            try
            {
                var userBlocks = await _context.UserBlocks.Where(c => c.CreateUserId == id).ToListAsync();
                if (userBlocks == null)
                {
                    return new StatusCodeResult(404);
                }
                return userBlocks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserBlock>>> GetAllBlockByTargetUserId(Guid id)
        {
            try
            {
                var userBlocks = await _context.UserBlocks.Where(c => c.TargetUserId == id).ToListAsync();
                if (userBlocks == null)
                {
                    return new StatusCodeResult(404);
                }
                return userBlocks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserFollow>>> GetAllUserFollows()
        {
            try
            {
                var userFollows = await _context.UserFollows.ToListAsync();
                if (userFollows == null)
                {
                    return new StatusCodeResult(404);
                }
                return userFollows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserFollow>>> GetAllFollowsByCreateUserId(Guid id)
        {
            try
            {
                var userFollows = await _context.UserFollows.Where(c => c.CreateUserId == id).ToListAsync();
                if (userFollows == null)
                {
                    return new StatusCodeResult(404);
                }
                return userFollows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserFollow>>> GetAllFollowsByTargetUserId(Guid id)
        {
            try
            {
                var userFollows = await _context.UserFollows.Where(c => c.TargetUserId == id).ToListAsync();
                if (userFollows == null)
                {
                    return new StatusCodeResult(404);
                }
                return userFollows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
