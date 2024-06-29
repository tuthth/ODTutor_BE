using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {   
        private readonly IMessageService _messageService;
        public MessageController(IMessageService message)
        {
            _messageService = message;
        }
        // Create a New Message
        [HttpPost("create")]
        public async Task <IActionResult> CreateMessage(string document, string collectionm, MessageRequest request)
        {
            await _messageService.CreateMessageAsync(collectionm, document, request);
            return Ok();
        }

        // Get a New Message
        [HttpGet("get")]
        public async Task<UserFireStoreReponse> GetMessage(string document, string collection)
        {
            var response = await _messageService.GetMessageAsync(collection, document);
            return  response;
        }

        // Block a User
        [HttpPost("block")]
        public async Task<IActionResult> BlockUser(string document, string collection, UserFirestoreRequest request)
        {
            await _messageService.BlockUserAsync(collection, document, request);
            return Ok("Block Thành công");
        }

        // Unblock a User
        [HttpPost("unblock")]
        public async Task<IActionResult> UnBlockUser(string document, string collection, UserFirestoreRequest request)
        {
            await _messageService.UnBlockUserAsync(collection, document, request);
            return Ok("Gỡ block Thành công");
        }
    }
}
