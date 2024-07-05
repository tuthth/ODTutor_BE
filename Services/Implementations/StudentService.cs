using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class StudentService : BaseService, IStudentService
    {
        private readonly IFirebaseRealtimeDatabaseService _firebaseService;
        public StudentService(ODTutorContext context, IMapper mapper, IFirebaseRealtimeDatabaseService firebaseService) : base(context, mapper)
        {
            _firebaseService = firebaseService;
        }
        public async Task<ActionResult<List<Student>>> GetAllStudents()
        {
            try
            {
                var students = await _context.Students.OrderByDescending(c => c.UserNavigation.CreatedAt).ToListAsync();
                if (students == null)
                {
                    return new StatusCodeResult(404);
                }
                return students;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<Student>>> GetAllStudentsPaging(PagingRequest request)
        {
            try
            {
                var studentsList = await _context.Students.OrderByDescending(c => c.UserNavigation.CreatedAt).ToListAsync();
                if (studentsList == null || !studentsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedStudents = PagingHelper<Student>.Paging(studentsList, request.Page, request.PageSize);
                if (paginatedStudents == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedStudents;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Student>> GetStudent(Guid id)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(c => c.StudentId == id);
                if (student == null)
                {
                    return new StatusCodeResult(404);
                }
                return student;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByStudentId(Guid id)
        {
            try
            {
                var tutorRatings = await _context.TutorRatings.Where(c => c.StudentId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorRatings == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
