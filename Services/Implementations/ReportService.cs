using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Emails;
using Models.Models.Requests;
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
            Notification notification2x = _mapper.Map<Notification>(notification1);
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
            if(updateReportRequest.Status == (Int32)ReportEnum.Cancelled)
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
        public async Task<IActionResult> MakeActionReport(ReportAction action)
        {
            var report = _context.Reports.FirstOrDefault(x => x.ReportId == action.ReportId);
            if (report == null) return new StatusCodeResult(404);
            var sender = _context.Users.FirstOrDefault(x => x.Id == report.SenderUserId);
            var target = _context.Users.FirstOrDefault(x => x.Id == report.TargetId);

            if (report.Status != (Int32)ReportEnum.Finished) return new StatusCodeResult(409);
            DateTime finishedTime = DateTime.UtcNow.AddHours(7);
            if(action.Status == (Int32)ReportActionEnum.SevenDays)
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
    }
}
