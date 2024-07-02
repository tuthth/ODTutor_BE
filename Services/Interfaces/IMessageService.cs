using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMessageService
    {
        Task CreateMessageAsync(string collection, string document, MessageRequest messageRequest);
        Task<UserFireStoreReponse> GetMessageAsync(string collection, string document);
        Task<bool> BlockUserAsync(string collection, string document, UserFirestoreRequest request);
        Task<bool> UnBlockUserAsync(string collection, string document, UserFirestoreRequest request);
        Task<ActionResult<UserInFireStore>> CreateOrGetUserAndUserChatsInFirestore(string userID);
    }
}
