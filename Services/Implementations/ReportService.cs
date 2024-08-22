using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
using Models.Migrations;
using Models.Models.Emails;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notification = Models.Entities.Notification;


namespace Services.Implementations
{
    public class ReportService : BaseService, IReportService
    {
        private readonly IFirebaseRealtimeDatabaseService _firebaseRealtimeDatabaseService;
        public ReportService(ODTutorContext context, IMapper mapper, IFirebaseRealtimeDatabaseService firebaseRealtimeDatabaseService) : base(context, mapper)
        {
            _firebaseRealtimeDatabaseService = firebaseRealtimeDatabaseService;
        }
        public async Task<IActionResult> CreateReport(ReportRequest reportRequest)
        {
            if (reportRequest == null) return new StatusCodeResult(404);
            var report = _mapper.Map<Report>(reportRequest);
            var sender = _context.Users.FirstOrDefault(x => x.Id == reportRequest.SenderUserId);
            var target = _context.Users.FirstOrDefault(x => x.Id == reportRequest.TargetId);
            report.CreatedAt = DateTime.UtcNow.AddHours(7);
            report.UpdatedAt = DateTime.UtcNow.AddHours(7);
            report.ReportId = Guid.NewGuid();
            report.Type = (Int32)ReportStatusEnum.User;
            // Check have image or not 
            if (reportRequest.ImageURLs != null)
            {
                foreach (var item in reportRequest.ImageURLs)
                {
                    var reportImage = new ReportImages()
                    {
                        ReportImageId = Guid.NewGuid(),
                        ReportId = report.ReportId,
                        ImageURL = item
                    };
                    report.ReportImages.Add(reportImage);
                }
            }
            report.Status = (Int32)ReportEnum.Processing;

            _context.Reports.Add(report);
            var notification1 = new NotificationDTO()
            {
                NotificationId = Guid.NewGuid(),
                UserId = target.Id,
                Title = "Bạn đã nhận được một report",
                Content = "Bạn đã nhận được report từ người dùng khác. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId,
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification1x = _mapper.Map<Notification>(notification1);
            var notification2 = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                UserId = sender.Id,
                Title = "Report của bạn đã được gửi",
                Content = "Bạn đã gửi report thành công. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId,
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification2x = _mapper.Map<Notification>(notification2);
            _context.Notifications.Add(notification1x);
            _context.Notifications.Add(notification2x);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = sender.Email,
                Subject = "[ODTutor] Report gửi thành công",
                Body = "Bạn đã gửi report thành công. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId
            });
            await _appExtension.SendMail(new MailContent()
            {
                To = target.Email,
                Subject = "[ODTutor] Bạn đã nhận được report",
                Body = "Bạn đã nhận được report từ người dùng khác. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId
            });
            return new StatusCodeResult(201);
        }

        // Create report is Booking report
        public async Task<IActionResult> CreateReportBooking(ReportRequest reportRequest)
        {
            if (reportRequest == null) return new StatusCodeResult(404);
            var report = _mapper.Map<Report>(reportRequest);
            var sender = _context.Users.FirstOrDefault(x => x.Id == reportRequest.SenderUserId);
            var target = _context.Bookings
                .Include(x => x.TutorNavigation)
                .Include(x => x.TutorNavigation.UserNavigation)
                .FirstOrDefault(x => x.BookingId == reportRequest.TargetId);
            target.Status = (Int32)BookingEnum.Reported;
            var tutor = _context.Users.FirstOrDefault(x => x.Id == target.TutorNavigation.UserNavigation.Id);
            report.CreatedAt = DateTime.UtcNow.AddHours(7);
            report.UpdatedAt = DateTime.UtcNow.AddHours(7);
            report.ReportId = Guid.NewGuid();
            report.Type = (Int32)ReportStatusEnum.Booking;
            // Check have image or not 
            List<ReportImages> images = new List<ReportImages>();
            if (reportRequest.ImageURLs != null)
            {
                foreach (var item in reportRequest.ImageURLs)
                {
                    var reportImage = new ReportImages()
                    {
                        ReportImageId = Guid.NewGuid(),
                        ReportId = report.ReportId,
                        ImageURL = item
                    };
                    images.Add(reportImage);
                }
            }
            report.Status = (Int32)ReportEnum.Processing;
            _context.ReportImages.AddRange(images);
            _context.Reports.Add(report);
            var notification1 = new NotificationDTO()
            {
                NotificationId = Guid.NewGuid(),
                UserId = tutor.Id,
                Title = "Buổi học của bạn đã bị báo cáo ",
                Content = "Bạn đã nhận được report từ người dùng khác. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId,
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification1x = _mapper.Map<Notification>(notification1);
            var notification2 = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                UserId = sender.Id,
                Title = "Report của bạn đã được gửi",
                Content = "Bạn đã gửi report thành công. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId,
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification2x = _mapper.Map<Notification>(notification2);
            _context.Notifications.Add(notification1x);
            _context.Notifications.Add(notification2x);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = sender.Email,
                Subject = "[ODTutor] Report gửi thành công",
                Body = "Bạn đã gửi report thành công. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId
            });
            await _appExtension.SendMail(new MailContent()
            {
                To = tutor.Email,
                Subject = "[ODTutor] Bạn đã nhận được report",
                Body = "Bạn đã nhận được report từ người dùng khác. Vui lòng chờ quyết định của hệ thống. Chúng tôi sẽ phản hồi lại trong thời gian sớm nhất. \nReport Id: " + report.ReportId
            });
            return new StatusCodeResult(201);
        }
        // Action report to ban account for reportBooking 
        public async Task<IActionResult> MakeActionReportBooking(ReportAction action)
        {
            var report = _context.Reports.FirstOrDefault(x => x.ReportId == action.ReportId);
            if (report == null) return new StatusCodeResult(404);
            var sender = _context.Users.FirstOrDefault(x => x.Id == report.SenderUserId);
            var target = _context.Bookings
                .Include(x => x.TutorNavigation)
                .Include(x => x.TutorNavigation.UserNavigation)
                .FirstOrDefault(x => x.BookingId == report.TargetId);
            target.Status = (Int32)BookingEnum.Reported;
            var tutor = _context.Users.FirstOrDefault(x => x.Id == target.TutorNavigation.UserNavigation.Id);
            if (report.Status != (Int32)ReportEnum.Finished) return new StatusCodeResult(409);
            DateTime finishedTime = DateTime.UtcNow.AddHours(7);
            if (action.Status == (Int32)ReportActionEnum.SevenDays)
            {
                finishedTime = finishedTime.AddDays(7);
                tutor.Status = 1;
                tutor.Banned = true;
                tutor.BanExpiredAt = finishedTime;
            }
            else if (action.Status == (Int32)ReportActionEnum.ThirtyDays)
            {
                finishedTime = finishedTime.AddDays(30);
                tutor.Status = 1;
                tutor.Banned = true;
                tutor.BanExpiredAt = finishedTime;
            }
            else if (action.Status == (Int32)ReportActionEnum.NinetyDays)
            {
                finishedTime = finishedTime.AddDays(90);
                tutor.Status = 1;
                tutor.Banned = true;
                tutor.BanExpiredAt = finishedTime;
            }
            else if (action.Status == (Int32)ReportActionEnum.Lifetime)
            {
                finishedTime = finishedTime.AddYears(30);
                tutor.Status = 1;
                tutor.Banned = true;
                tutor.BanExpiredAt = finishedTime;
            }
            await _appExtension.SendMail(new MailContent()
            {
                To = sender.Email,
                Subject = "[ODTutor] Report được chấp thuận",
                Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nN"
            });
            await _appExtension.SendMail(new MailContent()
            {
                To = tutor.Email,
                Subject = "[ODTutor] Tài khoản bị đình chỉ",
                Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nN"
            });
            var notification1 = new NotificationDTO()
            {
                NotificationId = Guid.NewGuid(),
                UserId = tutor.Id,
                Title = "Tài khoản của bạn đã bị đình chỉ",
                Content = "Hệ thống đã cấm tài khoản của bạn. Vui lòng đến mục Báo cáo để xem chi tiết",
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification1x = _mapper.Map<Notification>(notification1);
            var notification2 = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                UserId = sender.Id,
                Title = "Report của bạn đã được chấp thuận",
                Content = "Hệ thống đã cấm tài khoản của người dùng. Vui lòng đến mục Báo cáo để xem chi tiết",
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification2x = _mapper.Map<Notification>(notification2);
            _context.Notifications.Add(notification1x);
            _context.Notifications.Add(notification2x);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
            // Create TutorAction for moderator 
            var tutorAction = new TutorAction()
            {
                TutorActionId = Guid.NewGuid(),
                TutorId = tutor.Id,
                ModeratorId = Guid.Parse("3E4B355D-3D60-4A2A-2ADD-08DC93FF561F"),
                CreateAt = DateTime.UtcNow.AddHours(7),
                ReponseDate = DateTime.UtcNow.AddHours(7),
                Description = " Xử lý hành vi vi phạm của người dùng",
                ActionType = (Int32)TutorActionTypeEnum.ReportBooking,
                Status = (Int32)TutorActionEnum.Accept
            };
            _context.TutorActions.Add(tutorAction);
            _context.Users.Update(tutor);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> UpdateReport(UpdateReportRequest updateReportRequest)
        {
            if (updateReportRequest == null) return new StatusCodeResult(404);
            var report = _context.Reports.FirstOrDefault(x => x.ReportId == updateReportRequest.ReportId);
            var sender = _context.Users.FirstOrDefault(x => x.Id == updateReportRequest.SenderUserId);
            var target = _context.Users.FirstOrDefault(x => x.Id == updateReportRequest.TargetId);
            if (report == null) return new StatusCodeResult(404);
            if (report.Status != (Int32)ReportEnum.Processing) return new StatusCodeResult(409);
            if (updateReportRequest.Status != (Int32)ReportEnum.Finished && updateReportRequest.Status != (Int32)ReportEnum.Cancelled) return new StatusCodeResult(406);
            report.Status = updateReportRequest.Status;
            report.UpdatedAt = updateReportRequest.UpdatedAt;
            foreach (var item in updateReportRequest.ImageURLs)
            {
                var reportImage = new ReportImages()
                {
                    ReportImageId = Guid.NewGuid(),
                    ReportId = report.ReportId,
                    ImageURL = item
                };
                report.ReportImages.Add(reportImage);
            }
            if (updateReportRequest.Status == (Int32)ReportEnum.Cancelled)
            {
                await _appExtension.SendMail(new MailContent()
                {
                    To = sender.Email,
                    Subject = "[ODTutor] Report bị từ chối",
                    Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nNội dung: " + report.Content + "\nLý do: " + updateReportRequest.Reason
                });
                await _appExtension.SendMail(new MailContent()
                {
                    To = target.Email,
                    Subject = "[ODTutor] Report đến bạn bị từ chối",
                    Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nNội dung: " + report.Content + "\nLý do: " + updateReportRequest.Reason
                });
                var notification1 = new NotificationDTO()
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = target.Id,
                    Title = "Report đến bạn bị từ chối",
                    Content = "Hệ thống đã hủy report đến bạn. Vui lòng đến mục Báo cáo để xem chi tiết",
                    CreatedAt = report.CreatedAt,
                    Status = 0
                };
                Notification notification1x = _mapper.Map<Notification>(notification1);
                var notification2 = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = sender.Id,
                    Title = "Report của bạn đã bị từ chối",
                    Content = "Hệ thống đã hủy report của bạn. Vui lòng đến mục Báo cáo để xem chi tiết",
                    CreatedAt = report.CreatedAt,
                    Status = 0
                };
                Notification notification2x = _mapper.Map<Notification>(notification2);
                _context.Notifications.Add(notification1x);
                _context.Notifications.Add(notification2x);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
            }
            if (updateReportRequest.Status == (Int32)ReportEnum.Finished)
            {
                await _appExtension.SendMail(new MailContent()
                {
                    To = sender.Email,
                    Subject = "[ODTutor] Report được chấp thuận",
                    Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nNội dung: " + report.Content + "\nLý do: " + updateReportRequest.Reason + "\nChúng tôi sẽ thông báo kết quả trong thời gian sớm nhất."
                });
                await _appExtension.SendMail(new MailContent()
                {
                    To = target.Email,
                    Subject = "[ODTutor] Report đến bạn được chấp thuận",
                    Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nNội dung: " + report.Content + "\nLý do: " + updateReportRequest.Reason + "\nChúng tôi sẽ thông báo kết quả trong thời gian sớm nhất."
                });
                var notification1 = new NotificationDTO()
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = target.Id,
                    Title = "Report đến bạn được chấp thuận",
                    Content = "Hệ thống đã chấp nhận report đến bạn. Vui lòng đến mục Báo cáo để xem chi tiết",
                    CreatedAt = report.CreatedAt,
                    Status = 0
                };
                Notification notification1x = _mapper.Map<Notification>(notification1);
                var notification2 = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = sender.Id,
                    Title = "Report của bạn đã được chấp thuận",
                    Content = "Hệ thống đã chấp thuận report của bạn. Vui lòng đến mục Báo cáo để xem chi tiết",
                    CreatedAt = report.CreatedAt,
                    Status = 0
                };
                Notification notification2x = _mapper.Map<Notification>(notification2);
                _context.Notifications.Add(notification1x);
                _context.Notifications.Add(notification2x);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
            }
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> MakeActionReportWithUser(ReportAction action)
        {
            var report = _context.Reports.FirstOrDefault(x => x.ReportId == action.ReportId);
            if (report == null) return new StatusCodeResult(404);
            var sender = _context.Users.FirstOrDefault(x => x.Id == report.SenderUserId);
            var target = _context.Users.FirstOrDefault(x => x.Id == report.TargetId);

            if (report.Status != (Int32)ReportEnum.Finished) return new StatusCodeResult(409);
            DateTime finishedTime = DateTime.UtcNow.AddHours(7);
            if (action.Status == (Int32)ReportActionEnum.SevenDays)
            {
                finishedTime = finishedTime.AddDays(7);
                target.Status = 1;
                target.Banned = true;
                target.BanExpiredAt = finishedTime;
            }
            else if (action.Status == (Int32)ReportActionEnum.ThirtyDays)
            {
                finishedTime = finishedTime.AddDays(30);
                target.Status = 1;
                target.Banned = true;
                target.BanExpiredAt = finishedTime;
            }
            else if (action.Status == (Int32)ReportActionEnum.NinetyDays)
            {
                finishedTime = finishedTime.AddDays(90);
                target.Status = 1;
                target.Banned = true;
                target.BanExpiredAt = finishedTime;
            }
            else if (action.Status == (Int32)ReportActionEnum.Lifetime)
            {
                finishedTime = finishedTime.AddYears(30);
                target.Status = 1;
                target.Banned = true;
                target.BanExpiredAt = finishedTime;
            }
            await _appExtension.SendMail(new MailContent()
            {
                To = sender.Email,
                Subject = "[ODTutor] Report được chấp thuận",
                Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nNội dung: " + report.Content + "\nThời hạn cấm: " + finishedTime + " GMT +0." + "\nChúng tôi xin lỗi vì đã mang đến cho bạn trải nghiệm không tốt ở hệ thống."
            });
            await _appExtension.SendMail(new MailContent()
            {
                To = target.Email,
                Subject = "[ODTutor] Tài khoản bị đình chỉ",
                Body = "Hệ thống đã hoàn tất việc kiểm tra hành vi người dùng. \nReport Id: " + report.ReportId + "\nTrạng thái: " + (ReportEnum)report.Status + "\nNội dung: " + report.Content + "\nThời hạn cấm: " + finishedTime + " GMT +0." + "\nĐể khiếu nại, vui lòng phản hồi lại email này. \nXin cảm ơn"
            });
            var notification1 = new NotificationDTO()
            {
                NotificationId = Guid.NewGuid(),
                UserId = target.Id,
                Title = "Tài khoản của bạn đã bị đình chỉ",
                Content = "Hệ thống đã cấm tài khoản của bạn. Vui lòng đến mục Báo cáo để xem chi tiết",
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification1x = _mapper.Map<Notification>(notification1);
            var notification2 = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                UserId = sender.Id,
                Title = "Report của bạn đã được chấp thuận",
                Content = "Hệ thống đã cấm tài khoản của người dùng. Vui lòng đến mục Báo cáo để xem chi tiết",
                CreatedAt = report.CreatedAt,
                Status = 0
            };
            Notification notification2x = _mapper.Map<Notification>(notification2);
            _context.Notifications.Add(notification1x);
            _context.Notifications.Add(notification2x);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
            _context.Users.Update(target);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<ActionResult<List<Report>>> GetAllReports()
        {
            try
            {
                var reports = await _context.Reports.ToListAsync();
                if (reports == null)
                {
                    return new StatusCodeResult(404);
                }
                return reports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Report>> GetReport(Guid id)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(c => c.ReportId == id);
                if (report == null)
                {
                    return new StatusCodeResult(404);
                }
                return report;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Report>>> GetReportsByUserId(Guid id)
        {
            try
            {
                var reports = await _context.Reports.Where(c => c.TargetId == id).ToListAsync();
                if (reports == null)
                {
                    return new StatusCodeResult(404);
                }
                return reports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Report>>> GetReportsByReporterId(Guid id)
        {
            try
            {
                var reports = await _context.Reports.Where(c => c.SenderUserId == id).ToListAsync();
                if (reports == null)
                {
                    return new StatusCodeResult(404);
                }
                return reports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Report Booking By Status and Paging 
        public async Task<PageResults<ReportResponse>> GetAllReportBookingReport(PagingRequest request)
        {
            try
            {
                var reports = _context.Reports
               .Include(x => x.ReportImages)
               .Include(x => x.UserNavigation)
               .Where(x => x.Type == (Int32)ReportStatusEnum.Booking)
               .Where(x => x.Status == (Int32)ReportEnum.Processing)
               .OrderByDescending(x => x.CreatedAt)
               .ToList();
                List<ReportResponse> reportResponses = new List<ReportResponse>();
                foreach (var item in reports)
                {
                    // Find Tutor of Report 
                    var tutor = _context.Bookings
                        .Include(x => x.TutorNavigation)
                        .Include(x => x.TutorNavigation.UserNavigation)
                        .FirstOrDefault(x => x.BookingId == item.TargetId);
                    ReportResponse reportResponse = new ReportResponse()
                    {
                        ReportId = item.ReportId,
                        BookingId = item.TargetId,
                        SenderId = item.SenderUserId,
                        SenderName = item.UserNavigation.Name,
                        SenderAvatar = item.UserNavigation.ImageUrl,
                        TutorName = tutor.TutorNavigation.UserNavigation.Name,
                        TutorAvatar = tutor.TutorNavigation.UserNavigation.ImageUrl,
                        Content = item.Content,
                        CreatedAt = item.CreatedAt,
                        Status = item.Status,
                        Type = item.Type,
                        ReportImages = item.ReportImages.Select(x => x.ImageURL).ToList()
                    };
                    reportResponses.Add(reportResponse);
                }
                var pagingBookingReport = PagingHelper<ReportResponse>.Paging(reportResponses, request.Page, request.PageSize);
                return pagingBookingReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Response Detail by Report Id 
        public async Task<ActionResult<ReportDetailResponse>> GetReportDetailByReportId(Guid reportId)
        {
            try
            {
                var reports = _context.Reports
               .Include(x => x.ReportImages)
               .Include(x => x.UserNavigation)
               .FirstOrDefault(x => x.ReportId == reportId);

                var booking = _context.Bookings
                       .Include(x => x.TutorNavigation)
                       .Include(x => x.TutorNavigation.UserNavigation)
                       .FirstOrDefault(x => x.BookingId == reports.TargetId);
                ReportDetailResponse reportDetailResponse = new ReportDetailResponse();
                reportDetailResponse.ReportId = reports.ReportId;
                reportDetailResponse.BookingId = reports.TargetId;
                reportDetailResponse.SenderName = reports.UserNavigation.Name;
                reportDetailResponse.SenderAvatar = reports.UserNavigation.ImageUrl;
                reportDetailResponse.TutorName = booking.TutorNavigation.UserNavigation.Name;
                reportDetailResponse.TutorAvatar = booking.TutorNavigation.UserNavigation.ImageUrl;
                reportDetailResponse.CreatedAt = reports.CreatedAt;
                reportDetailResponse.Content = reports.Content;
                reportDetailResponse.Status = reports.Status;
                reportDetailResponse.Type = reports.Type;
                reportDetailResponse.Images = reports.ReportImages.Select(x => x.ImageURL).ToList();
                reportDetailResponse.BookingCreateAt = booking.CreatedAt;
                reportDetailResponse.BookingContent = booking.BookingContent;
                reportDetailResponse.BookingMessage = booking.Message;
                reportDetailResponse.TotalPrice = booking.TotalPrice;
                reportDetailResponse.StudyTime = booking.StudyTime;
                reportDetailResponse.RescheduledTime = booking.RescheduledTime;
                reportDetailResponse.IsRescheduled = booking.IsRescheduled;
                reportDetailResponse.Description = booking.Description;
                reportDetailResponse.GoogleMeetUrl = booking.GoogleMeetUrl;
                reportDetailResponse.IsRated = booking.isRated;
                // Check number of report of tutor
                var numberOfTutorReport = _context.Reports
                    .Where(x => x.TargetId == booking.TutorId)
                    .Where(x => x.Status == (Int32)ReportEnum.Finished)
                    .Count();
                return reportDetailResponse;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Handle Report Booking 
        public async Task<IActionResult> HandleReportOfTutor (Guid ReportId, Guid ApprovalId)
        {
            
            var report = _context.Reports.FirstOrDefault(x => x.ReportId == ReportId);
            if (report == null) return new StatusCodeResult(404);
            var bookingg = _context.Bookings
                .Include(x => x.TutorNavigation)
                .Include(x => x.TutorNavigation.UserNavigation)
                .FirstOrDefault(x => x.BookingId == report.TargetId);
            // Check Number of Report of Tutor
            var numberOfTutorReport = _context.Reports
                .Where(x => x.TargetId == bookingg.TutorId)
                .Where(x => x.Status == (Int32)ReportEnum.Finished)
                .Count();
            // Switchcase for handle report
            switch (numberOfTutorReport)
            {
                case 0:
                    {
                        // Refund money for student
                        // Get BoookingTransaction From BookingId 
                        var bookingTransaction = await _context.BookingTransactions.FirstOrDefaultAsync(b => b.BookingId == bookingg.BookingId);
                        var walletTransactionId = bookingTransaction.BookingTransactionId;
                        var books = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingg.BookingId);
                        var tutor = await _context.Tutors
                            .Include(t => t.UserNavigation)
                            .FirstOrDefaultAsync(t => t.TutorId == books.TutorId);
                        // Lấy thông tin giao dịch ví từ database
                        var wallet = await _context.WalletTransactions.FirstOrDefaultAsync(w => w.WalletTransactionId == walletTransactionId);
                        if (wallet == null)
                        {
                            return new StatusCodeResult(404);
                        }

                        // Lấy thông tin booking tương ứng
                        var booking = await _context.BookingTransactions
                            .Include(b => b.BookingNavigation)
                            .FirstOrDefaultAsync(b => b.BookingTransactionId == walletTransactionId);
                        if (booking == null)
                        {
                            return new StatusCodeResult(404);
                        }

                        // Xác định người gửi và người nhận
                        var sender = await _context.Users.Include(u => u.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == wallet.SenderWalletId);
                        var receiver = await _context.Users.Include(u => u.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == wallet.ReceiverWalletId);

                        // Cập nhật trạng thái giao dịch ví và booking
                        // Xử lý hoàn tiền
                        wallet.Status = (int)VNPayType.CANCELLED;
                        wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                        wallet.SenderWalletNavigation.AvalaibleAmount += booking.Amount;
                        wallet.SenderWalletNavigation.Amount += booking.Amount;
                        wallet.SenderWalletNavigation.PendingAmount += booking.Amount;

                        wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                        wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;
                        booking.BookingNavigation.Status = (int)BookingEnum.Cancelled;

                        booking.Status = (int)VNPayType.CANCELLED;
                        _context.WalletTransactions.Update(wallet);
                        _context.BookingTransactions.Update(booking);

                        var book = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);
                        if (book != null)
                        {
                            book.Status = (int)BookingEnum.Cancelled;
                            _context.Bookings.Update(book);
                        }
                        // Gửi thông báo cho người gửi
                        var notification = new NotificationDTO
                        {
                            NotificationId = Guid.NewGuid(),
                            UserId = sender.Id,
                            Title = "Yêu cầu xử lý báo cáo của bạn đã được chấp nhận",
                            Content = "Chúng tôi đã xem xét và xử lý báo cáo của bạn. Tiền của bạn đã được hoàn trả vui lòng kiểm tra lại nếu có gì sai sót hãy liên hệ với chúng tôi. Cảm ơn bạn đã sử dụng dịch vụ",
                            CreatedAt = DateTime.UtcNow.AddHours(7),
                            Status = 0
                        };
                        Notification notificationx = _mapper.Map<Notification>(notification);
                        _context.Notifications.Add(notificationx);
                        _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                        // Gửi thông báo cho tutor 
                        var notification1 = new NotificationDTO
                        {
                            NotificationId = Guid.NewGuid(),
                            UserId = tutor.UserNavigation.Id,
                            Title = "Thông báo nhắc nhở",
                            Content = "Chúng tôi đã nhận được một phản hồi không tốt liên quan đến dịch vụ của bạn. Nếu chúng tôi nhận được một sự phản hồi không tốt chính đáng với với tài khoản này vào lần sau bạn sẽ bị cấm sử dụng dịch vụ như quy định. Hãy sắp xếp công việc và thời gian nhé!",
                            CreatedAt = DateTime.UtcNow.AddHours(7),
                            Status = 0
                        };
                        // Create Tutor Action 
                        var tutorAction = new TutorAction()
                        {
                            TutorActionId = Guid.NewGuid(),
                            TutorId = tutor.TutorId,
                            ModeratorId = ApprovalId,
                            CreateAt = DateTime.UtcNow.AddHours(7),
                            ReponseDate = DateTime.UtcNow.AddHours(7),
                            Description = " Xử lý hành vi vi phạm của người dùng",
                            ActionType = (Int32)TutorActionTypeEnum.ReportBooking,
                            Status = (Int32)TutorActionEnum.Accept
                        };
                        await _context.SaveChangesAsync();
                        break;
                    }
                case 1:
                    // Refund money for student
                    // Get BoookingTransaction From BookingId
                    var bookingTransactionVer2 = await _context.BookingTransactions.FirstOrDefaultAsync(b => b.BookingId == bookingg.BookingId);
                    var walletTransactionIdVer2 = bookingTransactionVer2.BookingTransactionId;
                    var booksVer2 = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingg.BookingId);
                    var tutorVer2 = await _context.Tutors
                        .Include(t => t.UserNavigation)
                        .FirstOrDefaultAsync(t => t.TutorId == booksVer2.TutorId);
                    // Lấy thông tin giao dịch ví từ database
                    var walletVer2 = await _context.WalletTransactions.FirstOrDefaultAsync(w => w.WalletTransactionId == walletTransactionIdVer2);
                    if (walletVer2 == null)
                    {
                        return new StatusCodeResult(404);
                    }

                    // Lấy thông tin booking tương ứng
                    var bookingVer2 = await _context.BookingTransactions
                        .Include(b => b.BookingNavigation)
                        .FirstOrDefaultAsync(b => b.BookingTransactionId == walletTransactionIdVer2);
                    if (bookingVer2 == null)
                    {
                        return new StatusCodeResult(404);
                    }

                    // Xác định người gửi và người nhận
                    var senderVer2 = await _context.Users.Include(u => u.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == walletVer2.SenderWalletId);
                    var receiverVer2 = await _context.Users.Include(u => u.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == walletVer2.ReceiverWalletId);

                    // Cập nhật trạng thái giao dịch ví và booking
                    // Xử lý hoàn tiền
                    walletVer2.Status = (int)VNPayType.CANCELLED;
                    walletVer2.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    walletVer2.SenderWalletNavigation.AvalaibleAmount += bookingVer2.Amount;
                    walletVer2.SenderWalletNavigation.Amount += bookingVer2.Amount;
                    walletVer2.SenderWalletNavigation.PendingAmount += bookingVer2.Amount;

                    walletVer2.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    walletVer2.ReceiverWalletNavigation.PendingAmount -= bookingVer2.Amount;
                    bookingVer2.BookingNavigation.Status = (int)BookingEnum.Cancelled;

                    bookingVer2.Status = (int)VNPayType.CANCELLED;
                    _context.WalletTransactions.Update(walletVer2);
                    _context.BookingTransactions.Update(bookingVer2);

                    var bookVer2 = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingVer2.BookingId);
                    if (bookVer2 != null)
                    {
                        bookVer2.Status = (int)BookingEnum.Cancelled;
                        _context.Bookings.Update(bookVer2);
                    }
                    // Gửi thông báo cho người gửi
                    var notificationVer2 = new NotificationDTO
                    {
                        NotificationId = Guid.NewGuid(),
                        UserId = senderVer2.Id,
                        Title = "Yêu cầu xử lý báo cáo của bạn đã được chấp nhận",
                        Content = "Chúng tôi đã xem xét và xử lý báo cáo của bạn. Tiền của bạn đã được hoàn trả vui lòng kiểm tra lại nếu có gì sai sót hãy liên hệ với chúng tôi. Cảm ơn bạn đã sử dụng dịch vụ",
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = 0
                    };
                    Notification notificationxVer2 = _mapper.Map<Notification>(notificationVer2);
                    _context.Notifications.Add(notificationxVer2);
                    _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notificationVer2.UserId}/{notificationVer2.NotificationId}", notificationVer2);
                    // Block account tutor in 7 days
                    var userTutor = _context.Users.FirstOrDefault(x => x.Id == tutorVer2.UserId);
                    userTutor.Banned = true;
                    userTutor.BanExpiredAt = DateTime.UtcNow.AddHours(7).AddDays(7).Date; // Thiết lập thời gian là 00:00:00
                    _context.Users.Update(userTutor);
                    await _context.SaveChangesAsync();
                    // Create Tutor Action 
                    var tutorActionVer2 = new TutorAction()
                    {
                        TutorActionId = Guid.NewGuid(),
                        TutorId = tutorVer2.TutorId,
                        ModeratorId = ApprovalId,
                        CreateAt = DateTime.UtcNow.AddHours(7),
                        ReponseDate = DateTime.UtcNow.AddHours(7),
                        Description = " Xử lý hành vi vi phạm của người dùng",
                        ActionType = (Int32)TutorActionTypeEnum.ReportBooking,
                        Status = (Int32)TutorActionEnum.Accept
                    };
                    await _context.SaveChangesAsync();
                    break;
            }
            // Update Report Status
            report.Status = (Int32)ReportEnum.Finished;
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);

        }

        // Deny Report Booking 
        public async Task<IActionResult> DenyReportOfTutor (Guid ReportId, Guid ApprovalId)
        {
            try
            {
                // Send Money to Tutor 
                var report = _context.Reports.FirstOrDefault(x => x.ReportId == ReportId);
                if (report == null) return new StatusCodeResult(404);
                var bookingg = _context.Bookings
                    .Include(x => x.TutorNavigation)
                    .Include(x => x.TutorNavigation.UserNavigation)
                    .FirstOrDefault(x => x.BookingId == report.TargetId);
                // Get BoookingTransaction From BookingId
                var booking = await _context.BookingTransactions.FirstOrDefaultAsync(b => b.BookingId == bookingg.BookingId);
                var walletTransactionId = booking.BookingTransactionId;
                var books = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingg.BookingId);
                var tutor = await _context.Tutors
                    .Include(t => t.UserNavigation)
                    .FirstOrDefaultAsync(t => t.TutorId == books.TutorId);
                // Lấy thông tin giao dịch ví từ database
                var wallet = await _context.WalletTransactions.FirstOrDefaultAsync(w => w.WalletTransactionId == walletTransactionId);
                if (wallet == null)
                {
                    return new StatusCodeResult(404);
                }
                wallet.Status = (int)VNPayType.APPROVE;
                wallet.SenderWalletNavigation.PendingAmount += booking.Amount;
                wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;
                // Caculate rose fee for Admin and Tutor based on number of finished bookings of tutor
                int percentageOfTutor = GetTutorPercentageOfTutorByUserId(booking.ReceiverWalletNavigation.UserId);
                wallet.ReceiverWalletNavigation.AvalaibleAmount += (booking.Amount - (booking.Amount * percentageOfTutor) / 100);
                wallet.ReceiverWalletNavigation.Amount += (booking.Amount - (booking.Amount * percentageOfTutor) / 100);
                booking.Status = (int)VNPayType.APPROVE;
                // Send money from tutor to admin based on percentage of tutor
                var adminWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == Guid.Parse("D71B17CC-7997-4B23-2ADC-08DC93FF561F"));
                adminWallet.AvalaibleAmount += (booking.Amount * percentageOfTutor) / 100;
                adminWallet.Amount += (booking.Amount * percentageOfTutor) / 100;
                // Create wallet transaction for admin
                var walletTransactionForAdmin = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    SenderWalletId = booking.ReceiverWalletId,
                    ReceiverWalletId = adminWallet.WalletId,
                    Amount = (booking.Amount * percentageOfTutor) / 100,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (int)VNPayType.APPROVE,
                    Note = "Hoa hồng từ giao dịch của buổi học" + booking.BookingNavigation.BookingId + " của gia sư " + booking.ReceiverWalletNavigation.UserNavigation.Name
                };
                _context.WalletTransactions.Add(walletTransactionForAdmin);
                _context.Wallets.Update(adminWallet);
                _context.BookingTransactions.Update(booking);
                _context.WalletTransactions.Update(wallet);
                await _appExtension.SendMail(new MailContent()
                {
                    To = booking.SenderWalletNavigation.UserNavigation.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId
                });
                await _appExtension.SendMail(new MailContent()
                {
                    To = booking.ReceiverWalletNavigation.UserNavigation.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId
                });
                var notification1 = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Giao dịch booking",
                    Content = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId,
                    UserId = booking.SenderWalletNavigation.UserId,
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };
                var notification2 = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Giao dịch booking",
                    Content = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId,
                    UserId = booking.ReceiverWalletNavigation.UserId,
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };

                Models.Entities.Notification notification1x = _mapper.Map<Models.Entities.Notification>(notification1);
                Models.Entities.Notification notification2x = _mapper.Map<Models.Entities.Notification>(notification2);
                _context.Notifications.Add(notification1x);
                _context.Notifications.Add(notification2x);
                _firebaseRealtimeDatabaseService.UpdateAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                _firebaseRealtimeDatabaseService.UpdateAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
                // Create Tutor Action
                var tutorAction = new TutorAction()
                {
                    TutorActionId = Guid.NewGuid(),
                    TutorId = tutor.TutorId,
                    ModeratorId = ApprovalId,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    ReponseDate = DateTime.UtcNow.AddHours(7),
                    Description = " Từ chối xử lý hành vi vi phạm của người dùng vì không có đủ bằng chứng",
                    ActionType = (Int32)TutorActionTypeEnum.ReportBooking,
                    Status = (Int32)TutorActionEnum.Reject
                };
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        // Find total booking finished of tutor and show the rose percentage for admin and tutor
        public int GetTutorPercentageOfTutorByUserId(Guid userId)
        {
            int response = 0;
            var tutor = _context.Tutors.FirstOrDefault(t => t.UserId == userId);
            if (tutor == null)
            {
                return 0;
            }
            var totalEndBooking = _context.Bookings.Where(b => b.TutorId == tutor.TutorId && b.Status == (int)BookingEnum.Finished).Count();
            if (totalEndBooking < 10)
            {
                response = 10;
            }
            else if (totalEndBooking >= 10 && totalEndBooking < 20)
            {
                response = 8;
            }
            else if (totalEndBooking >= 20 && totalEndBooking < 30)
            {
                response = 6;
            }
            else
            {
                response = 5;
            }
            return response;
        }
    }
}
