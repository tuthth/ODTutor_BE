﻿using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpGet("get/users")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var result = await _adminService.GetAllUsers();
            if (result is ActionResult<List<User>> users) return Ok(users);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy người dùng"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user/{userID}")]
        public async Task<ActionResult<User>> GetUser(Guid userID)
        {
            var result = await _adminService.GetUser(userID);
            if (result is ActionResult<User> user) return Ok(user);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy người dùng"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/students")]
        public async Task<ActionResult<List<Student>>> GetAllStudents()
        {
            var result = await _adminService.GetAllStudents();
            if (result is ActionResult<List<Student>> students) return Ok(students);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy sinh viên"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student/{studentID}")]
        public async Task<ActionResult<Student>> GetStudent(Guid studentID)
        {
            var result = await _adminService.GetStudent(studentID);
            if (result is ActionResult<Student> student) return Ok(student);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy sinh viên"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutors")]
        public async Task<ActionResult<List<Tutor>>> GetAllTutors()
        {
            var result = await _adminService.GetAllTutors();
            if (result is ActionResult<List<Tutor>> tutors) return Ok(tutors);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy gia sư"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor/{tutorID}")]
        public async Task<ActionResult<Tutor>> GetTutor(Guid tutorID)
        {
            var result = await _adminService.GetTutor(tutorID);
            if (result is ActionResult<Tutor> tutor) return Ok(tutor);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy gia sư"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/subjects")]
        public async Task<ActionResult<List<Subject>>> GetAllSubjects()
        {
            var result = await _adminService.GetAllSubjects();
            if (result is ActionResult<List<Subject>> subjects) return Ok(subjects);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/subject/{subjectID}")]
        public async Task<ActionResult<Subject>> GetSubject(Guid subjectID)
        {
            var result = await _adminService.GetSubject(subjectID);
            if (result is ActionResult<Subject> subject) return Ok(subject);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/courses")]
        public async Task<ActionResult<List<Course>>> GetAllCourses()
        {
            var result = await _adminService.GetAllCourses();
            if (result is ActionResult<List<Course>> courses) return Ok(courses);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course/{courseID}")]
        public async Task<ActionResult<Course>> GetCourse(Guid courseID)
        {
            var result = await _adminService.GetCourse(courseID);
            if (result is ActionResult<Course> course) return Ok(course);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa học"); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-outlines")]
        public async Task<ActionResult<List<CourseOutline>>> GetAllCourseOutlines()
        {
            var result = await _adminService.GetAllCourseOutlines();
            if (result is ActionResult<List<CourseOutline>> courseOutlines) return Ok(courseOutlines);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy chương trình học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-outline/{courseOutlineID}")]
        public async Task<ActionResult<CourseOutline>> GetCourseOutline(Guid courseOutlineID)
        {
            var result = await _adminService.GetCourseOutline(courseOutlineID);
            if (result is ActionResult<CourseOutline> courseOutline) return Ok(courseOutline);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy chương trình học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-promotions")]
        public async Task<ActionResult<List<CoursePromotion>>> GetAllCoursePromotions()
        {
            var result = await _adminService.GetAllCoursePromotions();
            if (result is ActionResult<List<CoursePromotion>> coursePromotions) return Ok(coursePromotions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khuyến mãi");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-promotion/{coursePromotionID}")]
        public async Task<ActionResult<CoursePromotion>> GetCoursePromotion(Guid coursePromotionID)
        {
            var result = await _adminService.GetCoursePromotion(coursePromotionID);
            if (result is ActionResult<CoursePromotion> coursePromotion) return Ok(coursePromotion);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khuyến mãi");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions")]
        public async Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions()
        {
            var result = await _adminService.GetAllCourseTransactions();
            if (result is ActionResult<List<CourseTransaction>> courseTransactions) return Ok(courseTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch khóa học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transaction/{courseTransactionID}")]
        public async Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid courseTransactionID)
        {
            var result = await _adminService.GetCourseTransaction(courseTransactionID);
            if (result is ActionResult<CourseTransaction> courseTransaction) return Ok(courseTransaction);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch khóa học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/{senderID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderID(Guid senderID)
        {
            var result = await _adminService.GetCourseTransactionsBySenderId(senderID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions) return Ok(courseTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch khóa học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/{receiverID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _adminService.GetCourseTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions) return Ok(courseTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch khóa học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/{courseID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseID(Guid courseID)
        {
            var result = await _adminService.GetCourseTransactionsByCourseId(courseID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions) return Ok(courseTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch khóa học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/promotions")]
        public async Task<ActionResult<List<Promotion>>> GetAllPromotions()
        {
            var result = await _adminService.GetAllPromotions();
            if (result is ActionResult<List<Promotion>> promotions) return Ok(promotions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khuyến mãi");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/promotion/{promotionID}")]
        public async Task<ActionResult<Promotion>> GetPromotion(Guid promotionID)
        {
            var result = await _adminService.GetPromotion(promotionID);
            if (result is ActionResult<Promotion> promotion) return Ok(promotion);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khuyến mãi");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/reports")]
        public async Task<ActionResult<List<Report>>> GetAllReports()
        {
            var result = await _adminService.GetAllReports();
            if (result is ActionResult<List<Report>> reports) return Ok(reports);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy báo cáo");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/report/{reportID}")]
        public async Task<ActionResult<Report>> GetReport(Guid reportID)
        {
            var result = await _adminService.GetReport(reportID);
            if (result is ActionResult<Report> report) return Ok(report);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy báo cáo");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/reports/create/{userID}")]
        public async Task<ActionResult<List<Report>>> GetReportsByUserID(Guid userID)
        {
            var result = await _adminService.GetReportsByUserId(userID);
            if (result is ActionResult<List<Report>> reports) return Ok(reports);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy báo cáo");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/reports/target/{reporterID}")]
        public async Task<ActionResult<List<Report>>> GetReportsByReporterID(Guid reporterID)
        {
            var result = await _adminService.GetReportsByReporterId(reporterID);
            if (result is ActionResult<List<Report>> reports) return Ok(reports);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy báo cáo");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/schedules")]
        public async Task<ActionResult<List<Schedule>>> GetAllSchedules()
        {
            var result = await _adminService.GetAllSchedules();
            if (result is ActionResult<List<Schedule>> schedules) return Ok(schedules);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/schedule/{scheduleID}")]
        public async Task<ActionResult<Schedule>> GetSchedule(Guid scheduleID)
        {
            var result = await _adminService.GetSchedule(scheduleID);
            if (result is ActionResult<Schedule> schedule) return Ok(schedule);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/schedules/student-course/{studentCourseID}")]
        public async Task<ActionResult<List<Schedule>>> GetSchedulesByStudentCourseID(Guid studentCourseID)
        {
            var result = await _adminService.GetSchedulesByStudentCourseId(studentCourseID);
            if (result is ActionResult<List<Schedule>> schedules) return Ok(schedules);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch học");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-courses")]
        public async Task<ActionResult<List<StudentCourse>>> GetAllStudentCourses()
        {
            var result = await _adminService.GetAllStudentCourses();
            if (result is ActionResult<List<StudentCourse>> studentCourses) return Ok(studentCourses);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khóa học sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-course/{studentCourseID}")]
        public async Task<ActionResult<StudentCourse>> GetStudentCourse(Guid studentCourseID)
        {
            var result = await _adminService.GetStudentCourse(studentCourseID);
            if (result is ActionResult<StudentCourse> studentCourse) return Ok(studentCourse);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khóa học sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-courses/course/{courseID}")]
        public async Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByCourseID(Guid courseID)
        {
            var result = await _adminService.GetStudentCoursesByCourseId(courseID);
            if (result is ActionResult<List<StudentCourse>> studentCourses) return Ok(studentCourses);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khóa học sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-courses/student/{studentID}")]
        public async Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByStudentID(Guid studentID)
        {
            var result = await _adminService.GetStudentCoursesByStudentId(studentID);
            if (result is ActionResult<List<StudentCourse>> studentCourses) return Ok(studentCourses);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khóa học sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-requests")]
        public async Task<ActionResult<List<StudentRequest>>> GetAllStudentRequests()
        {
            var result = await _adminService.GetAllStudentRequests();
            if (result is ActionResult<List<StudentRequest>> studentRequests) return Ok(studentRequests);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy yêu cầu sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-request/{studentRequestID}")]
        public async Task<ActionResult<StudentRequest>> GetStudentRequest(Guid studentRequestID)
        {
            var result = await _adminService.GetStudentRequest(studentRequestID);
            if (result is ActionResult<StudentRequest> studentRequest) return Ok(studentRequest);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy yêu cầu sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-requests/student/{studentID}")]
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsByStudentID(Guid studentID)
        {
            var result = await _adminService.GetStudentRequestsByStudentId(studentID);
            if (result is ActionResult<List<StudentRequest>> studentRequests) return Ok(studentRequests);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy yêu cầu sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-requests/subject/{subjectID}")]
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsBySubjectID(Guid subjectID)
        {
            var result = await _adminService.GetStudentRequestsBySubjectId(subjectID);
            if (result is ActionResult<List<StudentRequest>> studentRequests) return Ok(studentRequests);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy yêu cầu sinh viên");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificates")]
        public async Task<ActionResult<List<TutorCertificate>>> GetAllTutorCertificates()
        {
            var result = await _adminService.GetAllTutorCertificates();
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates) return Ok(tutorCertificates);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy chứng chỉ gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificate/{tutorCertificateID}")]
        public async Task<ActionResult<TutorCertificate>> GetTutorCertificate(Guid tutorCertificateID)
        {
            var result = await _adminService.GetTutorCertificate(tutorCertificateID);
            if (result is ActionResult<TutorCertificate> tutorCertificate) return Ok(tutorCertificate);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy chứng chỉ gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificates/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorCertificatesByTutorId(tutorID);
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates) return Ok(tutorCertificates);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy chứng chỉ gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subjects")]
        public async Task<ActionResult<List<TutorSubject>>> GetAllTutorSubjects()
        {
            var result = await _adminService.GetAllTutorSubjects();
            if (result is ActionResult<List<TutorSubject>> tutorSubjects) return Ok(tutorSubjects);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy môn học gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subject/{tutorSubjectID}")]
        public async Task<ActionResult<TutorSubject>> GetTutorSubject(Guid tutorSubjectID)
        {
            var result = await _adminService.GetTutorSubject(tutorSubjectID);
            if (result is ActionResult<TutorSubject> tutorSubject) return Ok(tutorSubject);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy môn học gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subjects/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorSubjectsByTutorId(tutorID);
            if (result is ActionResult<List<TutorSubject>> tutorSubjects) return Ok(tutorSubjects);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy môn học gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subjects/subject/{subjectID}")]
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsBySubjectID(Guid subjectID)
        {
            var result = await _adminService.GetTutorSubjectsBySubjectId(subjectID);
            if (result is ActionResult<List<TutorSubject>> tutorSubjects) return Ok(tutorSubjects);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy môn học gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-ratings")]
        public async Task<ActionResult<List<TutorRating>>> GetAllTutorRatings()
        {
            var result = await _adminService.GetAllTutorRatings();
            if (result is ActionResult<List<TutorRating>> tutorRatings) return Ok(tutorRatings);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy đánh giá gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-rating/{tutorRatingID}")]
        public async Task<ActionResult<TutorRating>> GetTutorRating(Guid tutorRatingID)
        {
            var result = await _adminService.GetTutorRating(tutorRatingID);
            if (result is ActionResult<TutorRating> tutorRating) return Ok(tutorRating);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy đánh giá gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-ratings/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorRatingsByTutorId(tutorID);
            if (result is ActionResult<List<TutorRating>> tutorRatings) return Ok(tutorRatings);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy đánh giá gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-ratings/student/{studentID}")]
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByStudentID(Guid studentID)
        {
            var result = await _adminService.GetTutorRatingsByStudentId(studentID);
            if (result is ActionResult<List<TutorRating>> tutorRatings) return Ok(tutorRatings);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy đánh giá gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-rating-images")]
        public async Task<ActionResult<List<TutorRatingImage>>> GetAllTutorRatingImages()
        {
            var result = await _adminService.GetAllTutorRatingImages();
            if (result is ActionResult<List<TutorRatingImage>> tutorRatingImages) return Ok(tutorRatingImages);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy hình ảnh đánh giá gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-rating-image/{tutorRatingImageID}")]
        public async Task<ActionResult<TutorRatingImage>> GetTutorRatingImage(Guid tutorRatingImageID)
        {
            var result = await _adminService.GetTutorRatingImage(tutorRatingImageID);
            if (result is ActionResult<TutorRatingImage> tutorRatingImage) return Ok(tutorRatingImage);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy hình ảnh đánh giá gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-schedules")]
        public async Task<ActionResult<List<TutorSchedule>>> GetAllTutorSchedules()
        {
            var result = await _adminService.GetAllTutorSchedules();
            if (result is ActionResult<List<TutorSchedule>> tutorSchedules) return Ok(tutorSchedules);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch học gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-schedule/{tutorScheduleID}")]
        public async Task<ActionResult<TutorSchedule>> GetTutorSchedule(Guid tutorScheduleID)
        {
            var result = await _adminService.GetTutorSchedule(tutorScheduleID);
            if (result is ActionResult<TutorSchedule> tutorSchedule) return Ok(tutorSchedule);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch học gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-schedules/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorSchedule>>> GetTutorSchedulesByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorSchedulesByTutorId(tutorID);
            if (result is ActionResult<List<TutorSchedule>> tutorSchedules) return Ok(tutorSchedules);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch học gia sư");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/bookings")]
        public async Task<ActionResult<List<Booking>>> GetAllBookings()
        {
            var result = await _adminService.GetAllBookings();
            if (result is ActionResult<List<Booking>> bookings) return Ok(bookings);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch đặt");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking/{bookingID}")]
        public async Task<ActionResult<Booking>> GetBooking(Guid bookingID)
        {
            var result = await _adminService.GetBooking(bookingID);
            if (result is ActionResult<Booking> booking) return Ok(booking);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch đặt");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/bookings/student/{studentID}")]
        public async Task<ActionResult<List<Booking>>> GetBookingsByStudentID(Guid studentID)
        {
            var result = await _adminService.GetBookingsByStudentId(studentID);
            if (result is ActionResult<List<Booking>> bookings) return Ok(bookings);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch đặt");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/bookings/tutor/{tutorID}")]
        public async Task<ActionResult<List<Booking>>> GetBookingsByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetBookingsByTutorId(tutorID);
            if (result is ActionResult<List<Booking>> bookings) return Ok(bookings);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy lịch đặt");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks")]
        public async Task<ActionResult<List<UserBlock>>> GetAllUserBlocks()
        {
            var result = await _adminService.GetAllUserBlocks();
            if (result is ActionResult<List<UserBlock>> userBlocks) return Ok(userBlocks);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khóa tài khoản");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-block/create/{userBlockID}")]
        public async Task<ActionResult<List<UserBlock>>> GetUserBlock(Guid userBlockID)
        {
            var result = await _adminService.GetAllBlockByCreateUserId(userBlockID);
            if (result is ActionResult<List<UserBlock>> userBlock) return Ok(userBlock);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khóa tài khoản");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks/target/{targetID}")]
        public async Task<ActionResult<List<UserBlock>>> GetUserBlocksByTargetID(Guid targetID)
        {
            var result = await _adminService.GetAllBlockByTargetUserId(targetID);
            if (result is ActionResult<List<UserBlock>> userBlocks) return Ok(userBlocks);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy khóa tài khoản");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follows")]
        public async Task<ActionResult<List<UserFollow>>> GetAllUserFollows()
        {
            var result = await _adminService.GetAllUserFollows();
            if (result is ActionResult<List<UserFollow>> userFollows) return Ok(userFollows);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy theo dõi tài khoản");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follow/create/{userFollowID}")]
        public async Task<ActionResult<List<UserFollow>>> GetUserFollow(Guid userFollowID)
        {
            var result = await _adminService.GetAllFollowsByCreateUserId(userFollowID);
            if (result is ActionResult<List<UserFollow>> userFollow) return Ok(userFollow);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy theo dõi tài khoản");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follows/target/{targetID}")]
        public async Task<ActionResult<List<UserFollow>>> GetUserFollowsByTargetID(Guid targetID)
        {
            var result = await _adminService.GetAllFollowsByTargetUserId(targetID);
            if (result is ActionResult<List<UserFollow>> userFollows) return Ok(userFollows);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy theo dõi tài khoản");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallets")]
        public async Task<ActionResult<List<Wallet>>> GetAllWallets()
        {
            var result = await _adminService.GetAllWallets();
            if (result is ActionResult<List<Wallet>> wallets) return Ok(wallets);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy ví");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet/{walletID}")]
        public async Task<ActionResult<Wallet>> GetWallet(Guid walletID)
        {
            var result = await _adminService.GetWalletByWalletId(walletID);
            if (result is ActionResult<Wallet> wallet) return Ok(wallet);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy ví");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet/user/{userID}")]
        public async Task<ActionResult<Wallet>> GetWalletByUserID(Guid userID)
        {
            var result = await _adminService.GetWalletByUserId(userID);
            if (result is ActionResult<Wallet> wallet) return Ok(wallet);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy ví");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions")]
        public async Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions()
        {
            var result = await _adminService.GetAllBookingTransactions();
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions) return Ok(bookingTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch đặt lịch");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transaction/{bookingTransactionID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransaction(Guid bookingTransactionID)
        {
            var result = await _adminService.GetBookingTransactionsByBookingId(bookingTransactionID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransaction) return Ok(bookingTransaction);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch đặt lịch");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderID(Guid senderID)
        {
            var result = await _adminService.GetBookingTransactionsBySenderId(senderID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions) return Ok(bookingTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch đặt lịch");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _adminService.GetBookingTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions) return Ok(bookingTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch đặt lịch");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions")]
        public async Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions()
        {
            var result = await _adminService.GetAllWalletTransactions();
            if (result is ActionResult<List<WalletTransaction>> walletTransactions) return Ok(walletTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch ví");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transaction/{walletTransactionID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransaction(Guid walletTransactionID)
        {
            var result = await _adminService.GetWalletTransactionsByWalletTransactionId(walletTransactionID);
            if (result is ActionResult<List<WalletTransaction>> walletTransaction) return Ok(walletTransaction);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch ví");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderID(Guid senderID)
        {
            var result = await _adminService.GetWalletTransactionsBySenderId(senderID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions) return Ok(walletTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch ví");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _adminService.GetWalletTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions) return Ok(walletTransactions);
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404)
                {
                    return NotFound("Không tìm thấy giao dịch ví");
                }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Lỗi không xác định");
        }

    }
}
