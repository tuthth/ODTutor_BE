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
        public StudentRequestService(ODTutorContext context, IMapper mapper, INotificationService notificationService , IFirebaseRealtimeDatabaseService service) : base(context, mapper)
        {
            _service = service;
            _notificationService = notificationService;
        }
        public async Task<IActionResult> CreateStudentRequest(CreateStudentRequest request)
        {
            Student student = _context.Students.FirstOrDefault(x => x.StudentId == request.StudentId);
            var subject = _context.Subjects.FirstOrDefault(x => x.SubjectId == request.SubjectId);
            if (student == null || subject == null)
            {
                return new StatusCodeResult(404);
            }
            var studentRequest = _mapper.Map<StudentRequest>(request);
            studentRequest.CreatedAt = DateTime.UtcNow.AddHours(7);
            studentRequest.StudentRequestId = Guid.NewGuid();
            studentRequest.Status = (Int32)StudentRequestEnum.Pending;
            var notification = new Models.Entities.Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Yêu cầu của bạn đã được gửi đến các gia sư khác",
                Content = "Vui lòng chờ trong thời gian sớm nhất để nhận được phản hồi từ các gia sư khác trong tin nhắn nhé",
                UserId = student.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (Int32)NotificationEnum.UnRead
            };
            await _service.SetAsync<Models.Entities.Notification>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _service.SetAsync<StudentRequest>($"Studentrequest/{studentRequest.StudentRequestId}", studentRequest);
            await _context.Notifications.AddAsync(notification);
            await _context.StudentRequests.AddAsync(studentRequest);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateStudentRequest(UpdateStudentRequest request)
        {
            var studentRequest = _context.StudentRequests.FirstOrDefault(x => x.StudentRequestId == request.StudentRequestId);
            var student = _context.Students.FirstOrDefault(x => x.StudentId == request.StudentId);
            var subject = _context.Subjects.FirstOrDefault(x => x.SubjectId == request.SubjectId);
            if (student == null || subject == null)
            {
                return new StatusCodeResult(406);
            }
            if (student.UserNavigation.Banned == true)
            {
                return new StatusCodeResult(403);
            }
            if (studentRequest == null)
            {
                return new StatusCodeResult(404);
            }
            if (studentRequest.Status != (Int32)StudentRequestEnum.Pending)
            {
                return new StatusCodeResult(409);
            }
            studentRequest.Message = request.Message;
            studentRequest.Status = request.Status;
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
                    throw new CrudException(System.Net.HttpStatusCode.NotFound, "No student requests found", "");
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
                    throw new CrudException(HttpStatusCode.NotFound, "No student requests found", "");
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



    }
}
