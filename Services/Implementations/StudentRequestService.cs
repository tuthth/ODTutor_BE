using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class StudentRequestService : BaseService, IStudentRequestService
    {
        public StudentRequestService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> CreateStudentRequest(CreateStudentRequest request)
        {
            var student = _context.Students.FirstOrDefault(x => x.StudentId == request.StudentId && x.UserNavigation.IsPremium == true);
            var subject = _context.Subjects.FirstOrDefault(x => x.SubjectId == request.SubjectId);
            if (student == null || subject == null)
            {
                return new StatusCodeResult(404);
            }
            if(student.UserNavigation.Banned == true)
            {
                return new StatusCodeResult(403);
            }
            var studentRequest = _mapper.Map<StudentRequest>(request);
            studentRequest.CreatedAt = DateTime.UtcNow;
            studentRequest.StudentRequestId = Guid.NewGuid();
            studentRequest.Status = (Int32)StudentRequestEnum.Pending;
            _context.StudentRequests.Add(studentRequest);
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
    }
}
