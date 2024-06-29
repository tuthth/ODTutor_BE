using AutoMapper;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class MessageService : BaseService, IMessageService
    {
        private readonly ICloudFireStoreService _cloudFireStoreService;
        public MessageService(ICloudFireStoreService cloudFireStoreService, ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
            _cloudFireStoreService = cloudFireStoreService;
        }

        // Create a new message
        public async Task CreateMessageAsync(string collection, string document, MessageRequest request)
        {
            await _cloudFireStoreService.SetAsync(collection, document, request);
        }

        // Get a message
        public async Task<UserFireStoreReponse> GetMessageAsync(string collection, string document)
        {
            Dictionary<string, object> data = await _cloudFireStoreService.GetAsync<Dictionary<string, object>>(collection, document);
            if (data != null)
            {
                UserFireStoreReponse response = new UserFireStoreReponse
                {

                    Avatar = data["avatar"].ToString(),
                    Email = data["name"].ToString()
                };
                return response;
            }
            else
            {
                return null;
            }
        }


        // Create a new User Or Update the  time they login
        public async Task CreateUserAsync(string collection, string document, UserFireStoreReponse request)
        {
            await _cloudFireStoreService.SetAsync(collection, document, request);
        }

        // Remove chat 
        public async Task<bool> RemoveChatAsync(string collection, string document)
        {
            return await _cloudFireStoreService.RemoveData(collection, document);
        }

        // Blocked User
        public async Task<bool> BlockUserAsync(string collection, string document, UserFirestoreRequest request)
        {
            Dictionary<string, object> data = await _cloudFireStoreService.GetAsync<Dictionary<string, object>>(collection, document);
            if (data != null)
            {
                List<string> blockedUsers;

                if (data.ContainsKey("blockedUser"))
                {
                    blockedUsers = ((List<object>)data["blockedUser"]).Cast<string>().ToList();
                }
                else
                {
                    blockedUsers = new List<string>();
                }

                if (!blockedUsers.Contains(request.UserId.ToString()))
                {
                    blockedUsers.Add(request.UserId.ToString());
                    data["blockedUser"] = blockedUsers;
                    await _cloudFireStoreService.SetAsync(collection, document, data);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        //------- Internal methods-------//



    }
}
