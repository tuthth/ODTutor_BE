﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }

        [HttpGet("get/users")]
        public async Task<ActionResult<List<UserView>>> GetAllUsers()
        {
            var result = await _adminService.GetAllUsers();
            if (result is ActionResult<List<User>> users && result.Value != null)
            {
                var userViews = _mapper.Map<List<UserView>>(users.Value);
                return Ok(userViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy người dùng" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/users/paging")]
        public async Task<ActionResult<PageResults<UserView>>> GetAllUsersPaging(int page, int pageSize)
        {
            var request = new PagingRequest
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _adminService.GetAllUsersPaging(request);
            if (result is ActionResult<PageResults<User>> users && result.Value != null)
            {
                var userViews = _mapper.Map<PageResults<User>, PageResults<UserView>>(users.Value);
                return Ok(userViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy người dùng" }); }
                if (statusCodeResult.StatusCode == 400) { return BadRequest(new { Message = "Dữ liệu không hợp lệ" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/user/{userID}")]
        public async Task<ActionResult<UserView>> GetUser(Guid userID)
        {
            var result = await _adminService.GetUser(userID);
            if (result is ActionResult<User> user && result.Value != null)
            {
                var userView = _mapper.Map<UserView>(user.Value);
                return Ok(userView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy người dùng" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutors")]
        public async Task<ActionResult<List<TutorView>>> GetAllTutors()
        {
            var result = await _adminService.GetAllTutors();
            if (result is ActionResult<List<Tutor>> tutors && result.Value != null)
            {
                var tutorViews = _mapper.Map<List<TutorView>>(tutors.Value);
                return Ok(tutorViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor/{tutorID}")]
        public async Task<ActionResult<TutorView>> GetTutor(Guid tutorID)
        {
            var result = await _adminService.GetTutor(tutorID);
            if (result is ActionResult<Tutor> tutor && result.Value != null)
            {
                var tutorView = _mapper.Map<TutorView>(tutor.Value);
                return Ok(tutorView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-certificates")]
        public async Task<ActionResult<List<TutorCertificateView>>> GetAllTutorCertificates()
        {
            var result = await _adminService.GetAllTutorCertificates();
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates && result.Value != null)
            {
                var tutorCertificateViews = _mapper.Map<List<TutorCertificateView>>(tutorCertificates.Value);
                return Ok(tutorCertificateViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy chứng chỉ gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-certificate/{tutorCertificateID}")]
        public async Task<ActionResult<TutorCertificateView>> GetTutorCertificate(Guid tutorCertificateID)
        {
            var result = await _adminService.GetTutorCertificate(tutorCertificateID);
            if (result is ActionResult<TutorCertificate> tutorCertificate && result.Value != null)
            {
                var tutorCertificateView = _mapper.Map<TutorCertificateView>(tutorCertificate.Value);
                return Ok(tutorCertificateView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy chứng chỉ gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-certificates/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorCertificateView>>> GetTutorCertificatesByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorCertificatesByTutorId(tutorID);
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates && result.Value != null)
            {
                var tutorCertificateViews = _mapper.Map<List<TutorCertificateView>>(tutorCertificates.Value);
                return Ok(tutorCertificateViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy chứng chỉ gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-certificates/subject/{subjectID}")]
        public async Task<ActionResult<List<TutorCertificateView>>> GetTutorCertificatesBySubjectID(Guid subjectID)
        {
            var result = await _adminService.GetTutorCertificatesBySubjectId(subjectID);
            if (result is ActionResult<List<TutorCertificate>> tutorCertificates && result.Value != null)
            {
                var tutorCertificateViews = _mapper.Map<List<TutorCertificateView>>(tutorCertificates.Value);
                return Ok(tutorCertificateViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy chứng chỉ gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-subjects")]
        public async Task<ActionResult<List<TutorSubjectView>>> GetAllTutorSubjects()
        {
            var result = await _adminService.GetAllTutorSubjects();
            if (result is ActionResult<List<TutorSubject>> tutorSubjects && result.Value != null)
            {
                var tutorSubjectViews = _mapper.Map<List<TutorSubjectView>>(tutorSubjects.Value);
                return Ok(tutorSubjectViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-subject/{tutorSubjectID}")]
        public async Task<ActionResult<TutorSubjectView>> GetTutorSubject(Guid tutorSubjectID)
        {
            var result = await _adminService.GetTutorSubject(tutorSubjectID);
            if (result is ActionResult<TutorSubject> tutorSubject && result.Value != null)
            {
                var tutorSubjectView = _mapper.Map<TutorSubjectView>(tutorSubject.Value);
                return Ok(tutorSubjectView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-subjects/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorSubjectView>>> GetTutorSubjectsByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorSubjectsByTutorId(tutorID);
            if (result is ActionResult<List<TutorSubject>> tutorSubjects && result.Value != null)
            {
                var tutorSubjectViews = _mapper.Map<List<TutorSubjectView>>(tutorSubjects.Value);
                return Ok(tutorSubjectViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-subjects/subject/{subjectID}")]
        public async Task<ActionResult<List<TutorSubjectView>>> GetTutorSubjectsBySubjectID(Guid subjectID)
        {
            var result = await _adminService.GetTutorSubjectsBySubjectId(subjectID);
            if (result is ActionResult<List<TutorSubject>> tutorSubjects && result.Value != null)
            {
                var tutorSubjectViews = _mapper.Map<List<TutorSubjectView>>(tutorSubjects.Value);
                return Ok(tutorSubjectViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy môn học gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-ratings")]
        public async Task<ActionResult<List<TutorRatingView>>> GetAllTutorRatings()
        {
            var result = await _adminService.GetAllTutorRatings();
            if (result is ActionResult<List<TutorRating>> tutorRatings && result.Value != null)
            {
                var tutorRatingViews = _mapper.Map<List<TutorRatingView>>(tutorRatings.Value);
                return Ok(tutorRatingViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy đánh giá gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-rating/{tutorRatingID}")]
        public async Task<ActionResult<TutorRatingView>> GetTutorRating(Guid tutorRatingID)
        {
            var result = await _adminService.GetTutorRating(tutorRatingID);
            if (result is ActionResult<TutorRating> tutorRating && result.Value != null)
            {
                var tutorRatingView = _mapper.Map<TutorRatingView>(tutorRating.Value);
                return Ok(tutorRatingView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy đánh giá gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-ratings/tutor/{tutorID}")]
        public async Task<ActionResult<List<TutorRatingView>>> GetTutorRatingsByTutorID(Guid tutorID)
        {
            var result = await _adminService.GetTutorRatingsByTutorId(tutorID);
            if (result is ActionResult<List<TutorRating>> tutorRatings && result.Value != null)
            {
                var tutorRatingViews = _mapper.Map<List<TutorRatingView>>(tutorRatings.Value);
                return Ok(tutorRatingViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy đánh giá gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-ratings/student/{studentID}")]
        public async Task<ActionResult<List<TutorRatingView>>> GetTutorRatingsByStudentID(Guid studentID)
        {
            var result = await _adminService.GetTutorRatingsByStudentId(studentID);
            if (result is ActionResult<List<TutorRating>> tutorRatings && result.Value != null)
            {
                var tutorRatingViews = _mapper.Map<List<TutorRatingView>>(tutorRatings.Value);
                return Ok(tutorRatingViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy đánh giá gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-rating-images")]
        public async Task<ActionResult<List<TutorRatingImageView>>> GetAllTutorRatingImages()
        {
            var result = await _adminService.GetAllTutorRatingImages();
            if (result is ActionResult<List<TutorRatingImage>> tutorRatingImages && result.Value != null)
            {
                var tutorRatingImageViews = _mapper.Map<List<TutorRatingImageView>>(tutorRatingImages.Value);
                return Ok(tutorRatingImageViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy hình ảnh đánh giá gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/tutor-rating-image/{tutorRatingImageID}")]
        public async Task<ActionResult<TutorRatingImageView>> GetTutorRatingImage(Guid tutorRatingImageID)
        {
            var result = await _adminService.GetTutorRatingImage(tutorRatingImageID);
            if (result is ActionResult<TutorRatingImage> tutorRatingImage && result.Value != null)
            {
                var tutorRatingImageView = _mapper.Map<TutorRatingImageView>(tutorRatingImage.Value);
                return Ok(tutorRatingImageView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy hình ảnh đánh giá gia sư" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/moderators")]
        public async Task<ActionResult<List<ModeratorView>>> GetAllModerators()
        {
            var result = await _adminService.GetModerators();
            if (result is ActionResult<List<Moderator>> moderators && result.Value != null)
            {
                var moderatorViews = _mapper.Map<List<ModeratorView>>(moderators.Value);
                return Ok(moderatorViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy quản trị viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/moderator/{moderatorID}")]
        public async Task<ActionResult<ModeratorView>> GetModerator(Guid moderatorID)
        {
            var result = await _adminService.GetModeratorById(moderatorID);
            if (result is ActionResult<Moderator> moderator && result.Value != null)
            {
                var moderatorView = _mapper.Map<ModeratorView>(moderator.Value);
                return Ok(moderatorView);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy quản trị viên" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/notifications")]
        public async Task<ActionResult<List<NotificationView>>> GetAllNotifications()
        {
            var result = await _adminService.GetNotifications();
            if (result is ActionResult<List<Notification>> notifications && result.Value != null)
            {
                var notificationViews = _mapper.Map<List<NotificationView>>(notifications.Value);
                return Ok(notificationViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thông báo" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        [HttpGet("get/notification/{userID}")]
        public async Task<ActionResult<List<NotificationView>>> GetNotificationsByUserId(Guid userID)
        {
            var result = await _adminService.GetNotificationsByUserId(userID);
            if (result is ActionResult<List<Notification>> notifications && result.Value != null)
            {
                var notificationViews = _mapper.Map<List<NotificationView>>(notifications.Value);
                return Ok(notificationViews);
            }
            if ((IActionResult)result.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thông báo" }); }
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// dow = day of week, morning: 0-8h, afternoon: 8-15h, evening: 15-24h
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("get/student-statistics/dow")]
        public async Task<IActionResult> GetStudentStatisticsByDayOfWeek()
        {
            var result = await _adminService.GetStudentStatisticsByDayOfWeek();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê học sinh" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        /// <summary>
        /// get by month
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("get/student-statistics/month")]
        public async Task<IActionResult> GetStudentStatisticsByMonth()
        {
            var result = await _adminService.GetStudentStatisticsByMonth();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê học sinh" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-statistics/dow")]
        public async Task<IActionResult> GetTutorStatisticsByDayOfWeek()
        {
            var result = await _adminService.GetTutorStatisticsByDayOfWeek();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê gia sư" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/tutor-statistics/month")]
        public async Task<IActionResult> GetTutorStatisticsByMonth()
        {
            var result = await _adminService.GetTutorStatisticsByMonth();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê gia sư" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-statistics/month")]
        public async Task<IActionResult> GetBookingStatisticsByMonth()
        {
            var result = await _adminService.GetBookingStatisticsByMonth();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê đặt lịch" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transaction-statistics/month")]
        public async Task<IActionResult> GetBookingTransactionStatisticsByMonth()
        {
            var result = await _adminService.GetBookingTransactionStatisticsByMonth();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê giao dịch đặt lịch" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-statistics/tutor/{tutorID}/month")]
        public async Task<IActionResult> GetBookingStatisticsOf1TutorByMonth(Guid tutorID)
        {
            var result = await _adminService.GetBookingStatisticsOf1TutorByMonth(tutorID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê đặt lịch của gia sư" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-statistics/top5-tutors/month")]
        public async Task<IActionResult> GetBookingStatisticsTop5TutorsByMonth()
        {
            var result = await _adminService.GetBookingStatisticsTop5TutorsByMonth();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê đặt lịch của gia sư" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transaction-statistics/tutor/{receiverID}/month")]
        public async Task<IActionResult> GetBookingTransactionStatisticsOfATutorByMonth(Guid receiverID)
        {
            var result = await _adminService.GetBookingTransactionStatisticsOfATutorByMonth(receiverID);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê giao dịch đặt lịch của gia sư" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("get/booking-transaction-statistics/top5-tutors/month")]
        public async Task<IActionResult> GetBookingTransactionStatisticsTop5TutorsByMonth()
        {
            var result = await _adminService.GetBookingTransactionStatisticsTop5TutorsByMonth();
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy thống kê giao dịch đặt lịch của gia sư" }); }
            }
            if (result is JsonResult jsonResult)
            {
                return Ok(jsonResult.Value);
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
