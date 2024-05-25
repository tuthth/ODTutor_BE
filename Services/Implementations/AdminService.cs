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
                var users = await _context.Users.ToListAsync();
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
                var students = await _context.Students.ToListAsync();
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
                var tutors = await _context.Tutors.ToListAsync();
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
        public async Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions()
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid id)
        {
            try
            {
                var courseTransaction = await _context.CourseTransactions.FirstOrDefaultAsync(c => c.CourseTransactionId == id);
                if (courseTransaction == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderId(Guid id)
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.Where(c => c.SenderWalletId == id).ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverId(Guid id)
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.Where(c => c.ReceiverWalletId == id).ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseId(Guid id)
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.Where(c => c.CourseId == id).ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
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
        public async Task<ActionResult<List<Schedule>>> GetAllSchedules()
        {
            try
            {
                var schedules = await _context.Schedules.ToListAsync();
                if (schedules == null)
                {
                    return new StatusCodeResult(404);
                }
                return schedules;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Schedule>> GetSchedule(Guid id)
        {
            try
            {
                var schedule = await _context.Schedules.FirstOrDefaultAsync(c => c.ScheduleId == id);
                if (schedule == null)
                {
                    return new StatusCodeResult(404);
                }
                return schedule;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Schedule>>> GetSchedulesByStudentCourseId(Guid id)
        {
            try
            {
                var schedules = await _context.Schedules.Where(c => c.StudentCourseId == id).ToListAsync();
                if (schedules == null)
                {
                    return new StatusCodeResult(404);
                }
                return schedules;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<StudentCourse>>> GetAllStudentCourses()
        {
            try
            {
                var studentCourses = await _context.StudentCourses.ToListAsync();
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
                var studentCourses = await _context.StudentCourses.Where(c => c.CourseId == id).ToListAsync();
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
                var studentCourses = await _context.StudentCourses.Where(c => c.StudentId == id).ToListAsync();
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
        public async Task<ActionResult<List<TutorSubject>>> GetAllTutorSubjects()
        {
            try
            {
                var tutorSubjects = await _context.TutorSubjects.ToListAsync();
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
                var tutorSubjects = await _context.TutorSubjects.Where(c => c.TutorId == id).ToListAsync();
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
                var tutorSubjects = await _context.TutorSubjects.Where(c => c.SubjectId == id).ToListAsync();
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
                var tutorRatings = await _context.TutorRatings.ToListAsync();
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
                var tutorRatings = await _context.TutorRatings.Where(c => c.TutorId == id).ToListAsync();
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
                var tutorRatings = await _context.TutorRatings.Where(c => c.StudentId == id).ToListAsync();
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
        public async Task<ActionResult<List<TutorSchedule>>> GetAllTutorSchedules()
        {
            try
            {
                var tutorSchedules = await _context.TutorSchedules.ToListAsync();
                if (tutorSchedules == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSchedules;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorSchedule>> GetTutorSchedule(Guid id)
        {
            try
            {
                var tutorSchedule = await _context.TutorSchedules.FirstOrDefaultAsync(c => c.TutorScheduleId == id);
                if (tutorSchedule == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSchedule;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorSchedule>>> GetTutorSchedulesByTutorId(Guid id)
        {
            try
            {
                var tutorSchedules = await _context.TutorSchedules.Where(c => c.TutorId == id).ToListAsync();
                if (tutorSchedules == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSchedules;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Booking>>> GetAllBookings()
        {
            try
            {
                var bookings = await _context.Bookings.ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Booking>> GetBooking(Guid id)
        {
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(c => c.BookingId == id);
                if (booking == null)
                {
                    return new StatusCodeResult(404);
                }
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Booking>>> GetBookingsByStudentId(Guid id)
        {
            try
            {
                var bookings = await _context.Bookings.Where(c => c.StudentId == id).ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Booking>>> GetBookingsByTutorId(Guid id)
        {
            try
            {
                var bookings = await _context.Bookings.Where(c => c.TutorId == id).ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserBlock>>> GetAllUserBlocks()
        {
            try
            {
                var userBlocks = await _context.UserBlocks.ToListAsync();
                if (userBlocks == null)
                {
                    return new StatusCodeResult(404);
                }
                return userBlocks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserBlock>>> GetAllBlockByCreateUserId(Guid id)
        {
            try
            {
                var userBlocks = await _context.UserBlocks.Where(c => c.CreateUserId == id).ToListAsync();
                if (userBlocks == null)
                {
                    return new StatusCodeResult(404);
                }
                return userBlocks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserBlock>>> GetAllBlockByTargetUserId(Guid id)
        {
            try
            {
                var userBlocks = await _context.UserBlocks.Where(c => c.TargetUserId == id).ToListAsync();
                if (userBlocks == null)
                {
                    return new StatusCodeResult(404);
                }
                return userBlocks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserFollow>>> GetAllUserFollows()
        {
            try
            {
                var userFollows = await _context.UserFollows.ToListAsync();
                if (userFollows == null)
                {
                    return new StatusCodeResult(404);
                }
                return userFollows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserFollow>>> GetAllFollowsByCreateUserId(Guid id)
        {
            try
            {
                var userFollows = await _context.UserFollows.Where(c => c.CreateUserId == id).ToListAsync();
                if (userFollows == null)
                {
                    return new StatusCodeResult(404);
                }
                return userFollows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<UserFollow>>> GetAllFollowsByTargetUserId(Guid id)
        {
            try
            {
                var userFollows = await _context.UserFollows.Where(c => c.TargetUserId == id).ToListAsync();
                if (userFollows == null)
                {
                    return new StatusCodeResult(404);
                }
                return userFollows;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Wallet>>> GetAllWallets()
        {
            try
            {
                var wallets = await _context.Wallets.ToListAsync();
                if (wallets == null)
                {
                    return new StatusCodeResult(404);
                }
                return wallets;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Wallet>> GetWalletByWalletId(Guid id)
        {
            try
            {
                var wallet = await _context.Wallets.FirstOrDefaultAsync(c => c.WalletId == id);
                if (wallet == null)
                {
                    return new StatusCodeResult(404);
                }
                return wallet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Wallet>> GetWalletByUserId(Guid id)
        {
            try
            {
                var wallet = await _context.Wallets.FirstOrDefaultAsync(c => c.UserId == id);
                if (wallet == null)
                {
                    return new StatusCodeResult(404);
                }
                return wallet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions()
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderId(Guid id)
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Where(c => c.SenderWalletId == id).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverId(Guid id)
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Where(c => c.ReceiverWalletId == id).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByBookingId(Guid id)
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Where(c => c.BookingId == id).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions()
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByWalletTransactionId(Guid id)
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.Where(c => c.WalletTransactionId == id).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderId(Guid id)
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.Where(c => c.SenderWalletId == id).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverId(Guid id)
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.Where(c => c.ReceiverWalletId == id).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
