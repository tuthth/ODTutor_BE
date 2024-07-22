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
    public class StudentCourseService : BaseService, IStudentCourseService
    {
        public StudentCourseService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> UpdateStudentCourse(UpdateStudentCourseRequest request)
        {
            var studentCourse = _context.StudentCourses.FirstOrDefault(c => c.StudentCourseId == request.StudentCourseId);
            if (studentCourse == null)
            {
                return new StatusCodeResult(404);
            }
            _mapper.Map(request, studentCourse);
            _context.StudentCourses.Update(studentCourse);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> FinishStudentCourse(Guid studentCourseId)
        {

           var studentCourse = _context.StudentCourses.FirstOrDefault(c => c.StudentCourseId == studentCourseId);
            if (studentCourse == null)
            {
                return new StatusCodeResult(404);
            }
            if(studentCourse.Status != (Int32)CourseEnum.Active)
            {
                return new StatusCodeResult(409);
            }
            studentCourse.Status = (Int32)CourseEnum.Finished;
            _context.StudentCourses.Update(studentCourse);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<ActionResult<List<StudentCourse>>> GetAllStudentCourses()
        {
            try
            {
                var studentCourses = await _context.StudentCourses.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (studentCourses == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentCourses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<StudentCourse>> GetStudentCourse(Guid id)
        {
            try
            {
                var studentCourse = await _context.StudentCourses.FirstOrDefaultAsync(c => c.StudentCourseId == id);
                if (studentCourse == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentCourse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByCourseId(Guid id)
        {
            try
            {
                var studentCourses = await _context.StudentCourses.Where(c => c.CourseId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (studentCourses == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentCourses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByStudentId(Guid id)
        {
            try
            {
                var studentCourses = await _context.StudentCourses.Where(c => c.StudentId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (studentCourses == null)
                {
                    return new StatusCodeResult(404);
                }
                return studentCourses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
