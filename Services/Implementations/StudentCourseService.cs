using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
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
