using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
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
            if (result is ActionResult<List<User>> users)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy người dùng"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(users);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user/{userID}")]
        public async Task<ActionResult<User>> GetUser(Guid userID)
        {
            var result = await _adminService.GetUser(userID);
            if (result is ActionResult<User> user)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy người dùng"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(user);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/students")]
        public async Task<ActionResult<List<Student>>> GetAllStudents()
        {
            var result = await _adminService.GetAllStudents();
            if (result is ActionResult<List<Student>> students)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(students);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student/{studentID}")]
        public async Task<ActionResult<Student>> GetStudent(Guid studentID)
        {
            var result = await _adminService.GetStudent(studentID);
            if (result is ActionResult<Student> student)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(student);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutors")]
        public async Task<ActionResult<List<Tutor>>> GetAllTutors()
        {
            var result = await _adminService.GetAllTutors();
            if (result is ActionResult<List<Tutor>> tutors)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutors);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor/{tutorID}")]
        public async Task<ActionResult<Tutor>> GetTutor(Guid tutorID)
        {
            var result = await _adminService.GetTutor(tutorID);
            if (result is ActionResult<Tutor> tutor)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutor);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/subjects")]
        public async Task<ActionResult<List<Subject>>> GetAllSubjects()
        {
            var result = await _adminService.GetAllSubjects();
            if (result is ActionResult<List<Subject>> subjects)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(subjects);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/subject/{subjectID}")]
        public async Task<ActionResult<Subject>> GetSubject(Guid subjectID)
        {
            var result = await _adminService.GetSubject(subjectID);
            if (result is ActionResult<Subject> subject)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(subject);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions")]
        public async Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions()
        {
            var result = await _adminService.GetAllCourseTransactions();
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch khóa học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transaction/{courseTransactionID}")]
        public async Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid courseTransactionID)
        {
            var result = await _adminService.GetCourseTransaction(courseTransactionID);
            if (result is ActionResult<CourseTransaction> courseTransaction)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch khóa học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransaction);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderID(Guid senderID)
        {
            var result = await _adminService.GetCourseTransactionsBySenderId(senderID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch khóa học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/receiver.{receiverID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _adminService.GetCourseTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch khóa học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/course-transactions/course/{courseID}")]
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseID(Guid courseID)
        {
            var result = await _adminService.GetCourseTransactionsByCourseId(courseID);
            if (result is ActionResult<List<CourseTransaction>> courseTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch khóa học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(courseTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/schedules")]
        public async Task<ActionResult<List<Schedule>>> GetAllSchedules()
        {
            var result = await _adminService.GetAllSchedules();
            if (result is ActionResult<List<Schedule>> schedules)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(schedules);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/schedule/{scheduleID}")]
        public async Task<ActionResult<Schedule>> GetSchedule(Guid scheduleID)
        {
            var result = await _adminService.GetSchedule(scheduleID);
            if (result is ActionResult<Schedule> schedule)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(schedule);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/schedules/student-course/{studentCourseID}")]
        public async Task<ActionResult<List<Schedule>>> GetSchedulesByStudentCourseID(Guid studentCourseID)
        {
            var result = await _adminService.GetSchedulesByStudentCourseId(studentCourseID);
            if (result is ActionResult<List<Schedule>> schedules)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch học"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(schedules);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-courses")]
        public async Task<ActionResult<List<StudentCourse>>> GetAllStudentCourses()
        {
            var result = await _adminService.GetAllStudentCourses();
            if (result is ActionResult<List<StudentCourse>> studentCourses)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa học sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentCourses);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-course/{studentCourseID}")]
        public async Task<ActionResult<StudentCourse>> GetStudentCourse(Guid studentCourseID)
        {
            var result = await _adminService.GetStudentCourse(studentCourseID);
            if (result is ActionResult<StudentCourse> studentCourse)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa học sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentCourse);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-courses/course/{courseID}")]
        public async Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByCourseID(Guid courseID)
        {
            var result = await _adminService.GetStudentCoursesByCourseId(courseID);
            if (result is ActionResult<List<StudentCourse>> studentCourses)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa học sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentCourses);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-courses/student/{studentID}")]
        public async Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByStudentID(Guid studentID)
        {
            var result = await _adminService.GetStudentCoursesByStudentId(studentID);
            if (result is ActionResult<List<StudentCourse>> studentCourses)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa học sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentCourses);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-requests")]
        public async Task<ActionResult<List<StudentRequest>>> GetAllStudentRequests()
        {
            var result = await _adminService.GetAllStudentRequests();
            if (result is ActionResult<List<StudentRequest>> studentRequests)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy yêu cầu sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequests);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-request/{studentRequestID}")]
        public async Task<ActionResult<StudentRequest>> GetStudentRequest(Guid studentRequestID)
        {
            var result = await _adminService.GetStudentRequest(studentRequestID);
            if (result is ActionResult<StudentRequest> studentRequest)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy yêu cầu sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequest);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-requests/student/{studentID}")]
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsByStudentID(Guid studentID)
        {
            var result = await _adminService.GetStudentRequestsByStudentId(studentID);
            if (result is ActionResult<List<StudentRequest>> studentRequests)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy yêu cầu sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequests);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/student-requests/subject/{subjectID}")]
        public async Task<ActionResult<List<StudentRequest>>> GetStudentRequestsBySubjectID(Guid subjectID)
        {
            var result = await _adminService.GetStudentRequestsBySubjectId(subjectID);
            if (result is ActionResult<List<StudentRequest>> studentRequests)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy yêu cầu sinh viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(studentRequests);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificates")]
        public async Task<ActionResult<List<TutorCertificate>>> GetAllTutorCertificates()
        {
            var result = await _adminService.GetAllTutorCertificates();
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chứng chỉ gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorCertificates);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificate/{tutorCertificateID}")]
        public async Task<ActionResult<TutorCertificate>> GetTutorCertificate(Guid tutorCertificateID)
        {
            var result = await _adminService.GetTutorCertificate(tutorCertificateID);
            if (result is ActionResult<TutorCertificate> tutorCertificate)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chứng chỉ gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorCertificate);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificates/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorCertificatesByTutorId(tutorID);
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chứng chỉ gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorCertificates);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificates/subject/{subjectID}")]
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesBySubjectID(Guid subjectID)
        {
            var result = await _adminService.GetTutorCertificatesBySubjectId(subjectID);
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy chứng chỉ gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorCertificates);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subjects")]
        public async Task<ActionResult<List<TutorSubject>>> GetAllTutorSubjects()
        {
            var result = await _adminService.GetAllTutorSubjects();
            if (result is ActionResult<List<TutorSubject>> tutorSubjects)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorSubjects);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subject/{tutorSubjectID}")]
        public async Task<ActionResult<TutorSubject>> GetTutorSubject(Guid tutorSubjectID)
        {
            var result = await _adminService.GetTutorSubject(tutorSubjectID);
            if (result is ActionResult<TutorSubject> tutorSubject)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorSubject);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subjects/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorSubjectsByTutorId(tutorID);
            if (result is ActionResult<List<TutorSubject>> tutorSubjects)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorSubjects);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-subjects/subject/{subjectID}")]
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsBySubjectID(Guid subjectID)
        {
            var result = await _adminService.GetTutorSubjectsBySubjectId(subjectID);
            if (result is ActionResult<List<TutorSubject>> tutorSubjects)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy môn học gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorSubjects);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-ratings")]
        public async Task<ActionResult<List<TutorRating>>> GetAllTutorRatings()
        {
            var result = await _adminService.GetAllTutorRatings();
            if (result is ActionResult<List<TutorRating>> tutorRatings)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy đánh giá gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorRatings);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-rating/{tutorRatingID}")]
        public async Task<ActionResult<TutorRating>> GetTutorRating(Guid tutorRatingID)
        {
            var result = await _adminService.GetTutorRating(tutorRatingID);
            if (result is ActionResult<TutorRating> tutorRating)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy đánh giá gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorRating);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-ratings/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorRatingsByTutorId(tutorID);
            if (result is ActionResult<List<TutorRating>> tutorRatings)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy đánh giá gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorRatings);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-ratings/student/{studentID}")]
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByStudentID(Guid studentID)
        {
            var result = await _adminService.GetTutorRatingsByStudentId(studentID);
            if (result is ActionResult<List<TutorRating>> tutorRatings)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy đánh giá gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorRatings);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-rating-images")]
        public async Task<ActionResult<List<TutorRatingImage>>> GetAllTutorRatingImages()
        {
            var result = await _adminService.GetAllTutorRatingImages();
            if (result is ActionResult<List<TutorRatingImage>> tutorRatingImages)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy hình ảnh đánh giá gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorRatingImages);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-rating-image/{tutorRatingImageID}")]
        public async Task<ActionResult<TutorRatingImage>> GetTutorRatingImage(Guid tutorRatingImageID)
        {
            var result = await _adminService.GetTutorRatingImage(tutorRatingImageID);
            if (result is ActionResult<TutorRatingImage> tutorRatingImage)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy hình ảnh đánh giá gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorRatingImage);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-schedules")]
        public async Task<ActionResult<List<TutorSchedule>>> GetAllTutorSchedules()
        {
            var result = await _adminService.GetAllTutorSchedules();
            if (result is ActionResult<List<TutorSchedule>> tutorSchedules)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch học gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorSchedules);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-schedule/{tutorScheduleID}")]
        public async Task<ActionResult<TutorSchedule>> GetTutorSchedule(Guid tutorScheduleID)
        {
            var result = await _adminService.GetTutorSchedule(tutorScheduleID);
            if (result is ActionResult<TutorSchedule> tutorSchedule)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch học gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorSchedule);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-schedules/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorSchedule>>> GetTutorSchedulesByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorSchedulesByTutorId(tutorID);
            if (result is ActionResult<List<TutorSchedule>> tutorSchedules)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy lịch học gia sư"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(tutorSchedules);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks")]
        public async Task<ActionResult<List<UserBlock>>> GetAllUserBlocks()
        {
            var result = await _adminService.GetAllUserBlocks();
            if (result is ActionResult<List<UserBlock>> userBlocks)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa tài khoản"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(userBlocks);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-block/create/{userBlockID}")]
        public async Task<ActionResult<List<UserBlock>>> GetUserBlock(Guid userBlockID)
        {
            var result = await _adminService.GetAllBlockByCreateUserId(userBlockID);
            if (result is ActionResult<List<UserBlock>> userBlock)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa tài khoản"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(userBlock);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-blocks/target/{targetID}")]
        public async Task<ActionResult<List<UserBlock>>> GetUserBlocksByTargetID(Guid targetID)
        {
            var result = await _adminService.GetAllBlockByTargetUserId(targetID);
            if (result is ActionResult<List<UserBlock>> userBlocks)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy khóa tài khoản"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(userBlocks);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follows")]
        public async Task<ActionResult<List<UserFollow>>> GetAllUserFollows()
        {
            var result = await _adminService.GetAllUserFollows();
            if (result is ActionResult<List<UserFollow>> userFollows)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy theo dõi tài khoản"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(userFollows);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follow/create/{userFollowID}")]
        public async Task<ActionResult<List<UserFollow>>> GetUserFollow(Guid userFollowID)
        {
            var result = await _adminService.GetAllFollowsByCreateUserId(userFollowID);
            if (result is ActionResult<List<UserFollow>> userFollow)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy theo dõi tài khoản"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(userFollow);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/user-follows/target/{targetID}")]
        public async Task<ActionResult<List<UserFollow>>> GetUserFollowsByTargetID(Guid targetID)
        {
            var result = await _adminService.GetAllFollowsByTargetUserId(targetID);
            if (result is ActionResult<List<UserFollow>> userFollows)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy theo dõi tài khoản"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(userFollows);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallets")]
        public async Task<ActionResult<List<Wallet>>> GetAllWallets()
        {
            var result = await _adminService.GetAllWallets();
            if (result is ActionResult<List<Wallet>> wallets)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(wallets);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet/{walletID}")]
        public async Task<ActionResult<Wallet>> GetWallet(Guid walletID)
        {
            var result = await _adminService.GetWalletByWalletId(walletID);
            if (result is ActionResult<Wallet> wallet)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(wallet);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet/user/{userID}")]
        public async Task<ActionResult<Wallet>> GetWalletByUserID(Guid userID)
        {
            var result = await _adminService.GetWalletByUserId(userID);
            if (result is ActionResult<Wallet> wallet)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy ví"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(wallet);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions")]
        public async Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions()
        {
            var result = await _adminService.GetAllBookingTransactions();
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch đặt lịch"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transaction/{bookingTransactionID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransaction(Guid bookingTransactionID)
        {
            var result = await _adminService.GetBookingTransactionsByBookingId(bookingTransactionID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransaction)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch đặt lịch"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransaction);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderID(Guid senderID)
        {
            var result = await _adminService.GetBookingTransactionsBySenderId(senderID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch đặt lịch"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _adminService.GetBookingTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<BookingTransaction>> bookingTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch đặt lịch"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(bookingTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions")]
        public async Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions()
        {
            var result = await _adminService.GetAllWalletTransactions();
            if (result is ActionResult<List<WalletTransaction>> walletTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch ví"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transaction/{walletTransactionID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransaction(Guid walletTransactionID)
        {
            var result = await _adminService.GetWalletTransactionsByWalletTransactionId(walletTransactionID);
            if (result is ActionResult<List<WalletTransaction>> walletTransaction)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch ví"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransaction);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/sender/{senderID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderID(Guid senderID)
        {
            var result = await _adminService.GetWalletTransactionsBySenderId(senderID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch ví"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/wallet-transactions/receiver/{receiverID}")]
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverID(Guid receiverID)
        {
            var result = await _adminService.GetWalletTransactionsByReceiverId(receiverID);
            if (result is ActionResult<List<WalletTransaction>> walletTransactions)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy giao dịch ví"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(walletTransactions);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/moderators")]
        public async Task<ActionResult<List<Moderator>>> GetAllModerators()
        {
            var result = await _adminService.GetModerators();
            if (result is ActionResult<List<Moderator>> moderators)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy quản trị viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(moderators);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/moderator/{moderatorID}")]
        public async Task<ActionResult<Moderator>> GetModerator(Guid moderatorID)
        {
            var result = await _adminService.GetModeratorById(moderatorID);
            if (result is ActionResult<Moderator> moderator)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy quản trị viên"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(moderator);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/notifications")]
        public async Task<ActionResult<List<Notification>>> GetAllNotifications()
        {
            var result = await _adminService.GetNotifications();
            if (result is ActionResult<List<Notification>> notifications)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy thông báo"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(notifications);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/notification/{userID}")]
        public async Task<ActionResult<List<Notification>>> GetNotificationsByUserId(Guid userID)
        {
            var result = await _adminService.GetNotificationsByUserId(userID);
            if (result is ActionResult<List<Notification>> notifications)
            {
                if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy thông báo"); }
                    if (statusCodeResult.StatusCode == 200) return Ok(notifications);
                }
                if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
            throw new Exception("Lỗi không xác định");
        }
    }
}
