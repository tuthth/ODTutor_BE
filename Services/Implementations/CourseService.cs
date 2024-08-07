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
using System.Net;
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

        // Create Course
        public async Task<IActionResult> CreateCourse(CourseRequest courseRequest)
        {
            var tutor = await _context.Tutors
                .Include(t => t.UserNavigation)
                .FirstOrDefaultAsync(c => c.TutorId == courseRequest.TutorId);
            var tutorSlot = _context.TutorSlotAvailables.Include(x => x.TutorDateAvailable).FirstOrDefault(x => x.TutorSlotAvailableID == courseRequest.TutorSlotAvalaibleID);
            if (tutor == null)
            {
                return new StatusCodeResult(404);
            }
            if (courseRequest.TotalMoney < 0 || courseRequest.TotalStudents < 0)
            {
                return new StatusCodeResult(400);
            }
            if (tutorSlot.IsBooked == true)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Tutor slot available is booked", "");
            }
            if (tutorSlot.Status == (Int32)TutorSlotAvailabilityEnum.NotAvailable)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Tutor slot available is not available", "");
            }
            DateTime studyTime = new DateTime(tutorSlot.TutorDateAvailable.Date.Year, tutorSlot.TutorDateAvailable.Date.Month, tutorSlot.TutorDateAvailable.Date.Day, tutorSlot.StartTime.Hours, tutorSlot.StartTime.Minutes, tutorSlot.StartTime.Seconds);
            var course = _mapper.Map<Course>(courseRequest);
            course.CourseId = Guid.NewGuid();
            course.Status = (Int32)CourseEnum.Active;
            course.StudyTime = studyTime;
            course.CreatedAt = DateTime.UtcNow.AddHours(7);
            _context.Courses.Add(course);

            // Update tutor slot available
            // Find the tutor available slot
            // Changre status slot 
            TimeSpan courseTime = new TimeSpan(course.StudyTime.Value.Hour, course.StudyTime.Value.Minute, 0);
            DateTime CourseDate = course.StudyTime.Value.Date;
            var tutorDateAvailables = _context.TutorDateAvailables
                .Where(x => x.TutorID == course.TutorId && x.Date.Date == CourseDate)
                .Select(x => x.TutorDateAvailableID)
                .ToList();
            if (tutorDateAvailables == null)
            {
                return new StatusCodeResult(452);
            }
            var tutorSlotAvailables = _context.TutorSlotAvailables
                .Where(x => tutorDateAvailables.Contains(x.TutorDateAvailable.TutorDateAvailableID) && x.StartTime == courseTime)
                .FirstOrDefault();
            if (tutorSlotAvailables == null)
            {
                return new StatusCodeResult(453);
            }
            if (tutorSlotAvailables.IsBooked == true)
            {
                return new StatusCodeResult(454);
            }
            tutorSlotAvailables.IsBooked = true;
            tutorSlotAvailables.Status = (Int32)TutorSlotAvailabilityEnum.NotAvailable;
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
            if (courseRequest.TotalMoney < 0 || courseRequest.TotalStudents < 0)
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
            if (courseRequest.TotalStudents > 0 && courseRequest.TotalStudents != course.TotalStudent)
            {
                course.TotalStudent = courseRequest.TotalStudents;
            }
            if (courseRequest.Note != null && !courseRequest.Note.Equals(course.Note))
            {
                course.Note = courseRequest.Note;
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
        public async Task<IActionResult> CreateCourseSlot(CourseSlotRequest request)
        {
            var course = await _context.Courses.Include(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseId == request.CourseId);
            if (course == null)
            {
                return new StatusCodeResult(404);
            }
            var courseSlot = await _context.CourseSlots.FirstOrDefaultAsync(c => c.CourseId == request.CourseId && c.Description == request.Description);
            if (courseSlot != null)
            {
                return new StatusCodeResult(409);
            }
            var courseSlotsExist = await _context.CourseSlots.Where(c => c.CourseId == request.CourseId).ToListAsync();
            var newSlot = new CourseSlot
            {
                CourseSlotId = Guid.NewGuid(),
                CourseId = request.CourseId,
                Description = request.Description,
                SlotNumber = courseSlotsExist.Count == 0 ? 1 : courseSlotsExist.Max(c => c.SlotNumber) + 1
            };
            _context.CourseSlots.Add(newSlot);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateCourseSlot(UpdateCourseSlotRequest request)
        {
            var courseSlot = await _context.CourseSlots.Include(c => c.CourseNavigation).ThenInclude(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseSlotId == request.CourseSlotId);
            if (courseSlot == null)
            {
                return new StatusCodeResult(404);
            }
            courseSlot.Description = request.Description;
            _context.CourseSlots.Update(courseSlot);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> SwapSlotNumber(CourseSlotSwapRequest request)
        {
            var courseSlot1 = await _context.CourseSlots.FirstOrDefaultAsync(c => c.CourseSlotId == request.CourseSlotId1);
            var courseSlot2 = await _context.CourseSlots.FirstOrDefaultAsync(c => c.CourseSlotId == request.CourseSlotId2);
            if(courseSlot1 == null || courseSlot2 == null)
            {
                return new StatusCodeResult(404);
            }
            if(courseSlot1.CourseId != courseSlot2.CourseId)
            {
                return new StatusCodeResult(409);
            }
            var temp = courseSlot1.SlotNumber;
            courseSlot1.SlotNumber = courseSlot2.SlotNumber;
            courseSlot2.SlotNumber = temp;
            _context.CourseSlots.Update(courseSlot1);
            _context.CourseSlots.Update(courseSlot2);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> DeleteCourseSlot(Guid id)
        {
            var courseSlot = await _context.CourseSlots.Include(c => c.CourseNavigation).ThenInclude(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseSlotId == id);
            if (courseSlot == null)
            {
                return new StatusCodeResult(404);
            }
            var courseId = courseSlot.CourseId;
            var slotNumberToDelete = courseSlot.SlotNumber;

            _context.CourseSlots.Remove(courseSlot);
            var subsequentSlots = await _context.CourseSlots.Where(c => c.CourseId == courseId && c.SlotNumber > slotNumberToDelete).ToListAsync();
            foreach (var slot in subsequentSlots)
            {
                slot.SlotNumber--;
                _context.CourseSlots.Update(slot);
            }
            await _context.SaveChangesAsync();
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
        public async Task<ActionResult<List<Promotion>>> GetPromotionsByTutorId(Guid tutorId)
        {
           try
            {
                var promotions = await _context.Promotions.Where(c => c.TutorId == tutorId).OrderBy(c => c.CreatedAt).ToListAsync();
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
        public async Task<ActionResult<List<CourseSlot>>> GetAllCourseSlots()
        {
            try
            {
                var courseSlots = await _context.CourseSlots.OrderBy(c => c.CourseId).ToListAsync();
                if (courseSlots == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseSlots;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CourseSlot>> GetCourseSlot(Guid id)
        {
            try
            {
                var courseSlot = await _context.CourseSlots.FirstOrDefaultAsync(c => c.CourseSlotId == id);
                if (courseSlot == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseSlot;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseSlot>>> GetCourseSlotsByCourseId(Guid courseId)
        {
            try
            {
                var courseSlots = await _context.CourseSlots.Where(c => c.CourseId == courseId).ToListAsync();
                if (courseSlots == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseSlots;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Student Register Student Course 
        public async Task<IActionResult> RegisterCourse(RegisterCourseRequest request)
        {
            var course = await _context.Courses.Include(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.CourseId == request.CourseId);
            var student = await _context.Students.Include(c => c.UserNavigation).FirstOrDefaultAsync(c => c.StudentId == request.StudentId);
            if (course == null || student == null)
            {
                return new StatusCodeResult(404);
            }
            // Kiểm tra đã đủ sô lượng trong khóa học chưa
            var studentCourses = await _context.StudentCourses
                .Where(c => c.CourseId == request.CourseId 
                && c.Status == (Int32)StudentCourseEnum.Success)
                .ToListAsync();
            if (studentCourses.Count >= course.TotalStudent)
            {
                return new StatusCodeResult(409);
            }
            var studentCourse = new StudentCourse
            {
                StudentCourseId = Guid.NewGuid(),
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                Status = (Int32)StudentCourseEnum.Success,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                StartDate = course.StudyTime.Value,
                // Cộng thêm 50 phút cho end date 
                EndDate = course.StudyTime.Value.AddMinutes(50)
            };
            _context.StudentCourses.Add(studentCourse);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Đăng ký khóa học thành công",
                Content = "Bạn đã đăng ký khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học.",
                UserId = student.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = student.UserNavigation.Email,
                Subject = "[ODTutor] Đăng ký khóa học thành công",
                Body = "Bạn đã đăng ký khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học."
            });
            return new StatusCodeResult(201);
        }


        // Cancle Register Student Course
        public async Task<IActionResult> CancelCourse(CancleCourseRequest request)
        {
            var studentCourse = await _context
                .StudentCourses.Include(c => c.CourseNavigation).ThenInclude(c => c.TutorNavigation).ThenInclude(c => c.UserNavigation).FirstOrDefaultAsync(c => c.StudentCourseId == request.RegisterCourseId);
            if (studentCourse == null)
            {
                return new StatusCodeResult(404);
            }
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == studentCourse.CourseId);
            _context.StudentCourses.Remove(studentCourse);
            _context.Courses.Update(course);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Hủy đăng ký khóa học thành công",
                Content = "Bạn đã hủy đăng ký khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học.",
                UserId = studentCourse.StudentNavigation.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = studentCourse.StudentNavigation.UserNavigation.Email,
                Subject = "[ODTutor] Hủy đăng ký khóa học thành công",
                Body = "Bạn đã hủy đăng ký khóa học thành công. Bạn có thể sử dụng tài khoản của mình để kiểm tra thông tin khóa học."
            });
            return new StatusCodeResult(200);
        }   
    }
}
