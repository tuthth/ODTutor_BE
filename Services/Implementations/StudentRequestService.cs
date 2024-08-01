using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class StudentRequestService : BaseService, IStudentRequestService
    {
        private readonly IFirebaseRealtimeDatabaseService _service;
        private readonly INotificationService _notificationService;
        public StudentRequestService(ODTutorContext context, IMapper mapper, INotificationService notificationService, IFirebaseRealtimeDatabaseService service) : base(context, mapper)
        {
            _service = service;
            _notificationService = notificationService;
        }
        public async Task<IActionResult> CreateStudentRequest(CreateStudentRequest request)
        {
            Student student = _context.Students.FirstOrDefault(x => x.StudentId == request.StudentId);
            var user = _context.Users.FirstOrDefault(x => x.Id == student.UserId);
            if (user.HasBoughtSubscription == true && user.RequestRefreshTime <= DateTime.UtcNow.AddHours(7))
            {
                user.HasBoughtSubscription = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(406);
            }
            var studentRequestCount = _context.StudentRequests
                .Where(x => x.StudentId == request.StudentId && x.CreatedAt >= user.RequestRefreshTime)
                .Count();

            int maxRequests = user.HasBoughtSubscription ? 25 : 5;

            if (studentRequestCount >= maxRequests)
            {
                return new StatusCodeResult(409);
            }
            if (studentRequestCount == 0)
            {
                user.RequestRefreshTime = DateTime.UtcNow.AddHours(7).AddDays(30);
            }
            var subject = _context.Subjects.FirstOrDefault(x => x.SubjectId == request.SubjectId);
            if (student == null || subject == null)
            {
                return new StatusCodeResult(404);
            }
            var studentRequest = _mapper.Map<StudentRequest>(request);
            studentRequest.CreatedAt = DateTime.UtcNow.AddHours(7);
            studentRequest.StudentRequestId = Guid.NewGuid();
            studentRequest.Status = (Int32)StudentRequestEnum.Pending;
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Yêu cầu của bạn đã được gửi đến các gia sư khác",
                Content = "Vui lòng chờ trong thời gian sớm nhất để nhận được phản hồi từ các gia sư khác trong tin nhắn nhé",
                UserId = student.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1x = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Users.Update(user);
            await _service.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _service.SetAsync<StudentRequest>($"Studentrequest/{studentRequest.StudentRequestId}", studentRequest);
            await _context.Notifications.AddAsync(notification1x);
            await _context.StudentRequests.AddAsync(studentRequest);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }

        public async Task<IActionResult> UpdateStudentRequest(UpdateStudentRequest request)
        {
            var studentRequest = _context.StudentRequests.Include(x => x.StudentNavigation).FirstOrDefault(x => x.StudentRequestId == request.StudentRequestId);
            var subject = _context.Subjects.FirstOrDefault(x => x.SubjectId == request.SubjectId);
            Student student = _context.Students.FirstOrDefault(x => x.StudentId == studentRequest.StudentId);
            if (studentRequest == null || subject == null)
            {
                return new StatusCodeResult(404);
            }
            studentRequest.SubjectId = request.SubjectId;
            studentRequest.Message = request.Message;
            studentRequest.Status = (Int32)StudentRequestEnum.Pending;
            studentRequest.CreatedAt = DateTime.Now;
            var studentRequestDTO = new StudentRequestDTO
            {
                StudentRequestId = request.StudentRequestId,
                SubjectId = request.SubjectId,
                Message = request.Message,
                Status = (Int32)StudentRequestEnum.Pending,
                CreatedAt = studentRequest.CreatedAt
            };
            await _service.UpdateAsync($"Studentrequest/{studentRequest.StudentRequestId}", studentRequestDTO);
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Yêu cầu của bạn đã được cập nhật",
                Content = "Vui lòng chờ trong thời gian sớm nhất để nhận được phản hồi từ các gia sư khác trong tin nhắn nhé",
                UserId = student.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1x = _mapper.Map<Models.Entities.Notification>(notification);
            await _context.Notifications.AddAsync(notification1x);
            _context.StudentRequests.Update(studentRequest);
            await _service.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }

        // Xóa yêu cầu của học sinh
        public async Task<IActionResult> DeleteStudentRequest(Guid id)
        {
            var studentRequest = _context.StudentRequests.FirstOrDefault(x => x.StudentRequestId == id);
            if (studentRequest == null)
            {
                return new StatusCodeResult(404);
            }
            studentRequest.Status = (Int32)StudentRequestEnum.Accepted;
            await _service.SetAsync<StudentRequest>($"Studentrequest/{studentRequest.StudentRequestId}", studentRequest);
            _context.StudentRequests.Update(studentRequest);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<ActionResult<List<StudentRequest>>> GetAllStudentRequests()
        {
            try
            {
                var studentRequests = await _context.StudentRequests.ToListAsync();
                if (studentRequests == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<StudentRequest>> GetStudentRequest(Guid id)
        {
            try
            {
                var studentRequest = await _context.StudentRequests.FirstOrDefaultAsync(c => c.StudentRequestId == id);
                if (studentRequest == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsByStudentId(Guid id)
        {
            try
            {
                var studentRequests = await _context.StudentRequests.Where(c => c.StudentId == id).ToListAsync();
                if (studentRequests == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsBySubjectId(Guid id)
        {
            try
            {
                var studentRequests = await _context.StudentRequests.Where(c => c.SubjectId == id).ToListAsync();
                if (studentRequests == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<StudentRequestView>>> GetStudentRequestsByStatus()
        {
            try
            {
                // Lấy dữ liệu
                var studentRequestsData = await _service.GetAsync<Dictionary<string, Dictionary<string, object>>>("Studentrequest");

                // Kiểm tra dữ liệu null
                if (studentRequestsData == null || !studentRequestsData.Any())
                {
                    throw new CrudException(System.Net.HttpStatusCode.OK, "No student requests found", "");
                }

                var studentRequests = new List<StudentRequestView>();

                // Duyệt qua từng mục trong dữ liệu
                foreach (var item in studentRequestsData)
                {
                    var studentRequestObject = item.Value;

                    var studentRequestView = new StudentRequestView
                    {
                        StudentRequestId = Guid.Parse(item.Key), // Dùng item.Key cho StudentRequestId
                        StudentId = Guid.Parse(studentRequestObject["StudentId"].ToString()),
                        SubjectId = Guid.Parse(studentRequestObject["SubjectId"].ToString()),
                        CreatedAt = DateTime.Parse(studentRequestObject["CreatedAt"].ToString()),
                        Message = studentRequestObject["Message"].ToString(),
                        Status = int.Parse(studentRequestObject["Status"].ToString())
                    };

                    studentRequests.Add(studentRequestView);
                }

                return new ActionResult<List<StudentRequestView>>(studentRequests);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine("Error: " + ex.Message);
                throw new CrudException(System.Net.HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }
        public async Task<ActionResult<PageResults<StudentRequest>>> GetStudentRequestsByStudentIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var studentRequests = await _context.StudentRequests
                    .Where(c => c.StudentId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (studentRequests == null || !studentRequests.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedStudentRequests = PagingHelper<StudentRequest>.Paging(studentRequests, request.Page, request.PageSize);
                if (paginatedStudentRequests == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedStudentRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<StudentRequest>>> GetStudentRequestsBySubjectIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var studentRequests = await _context.StudentRequests
                    .Where(c => c.SubjectId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (studentRequests == null || !studentRequests.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedStudentRequests = PagingHelper<StudentRequest>.Paging(studentRequests, request.Page, request.PageSize);
                if (paginatedStudentRequests == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedStudentRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<StudentRequestView>>> GetStudentRequestsByStatusPaging(PagingRequest request)
        {
            try
            {
                // Retrieve data
                var studentRequestsData = await _service.GetAsync<Dictionary<string, Dictionary<string, object>>>("Studentrequest");

                if (studentRequestsData == null || !studentRequestsData.Any())
                {
                    throw new CrudException(HttpStatusCode.OK, "No student requests found", "");
                }

                var studentRequests = new List<StudentRequestView>();

                foreach (var item in studentRequestsData)
                {
                    var studentRequestObject = item.Value;

                    var studentRequestView = new StudentRequestView
                    {
                        StudentRequestId = Guid.Parse(item.Key),
                        StudentId = Guid.Parse(studentRequestObject["StudentId"].ToString()),
                        SubjectId = Guid.Parse(studentRequestObject["SubjectId"].ToString()),
                        CreatedAt = DateTime.Parse(studentRequestObject["CreatedAt"].ToString()),
                        Message = studentRequestObject["Message"].ToString(),
                        Status = int.Parse(studentRequestObject["Status"].ToString())
                    };

                    studentRequests.Add(studentRequestView);
                }

                var paginatedStudentRequests = PagingHelper<StudentRequestView>.Paging(studentRequests, request.Page, request.PageSize);
                if (paginatedStudentRequests == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedStudentRequests;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        public async Task<ActionResult<PageResults<StudentRequest>>> GetAllStudentRequestsPaging(PagingRequest request)
        {
            try
            {
                var studentRequests = await _context.StudentRequests
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (studentRequests == null || !studentRequests.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedStudentRequests = PagingHelper<StudentRequest>.Paging(studentRequests, request.Page, request.PageSize);
                if (paginatedStudentRequests == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedStudentRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Tutor List Based On Subject
        public async Task<ActionResult<List<TutorView>>> GetListTutorBasedOnSubject(List<Guid> subjectId)
        {
            try
            {
                var tutorListView = _context.TutorSubjects
                    .Where(ts => subjectId.Contains(ts.SubjectId))
                    .Select(ts => new TutorView
                    {
                        TutorId = ts.TutorId,
                        UserId = ts.TutorNavigation.UserId,
                        IdentityNumber = ts.TutorNavigation.IdentityNumber,
                        PricePerHour = ts.TutorNavigation.PricePerHour,
                        Description = ts.TutorNavigation.Description,
                        Status = ts.TutorNavigation.Status,
                        CreateAt = ts.TutorNavigation.CreateAt,
                        UpdateAt = ts.TutorNavigation.UpdateAt,
                        VideoUrl = ts.TutorNavigation.VideoUrl
                    }).ToList();
                if (tutorListView.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.NoContent, "No Tutor Found", "");
                }
                return tutorListView;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get List Tutor Based On Subject Error", ex.Message);
            }
        }

        // Get Student Request Based On Tutor has bought subscription
        public async Task<ActionResult<List<StudentRequestView>>> GetStudentRequestBasedOnTutorHasBoughtSubscription(Guid tutorId)
        {
            try
            {
                var tutor = _context.Tutors.FirstOrDefault(t => t.TutorId == tutorId);
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor Not Found", "");
                }

                var tutorSubjects = _context.TutorSubjects.Where(ts => ts.TutorId == tutorId).Select(ts => ts.SubjectId).ToList();
                var studentRequests = _context.StudentRequests
                    .Where(sr => tutorSubjects.Contains(sr.SubjectId) && sr.Status == (int)StudentRequestEnum.Pending)
                    .ToList();

                if (studentRequests.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.NoContent, "No Student Request Found", "");
                }

                List<StudentRequestView> studentRequestViews = new List<StudentRequestView>();

                if (tutor.HasBoughtSubscription == false)
                {
                    // Renew studentRequest list after 00:00 am everyday and have 10 latest items based on created date
                    var studentRequest = studentRequests.OrderByDescending(sr => sr.CreatedAt).Take(10).ToList();
                    foreach (var item in studentRequest)
                    {
                        var studentRequestView1 = new StudentRequestView
                        {
                            StudentRequestId = item.StudentRequestId,
                            StudentId = item.StudentId,
                            SubjectId = item.SubjectId,
                            CreatedAt = item.CreatedAt,
                            Message = item.Message,
                            Status = item.Status
                        };
                        studentRequestViews.Add(studentRequestView1);
                    }
                }
                else if (tutor.HasBoughtSubscription == true)
                {   
                    var studentRequestVip = _context.StudentRequests
                    .Where(sr => sr.Status == (Int32)StudentRequestEnum.Pending)
                    .ToList();
                    foreach (var item in studentRequestVip)
                    {
                        var studentRequestView = new StudentRequestView
                        {
                            StudentRequestId = item.StudentRequestId,
                            StudentId = item.StudentId,
                            SubjectId = item.SubjectId,
                            CreatedAt = item.CreatedAt,
                            Message = item.Message,
                            Status = item.Status
                        };
                        studentRequestViews.Add(studentRequestView);
                    }
                }
                return studentRequestViews;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Student Request Based On Tutor Has Bought Subscription Error", ex.Message);
            }
        }

        // View StudentRequest Paging Based On Tutor has bought subscription
        public async Task<ActionResult<PageResults<StudentRequestView>>> ViewStudentRequestPagingBasedOnTutorHasBoughtSubscription(Guid tutorId, PagingRequest request)
        {
            try
            {
                var tutor = _context.Tutors.FirstOrDefault(t => t.TutorId == tutorId);
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor Not Found", "");
                }

                var tutorSubjects = _context.TutorSubjects.Where(ts => ts.TutorId == tutorId).Select(ts => ts.SubjectId).ToList();
                var studentRequests = _context.StudentRequests
                    .Where(sr => tutorSubjects.Contains(sr.SubjectId) && sr.Status == (int)StudentRequestEnum.Pending)
                    .ToList();
                if (studentRequests.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.NoContent, "No Student Request Found", "");
                }

                List<StudentRequestView> studentRequestViews = new List<StudentRequestView>();

                if (tutor.SubcriptionType == 0)
                {
                    // Renew studentRequest list after 00:00 am everyday and have 10 latest items based on created date
                    var studentRequest = studentRequests.OrderByDescending(sr => sr.CreatedAt).Take(10).ToList();
                    foreach (var item in studentRequest)
                    {
                        var studentRequestView1 = new StudentRequestView
                        {
                            StudentRequestId = item.StudentRequestId,
                            StudentId = item.StudentId,
                            SubjectId = item.SubjectId,
                            CreatedAt = item.CreatedAt,
                            Message = item.Message,
                            Status = item.Status
                        };
                        studentRequestViews.Add(studentRequestView1);
                    }
                }
                else if (tutor.SubcriptionType == 2 || tutor.SubcriptionType == 1)
                {
                    var studentRequestVip = _context.StudentRequests
                    .Where(sr => sr.Status == (Int32)StudentRequestEnum.Pending)
                    .ToList();
                    foreach (var item in studentRequestVip)
                    {
                        var studentRequestView = new StudentRequestView
                        {
                            StudentRequestId = item.StudentRequestId,
                            StudentId = item.StudentId,
                            SubjectId = item.SubjectId,
                            CreatedAt = item.CreatedAt,
                            Message = item.Message,
                            Status = item.Status
                        };
                        studentRequestViews.Add(studentRequestView);
                    }
                }
                var paginatedStudentRequests = PagingHelper<StudentRequestView>.Paging(studentRequestViews, request.Page, request.PageSize);
                if (paginatedStudentRequests == null)
                {
                    return new StatusCodeResult(400);
                }
                return paginatedStudentRequests;
            } catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
