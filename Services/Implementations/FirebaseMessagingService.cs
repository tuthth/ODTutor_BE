using Models.Entities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;

namespace Services.Implementations
{
    public class FirebaseMessagingService : IFirebaseMessagingService
    {   
        private readonly static FirebaseMessagingService _instance = new FirebaseMessagingService();
        public void SendToDevices(List<string> tokens, FirebaseAdmin.Messaging.Notification notification, Dictionary<string, string> data)
        {
            var message = new MulticastMessage()
            {
                Tokens = tokens,
                Notification = notification,
                Data = data
            };
        }
    }
}
