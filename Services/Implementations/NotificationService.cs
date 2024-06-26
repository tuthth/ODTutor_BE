using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    public class NotificationService : BaseService, INotificationService
    {   
        private readonly IFirebaseRealtimeDatabaseService _firebaseRealtimeDatabaseService;
        public NotificationService(ODTutorContext context, IMapper mapper, IFirebaseRealtimeDatabaseService firebaseRealtimeDatabaseService) : base (context, mapper)
        {
            _firebaseRealtimeDatabaseService = firebaseRealtimeDatabaseService;
        }

        // Create a notification
        public async Task <IActionResult> CreateNotification(NotificationRequest request)
        {
            try
            {
                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    UserId = request.UserId,
                    CreatedAt = DateTime.Now,
                    Status = 0
                };
                await _firebaseRealtimeDatabaseService.SetAsync($"notifications/{request.UserId}/{notification.NotificationId}", notification);
                return new StatusCodeResult(201);
            } catch(CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(System.Net.HttpStatusCode.InternalServerError, ex.Message,"");
            }   
        }

    }
}
