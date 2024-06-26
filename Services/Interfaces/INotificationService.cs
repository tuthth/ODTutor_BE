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
    public interface INotificationService
    {
        Task<IActionResult> CreateNotification(NotificationRequest request);
        Task<List<NotificationResponse>> GetNotificationByUserId(Guid userId);
    }
}
