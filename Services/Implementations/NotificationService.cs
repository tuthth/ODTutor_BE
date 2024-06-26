using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enumerables;
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
                    Status = (Int32)NotificationEnum.UnRead
                };
                await _firebaseRealtimeDatabaseService.SetAsync<Notification>($"notifications/{request.UserId}/{notification.NotificationId}", notification);
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                throw new CrudException(System.Net.HttpStatusCode.Created, "Tạo thông báo thành công", "");
            } catch(CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(System.Net.HttpStatusCode.InternalServerError, ex.Message,"");
            }   
        }


        // Get Notification By UserId
        public async Task<List<NotificationResponse>> GetNotificationByUserId(Guid userId)
        {
            try
            {
                var response = await _firebaseRealtimeDatabaseService.GetAsync<Dictionary<string, Notification>>($"notifications/{userId}");

                List<Notification> list = new List<Notification>();
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        list.Add(item.Value);
                    }
                }
                var notificationResponses = list.Select(notification => new NotificationResponse
                {
                    Title = notification.Title,
                    Content = notification.Content,
                    UserID = notification.UserId,
                    CreatedAt = notification.CreatedAt,
                }).ToList();
                return notificationResponses;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(System.Net.HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Set Notification Read, Unread
        public async Task<IActionResult> SetNotificationStatus(Guid userId, Guid notificationId)
        {
            try
            {
                var notification = await _firebaseRealtimeDatabaseService.GetAsync<Notification>($"notifications/{userId}/{notificationId}");
                if (notification == null)
                {
                    throw new CrudException(System.Net.HttpStatusCode.NotFound, "Không tìm thấy thông báo", "");
                }
                notification.Status = (Int32) NotificationEnum.Read;
                await _firebaseRealtimeDatabaseService.SetAsync<Notification>($"notifications/{userId}/{notificationId}", notification);
                Notification noti = await _context.Notifications.FindAsync(notificationId);
                noti.Status = (Int32)NotificationEnum.Read;
                await _context.SaveChangesAsync();
                throw new CrudException(System.Net.HttpStatusCode.OK, "Cập nhật trạng thái thông báo thành công", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(System.Net.HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }
    }
}
