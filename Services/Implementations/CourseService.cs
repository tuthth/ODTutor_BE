using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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

namespace Services.Implementations
{
    public class CourseService : BaseService, ICourseService
    {
        private readonly IFirebaseRealtimeDatabaseService _firebaseRealtimeDatabaseService;
        public CourseService(ODTutorContext context, IMapper mapper, IFirebaseRealtimeDatabaseService firebaseRealtimeDatabaseService) : base(context, mapper)
        {
            _firebaseRealtimeDatabaseService = firebaseRealtimeDatabaseService;
        }
        public async Task<IActionResult> CreateCourse(CourseRequest courseRequest)
        {
            var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == courseRequest.TutorId);
            if (tutor == null)
            {
                return new StatusCodeResult(404);
            }
            if (courseRequest.TotalMoney < 0 || courseRequest.TotalSlots < 0)
            {
                return new StatusCodeResult(400);
            }
            var course = _mapper.Map<Course>(courseRequest);
            course.CourseId = Guid.NewGuid();
            _context.Courses.Add(course);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Tạo khóa học thành công",
                Content = "Chúc mừng bạn đã tạo khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học.",
                UserId = tutor.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = tutor.UserNavigation.Email,
                Subject = "[ODTutor] Tạo khóa học thành công",
                Body = "Chúc mừng bạn đã tạo khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học."
            });
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateCourse(UpdateCourseRequest courseRequest)
        {
            var tutor = await _context.Tutors.Include(c => c.UserNavigation).FirstOrDefaultAsync(c => c.TutorId == courseRequest.TutorId);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseRequest.CourseId);
            if (tutor == null || course == null)
            {
                return new StatusCodeResult(404);
            }
            if(course.Status == (Int32)CourseEnum.Deleted) return new StatusCodeResult(409);
            if (courseRequest.TotalMoney < 0 || courseRequest.TotalSlots < 0)
            {
                return new StatusCodeResult(400);
            }
            if (courseRequest.Description != null)
            {
                course.Description = courseRequest.Description;
            }
            if (courseRequest.TotalMoney > 0 && courseRequest.TotalMoney != course.TotalMoney)
            {
                course.TotalMoney = courseRequest.TotalMoney;
            }
            if (courseRequest.TotalSlots > 0 && courseRequest.TotalSlots != course.TotalSlots)
            {
                course.TotalSlots = courseRequest.TotalSlots;
            }
            if (courseRequest.Note != null && !courseRequest.Note.Equals(course.Note))
            {
                course.Note = courseRequest.Note;
            }
            if (courseRequest.Status > 0 && courseRequest.Status != course.Status)
            {
                course.Status = courseRequest.Status;
            }
            _context.Courses.Update(course);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Cập nhật khóa học thành công",
                Content = "Chúc mừng bạn đã cập nhật khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học.",
                UserId = tutor.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification noti = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(noti);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = tutor.UserNavigation.Email,
                Subject = "[ODTutor] Cập nhật khóa học thành công",
                Body = "Chúc mừng bạn đã cập nhật khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học."
            });
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                var course = await _context.Courses.Include(c => c.TutorNavigation).FirstOrDefaultAsync(c => c.CourseId == id);
                if (course == null)
                {
                    return new StatusCodeResult(404);
                }
                var courseTransactions = await _context.CourseTransactions.AnyAsync(c => c.CourseId == id);
                if (courseTransactions == true)
                {
                    if (course.Status != (Int32)CourseEnum.Deleted)
                    {
                        course.Status = (Int32)CourseEnum.Deleted;
                        _context.Courses.Update(course);
                        var courseOutlines = await _context.CourseOutlines.Where(c => c.CourseId == id).ToListAsync();
                        foreach (var courseOutline in courseOutlines)
                        {
                            courseOutline.Status = (Int32)CourseEnum.Deleted;
                            _context.CourseOutlines.Update(courseOutline);
                        }
                        var coursePromotions = await _context.CoursePromotions.Where(c => c.CourseId == id).ToListAsync();
                        foreach (var coursePromotion in coursePromotions)
                        {
                            _context.CoursePromotions.Remove(coursePromotion);
                        }
                        var studentCourses = await _context.StudentCourses.Where(c => c.CourseId == id).ToListAsync();
                        foreach (var studentCourse in studentCourses)
                        {
                            var schedules = await _context.Schedules.Where(c => c.StudentCourseId == studentCourse.StudentCourseId).ToListAsync();
                            foreach (var schedule in schedules)
                            {
                                schedule.Status = (Int32)CourseEnum.Deleted;
                                _context.Schedules.Update(schedule);
                            }
                            studentCourse.Status = (Int32)CourseEnum.Deleted;
                            _context.StudentCourses.Update(studentCourse);
                        }
                        var notification = new NotificationDTO
                        {
                            NotificationId = Guid.NewGuid(),
                            Title = "Xóa khóa học khỏi tìm kiếm thành công",
                            Content = "Khóa học đã được xóa khỏi tìm kiếm. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học. Học viên đã từng đăng ký vẫn có thể truy cập để lấy tài liệu.",
                            UserId = course.TutorNavigation.UserId,
                            CreatedAt = DateTime.UtcNow.AddHours(7),
                            Status = (Int32)NotificationEnum.UnRead
                        };
                        Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
                        _context.Notifications.Add(notification1);
                        _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                        await _context.SaveChangesAsync();
                        await _appExtension.SendMail(new MailContent()
                        {
                            To = course.TutorNavigation.UserNavigation.Email,
                            Subject = "[ODTutor] Xóa khóa học khỏi tìm kiếm thành công",
                            Body = "Khóa học đã được xóa khỏi tìm kiếm. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa. Học viên đã từng đăng ký vẫn có thể truy cập để lấy tài liệu."
                        });
                        return new StatusCodeResult(200);
                    }
                    else return new StatusCodeResult(409);
                }
                else
                {
                    _context.Courses.Remove(course);
                    var coursePromotions = await _context.CoursePromotions.Where(c => c.CourseId == id).ToListAsync();
                    foreach (var coursePromotion in coursePromotions)
                    {
                        _context.CoursePromotions.Remove(coursePromotion);
                    }
                    var courseOutlines = await _context.CourseOutlines.Where(c => c.CourseId == id).ToListAsync();
                    foreach (var courseOutline in courseOutlines)
                    {
                        _context.CourseOutlines.Remove(courseOutline);
                    }
                    var studentCourses = await _context.StudentCourses.Where(c => c.CourseId == id).ToListAsync();
                    foreach (var studentCourse in studentCourses)
                    {
                        var schedules = await _context.Schedules.Where(c => c.StudentCourseId == studentCourse.StudentCourseId).ToListAsync();
                        _context.Schedules.RemoveRange(schedules);
                        _context.StudentCourses.Remove(studentCourse);
                    }
                    var notification = new NotificationDTO
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Xóa khóa học thành công",
                        Content = "Khóa học đã được xóa thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học.",
                        UserId = course.TutorNavigation.UserId,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (Int32)NotificationEnum.UnRead
                    };
                    Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
                    _context.Notifications.Add(notification1);
                    _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                    await _context.SaveChangesAsync();
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = course.TutorNavigation.UserNavigation.Email,
                        Subject = "[ODTutor] Xóa khóa học thành công",
                        Body = "Khóa học và toàn bộ thông tin liên quan đã được xóa thành công."
                    });
                    return new StatusCodeResult(204);
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> CreateCourseOutline(CourseOutlineRequest courseOutlineRequest)
        {
            var course = await _context.Courses.Include(c => c.TutorNavigation).FirstOrDefaultAsync(c => c.CourseId == courseOutlineRequest.CourseId && c.Status == (Int32)CourseEnum.Active);
            if (course == null)
            {
                return new StatusCodeResult(404);
            }
            var courseOutline = _mapper.Map<CourseOutline>(courseOutlineRequest);
            courseOutline.CourseOutlineId = Guid.NewGuid();
            _context.CourseOutlines.Add(courseOutline);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Tạo đề cương khóa học thành công",
                Content = "Đề cương được tạo thành công. Vui lòng kiểm tra thông tin đề cương tại mục Khóa học.",
                UserId = course.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = course.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Tạo đề cương khóa học thành công",
                Body = "Đề cương được tạo thành công. Vui lòng kiểm tra thông tin đề cương tại mục Khóa học."
            });
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateCourseOutline(UpdateCourseOutlineRequest courseOutlineRequest)
        {
            var course = await _context.Courses.Include(c => c.TutorNavigation).FirstOrDefaultAsync(c => c.CourseId == courseOutlineRequest.CourseId && c.Status == (Int32)CourseEnum.Active);
            var courseOutline = await _context.CourseOutlines.FirstOrDefaultAsync(c => c.CourseOutlineId == courseOutlineRequest.CourseOutlineId && c.Status != (Int32)CourseEnum.Deleted);
            if (course == null || courseOutline == null)
            {
                return new StatusCodeResult(404);
            }
            if(course.Status == (Int32)CourseEnum.Deleted) return new StatusCodeResult(409);
            if (courseOutlineRequest.Description != null)
            {
                courseOutline.Description = courseOutlineRequest.Description;
            }
            if (courseOutlineRequest.Title != null)
            {
                courseOutline.Title = courseOutlineRequest.Title;
            }
            if (courseOutlineRequest.Status > 0 && courseOutlineRequest.Status != courseOutline.Status)
            {
                courseOutline.Status = courseOutlineRequest.Status;
            }
            _context.CourseOutlines.Update(courseOutline);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Cập nhật đề cương khóa học thành công",
                Content = "Đề cương được cập nhật thành công. Vui lòng kiểm tra thông tin đề cương tại mục Khóa học.",
                UserId = course.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = course.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Cập nhật đề cương khóa học thành công",
                Body = "Đề cương được cập nhật thành công. Vui lòng kiểm tra thông tin đề cương tại mục Khóa học."
            });
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> DeleteCourseOutline(Guid id)
        {
            var courseOutline = await _context.CourseOutlines.FirstOrDefaultAsync(c => c.CourseOutlineId == id);
            var course = await _context.Courses.Include(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseId == courseOutline.CourseId);
            var transactions = await _context.CourseTransactions.AnyAsync(c => c.CourseId == id);
            if (courseOutline == null || course == null)
            {
                return new StatusCodeResult(404);
            }
            if (transactions == true)
            {
                if (course.Status == (Int32)CourseEnum.Deleted)
                {
                    return new StatusCodeResult(409);
                }
                else
                {
                    courseOutline.Status = (Int32)CourseEnum.Deleted;
                    _context.CourseOutlines.Update(courseOutline);
                    var notification = new NotificationDTO
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Xóa đề cương khóa học khỏi tìm kiếm thành công",
                        Content = "Đề cương được xóa khỏi tìm kiếm thành công. Các tài khoản đã đăng kí khóa học vẫn có thể truy cập vào nguồn tài liệu.",
                        UserId = course.TutorNavigation.UserId,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (Int32)NotificationEnum.UnRead
                    };
                    Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
                    _context.Notifications.Add(notification1);
                    _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                    await _context.SaveChangesAsync();
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = course.TutorNavigation.UserNavigation.Email,
                        Subject = "[ODTutor] Xóa đề cương khóa học khỏi tìm kiếm thành công",
                        Body = "Đề cương được xóa khỏi tìm kiếm thành công. Các tài khoản đã đăng kí khóa học vẫn có thể truy cập vào nguồn tài liệu."
                    });
                    return new StatusCodeResult(200);
                }
            }
            else
            {
                _context.CourseOutlines.Remove(courseOutline);
                var notification = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Xóa đề cương khóa học thành công",
                    Content = "Đề cương được xóa khỏi hệ thống thành công.",
                    UserId = course.TutorNavigation.UserId,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (Int32)NotificationEnum.UnRead
                };
                Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
                _context.Notifications.Add(notification1);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                await _appExtension.SendMail(new MailContent()
                {
                    To = course.TutorNavigation.UserNavigation.Email,
                    Subject = "[ODTutor] Xóa đề cương khóa học thành công",
                    Body = "Đề cương được xóa khỏi hệ thống thành công."
                });
                return new StatusCodeResult(204);
            }

        }
        public async Task<IActionResult> CreateCoursePromotion(CoursePromotionRequest coursePromotionRequest)
        {
            var course = await _context.Courses.Include(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseId == coursePromotionRequest.CourseId && c.Status == (Int32)CourseEnum.Active);
            var promotion = await _context.Promotions.FirstOrDefaultAsync(c => c.PromotionId == coursePromotionRequest.PromotionId);
            if (course == null || promotion == null)
            {
                return new StatusCodeResult(404);
            }
            var coursePromotion = _mapper.Map<CoursePromotion>(coursePromotionRequest);
            _context.CoursePromotions.Add(coursePromotion);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Tạo mã giảm giá cho khóa học thành công",
                Content = "Mã giảm giá được tạo thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá.",
                UserId = course.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = course.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Tạo mã giảm giá cho khóa học thành công",
                Body = "Mã giảm giá được tạo thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá."
            });
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> DeleteCoursePromotion(CoursePromotionRequest coursePromotionRequest)
        {
            var coursePromotion = await _context.CoursePromotions.Include(c => c.CourseNavigation).ThenInclude(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseId == coursePromotionRequest.CourseId && c.PromotionId == coursePromotionRequest.PromotionId);
            if (coursePromotion == null)
            {
                return new StatusCodeResult(404);
            }
            _context.CoursePromotions.Remove(coursePromotion);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Xóa mã giảm giá khóa học thành công",
                Content = "Mã giảm giá được xóa thành công.",
                UserId = coursePromotion.CourseNavigation.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = coursePromotion.CourseNavigation.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Xóa mã giảm giá khóa học thành công",
                Body = "Mã giảm giá được xóa thành công."
            });
            return new StatusCodeResult(204);
        }
        public async Task<IActionResult> UpdateCoursePromotion(CoursePromotionRequest coursePromotionRequest)
        {
            var coursePromotion = await _context.CoursePromotions.Include(c => c.CourseNavigation).ThenInclude(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseId == coursePromotionRequest.CourseId && c.PromotionId == coursePromotionRequest.PromotionId);
            if (coursePromotion == null)
            {
                return new StatusCodeResult(404);
            }
            coursePromotion.PromotionId = coursePromotionRequest.PromotionId;
            _context.CoursePromotions.Update(coursePromotion);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Cập nhật mã giảm giá khóa học thành công",
                Content = "Mã giảm giá được cập nhật thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá.",
                UserId = coursePromotion.CourseNavigation.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = coursePromotion.CourseNavigation.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Cập nhật mã giảm giá khóa học thành công",
                Body = "Mã giảm giá được cập nhật thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá."
            });
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> CreatePromotion(CreatePromotion createPromotion)
        {
            var promotionExist = await _context.Promotions.Include(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).AnyAsync(c => c.PromotionCode == createPromotion.PromotionCode); //moi ma 1 loai phan tram thoi. Ex: VARLUOTDIOLE, PEREZMUATAI
            if (promotionExist)
            {
                return new StatusCodeResult(409);
            }
            var promotion = _mapper.Map<Promotion>(createPromotion);
            promotion.PromotionId = Guid.NewGuid();
            _context.Promotions.Add(promotion);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Tạo mã giảm giá thành công",
                Content = "Mã giảm giá được tạo thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá.",
                UserId = promotion.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = promotion.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Tạo mã giảm giá thành công",
                Body = "Mã giảm giá được tạo thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá."
            });
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdatePromotion(UpdatePromotion updatePromotion)
        {
            var promotion = await _context.Promotions.Include(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.PromotionId == updatePromotion.PromotionId);
            if (promotion == null)
            {
                return new StatusCodeResult(404);
            }
            promotion.PromotionCode = updatePromotion.PromotionCode.ToUpper();
            promotion.Percentage = updatePromotion.Percentage;
            _context.Promotions.Update(promotion);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Cập nhật mã giảm giá thành công",
                Content = "Mã giảm giá được cập nhật thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá.",
                UserId = promotion.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = promotion.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Cập nhật mã giảm giá thành công",
                Body = "Mã giảm giá được cập nhật thành công. Vui lòng kiểm tra thông tin tại mục Mã giảm giá."
            });
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            var promotion = await _context.Promotions.Include(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.PromotionId == id);
            if (promotion == null)
            {
                return new StatusCodeResult(404);
            }
            _context.Promotions.Remove(promotion);
            var notification = new  NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Xóa mã giảm giá thành công",
                Content = "Mã giảm giá được xóa thành công",
                UserId = promotion.TutorNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = promotion.TutorNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Xóa mã giảm giá thành công",
                Body = "Mã giảm giá được xóa thành công."
            });
            return new StatusCodeResult(204);
        }
        public async Task<ActionResult<List<Course>>> GetAllCourses()
        {
            try
            {
                var courses = await _context.Courses.ToListAsync();
                if (courses == null)
                {
                    return new StatusCodeResult(404);
                }
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
                if (course == null)
                {
                    return new StatusCodeResult(404);
                }
                return course;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseOutline>>> GetAllCourseOutlines()
        {
            try
            {
                var courseOutlines = await _context.CourseOutlines.ToListAsync();
                if (courseOutlines == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseOutlines;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CourseOutline>> GetCourseOutline(Guid id)
        {
            try
            {
                var courseOutline = await _context.CourseOutlines.FirstOrDefaultAsync(c => c.CourseOutlineId == id);
                if (courseOutline == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseOutline;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CoursePromotion>>> GetAllCoursePromotions()
        {
            try
            {
                var coursePromotions = await _context.CoursePromotions.ToListAsync();
                if (coursePromotions == null)
                {
                    return new StatusCodeResult(404);
                }
                return coursePromotions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CoursePromotion>> GetCoursePromotion(Guid id)
        {
            try
            {
                var coursePromotion = await _context.CoursePromotions.FirstOrDefaultAsync(c => c.CourseId == id || c.PromotionId == id);
                if (coursePromotion == null)
                {
                    return new StatusCodeResult(404);
                }
                return coursePromotion;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Promotion>>> GetAllPromotions()
        {
            try
            {
                var promotions = await _context.Promotions.ToListAsync();
                if (promotions == null)
                {
                    return new StatusCodeResult(404);
                }
                return promotions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Promotion>> GetPromotion(Guid id)
        {
            try
            {
                var promotion = await _context.Promotions.FirstOrDefaultAsync(c => c.PromotionId == id);
                if (promotion == null)
                {
                    return new StatusCodeResult(404);
                }
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
