using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        // Create Or Get a User From FireStore
        public async Task<ActionResult<UserInFireStore>> CreateOrGetUserAndUserChatsInFirestore (string userID)
        {   
            try
            {
                if (!Guid.TryParse(userID, out Guid userId))
                {
                    throw new CrudException(System.Net.HttpStatusCode.BadRequest, "Invalid User ID", "");
                }
                // Fetch User From the context
                User user = _context.Users.FirstOrDefault(x => x.Id == userId);
                UserInFireStore userFireStore = await _cloudFireStoreService.GetAsync<UserInFireStore>("users", userID);
                // Check if the user is in the FireStore
                if (userFireStore == null)
                {   
                    DateTime timeForFireStore = DateTime.UtcNow;
                    // Create a new User in the FireStore
                    UserInFireStore userFireStoreReponse = new UserInFireStore
                    {
                        avatar = user.ImageUrl,
                        blockedUser = new List<string>(),
                        LastLogin = timeForFireStore,
                        name = user.Name,
                        userId = userID.ToString()
                    };
                    await _cloudFireStoreService.SetAsync("users", userID, userFireStoreReponse);
                    var userchats = new Dictionary<string, Object>();
                    await _cloudFireStoreService.SetAsync("userchats", userID, userchats);
                    return userFireStoreReponse;
                }
                else
                {   
                    DateTime timeForFireStore = DateTime.UtcNow;
                    long unixTime = ((DateTimeOffset)timeForFireStore).ToUnixTimeSeconds();
                    userFireStore.LastLogin = timeForFireStore;
                    userFireStore.name = user.Name;
                    userFireStore.avatar = user.ImageUrl;
                    await _cloudFireStoreService.SetAsync("users", userID, userFireStore);
                    return userFireStore;
                }
            }
            catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw ex;
            }
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
                    Email = data["name"].ToString(),
                    LastLogin = DateTime.Parse(data["LastLogin"].ToString()),
                    UserId = Guid.Parse(data["userId"].ToString()),
                    Name = data["name"].ToString(),
                    blockedUser = ((List<object>)data["blockedUser"]).Cast<string>().Select(x => Guid.Parse(x)).ToList()
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

        // Unblocked User
        public async Task<bool> UnBlockUserAsync(string collection, string document, UserFirestoreRequest request)
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

                if (blockedUsers.Contains(request.UserId.ToString()))
                {
                    blockedUsers.Remove(request.UserId.ToString());
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
