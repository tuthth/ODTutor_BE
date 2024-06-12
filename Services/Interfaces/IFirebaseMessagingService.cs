using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IFirebaseMessagingService
    {
        void SendToDevices(List<string> tokens, FirebaseAdmin.Messaging.Notification notification, Dictionary<string,string> data);
    }
}
