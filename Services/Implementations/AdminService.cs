using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using System.Web.Mvc;

namespace Services.Implementations
{
    public class AdminService : BaseService, IAdminService
    {
        public AdminService(ODTutorContext odContext, IMapper mapper) : base(odContext, mapper)
        {
        }
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (users == null)
                {
                    return new StatusCodeResult(404);
                }
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<User>>> GetAllUsersPaging(PagingRequest request)
        {
            try
            {
                var usersList = await _context.Users.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (usersList == null || !usersList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedUsers = PagingHelper<User>.Paging(usersList, request.Page, request.PageSize);
                if (paginatedUsers == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedUsers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
                if (user == null)
                {
                    return new StatusCodeResult(404);
                }
                return user;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
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
        public async Task<ActionResult<List<Tutor>>> GetAllTutors()
        {
            try
            {
                var tutors = await _context.Tutors.OrderByDescending(c => c.UserNavigation.CreatedAt).ToListAsync();
                if (tutors == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutors;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Tutor>> GetTutor(Guid id)
        {
            try
            {
                var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == id);
                if (tutor == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Subject>>> GetAllSubjects()
        {
            try
            {
                var subjects = await _context.Subjects.ToListAsync();
                if (subjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return subjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Subject>> GetSubject(Guid id)
        {
            try
            {
                var subject = await _context.Subjects.FirstOrDefaultAsync(c => c.SubjectId == id);
                if (subject == null)
                {
                    return new StatusCodeResult(404);
                }
                return subject;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorCertificate>>> GetAllTutorCertificates()
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.ToListAsync();
                if (tutorCertificates == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificates;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorCertificate>> GetTutorCertificate(Guid id)
        {
            try
            {
                var tutorCertificate = await _context.TutorCertificates.FirstOrDefaultAsync(c => c.TutorCertificateId == id);
                if (tutorCertificate == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByTutorId(Guid id)
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.Where(c => c.TutorId == id).ToListAsync();
                if (tutorCertificates == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificates;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesBySubjectId(Guid id)
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.Where(c => c.TutorId == id).ToListAsync();
                if (tutorCertificates == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificates;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorSubject>>> GetAllTutorSubjects()
        {
            try
            {
                var tutorSubjects = await _context.TutorSubjects.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorSubjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorSubject>> GetTutorSubject(Guid id)
        {
            try
            {
                var tutorSubject = await _context.TutorSubjects.FirstOrDefaultAsync(c => c.TutorSubjectId == id);
                if (tutorSubject == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubject;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsByTutorId(Guid id)
        {
            try
            {
                var tutorSubjects = await _context.TutorSubjects.Where(c => c.TutorId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorSubjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsBySubjectId(Guid id)
        {
            try
            {
                var tutorSubjects = await _context.TutorSubjects.Where(c => c.SubjectId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorSubjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorRating>>> GetAllTutorRatings()
        {
            try
            {
                var tutorRatings = await _context.TutorRatings.OrderByDescending(c => c.CreatedAt).ToListAsync();
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
        public async Task<ActionResult<TutorRating>> GetTutorRating(Guid id)
        {
            try
            {
                var tutorRating = await _context.TutorRatings.FirstOrDefaultAsync(c => c.TutorRatingId == id);
                if (tutorRating == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRating;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByTutorId(Guid id)
        {
            try
            {
                var tutorRatings = await _context.TutorRatings.Where(c => c.TutorId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
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
        public async Task<ActionResult<List<TutorRatingImage>>> GetAllTutorRatingImages()
        {
            try
            {
                var tutorRatingImages = await _context.TutorRatingImages.ToListAsync();
                if (tutorRatingImages == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatingImages;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorRatingImage>> GetTutorRatingImage(Guid id)
        {
            try
            {
                var tutorRatingImage = await _context.TutorRatingImages.FirstOrDefaultAsync(c => c.TutorRatingImageId == id);
                if (tutorRatingImage == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatingImage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<ActionResult<List<Moderator>>> GetModerators()
        {
            try
            {
                var moderators = await _context.Moderators.OrderByDescending(c => c.UserNavigation.CreatedAt).ToListAsync();
                if (moderators == null)
                {
                    return new StatusCodeResult(404);
                }
                return moderators;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Moderator>> GetModeratorById(Guid id)
        {
            try
            {
                var moderator = await _context.Moderators.FirstOrDefaultAsync(c => c.ModeratorId == id || c.UserId == id );
                if (moderator == null)
                {
                    return new StatusCodeResult(404);
                }
                return moderator;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Notification>>> GetNotifications()
        {
            try
            {
                var notifications = await _context.Notifications.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (notifications == null)
                {
                    return new StatusCodeResult(404);
                }
                return notifications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Notification>>> GetNotificationsByUserId(Guid id)
        {
            try
            {
                var notifications = await _context.Notifications.Where(c => c.UserId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (notifications == null)
                {
                    return new StatusCodeResult(404);
                }
                return notifications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
       
    }
}
