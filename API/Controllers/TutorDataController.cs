using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using Services;
using Services.Interfaces;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorDataController : ControllerBase
    {
        private readonly ITutorDataService _tutorDataService;
        public TutorDataController(ITutorDataService tutorDataService)
        {
            _tutorDataService = tutorDataService;
        }
        [HttpGet("get/all")]
        public async Task<ActionResult<List<TutorAccountResponse>>> GetAvalaibleTutors()
        {
            var tutors = await _tutorDataService.GetAvalaibleTutors();
            if(tutors == null) return NotFound();
            return tutors;
        }
        [HttpGet("get/all/v2")]
        public async Task<ActionResult<List<TutorAccountResponse>>> GetAvailaibleV2([FromQuery] PagingRequest pagingRequest)
        {
            var tutors = await _tutorDataService.GetAvalaibleTutorsV2(pagingRequest);
            return Ok(tutors);
        }

        [HttpGet("get/{tutorId}")]
        public async Task<ActionResult<TutorAccountResponse>> GetTutorById(Guid tutorId)
        {
            var tutor = await _tutorDataService.GetTutorById(tutorId);
            if (tutor == null) return StatusCode(StatusCodes.Status404NotFound, new {Message = "Không tìm thấy tài khoản, hoặc tài khoản bạn tìm đang bị đình chỉ" } );
            return tutor;
        }

        
        [HttpGet("get/rating/{tutorId}")]
        public async Task<ActionResult<TutorRatingResponse>> GetTutorRating(Guid tutorId)
        {
            var tutorRating = await _tutorDataService.GetTutorRating(tutorId);
            return tutorRating;
        }

        // Get Tutor Feedback Response by Tutor ID
        [HttpGet("get/feedbacks/{tutorID}")]
        public async Task<ActionResult<List<TutorFeedBackResponse>>> GetTutorFeedBackResponseByTutorID(Guid tutorID, [FromQuery] PagingRequest pagingRequest)
        {
            var response = await _tutorDataService.GetTutorFeedBackResponseByTutorID(tutorID, pagingRequest);
            return Ok(response);
        }

        // Get Tutor Slot Registered by Tutor ID
        /// <summary>
        ///  Get All Tutor Slot Registered by Tutor ID
        /// </summary>
        [HttpGet("get/slots/{tutorID}")]
        public async Task<ActionResult<List<TutorScheduleResponse>>> GetAllTutorScheduleByTutorID (Guid tutorID)
        {
            var response = await _tutorDataService.GetAllTutorSlotRegistered(tutorID);
            return Ok(response);
        }
        [HttpGet("get/slot/{tutorSlotAvalaibleId}")]
        public async Task<ActionResult<TutorSlotResponse>> GetTutorSlotAvalaibleById(Guid tutorSlotAvalaibleId)
        {
            var response = await _tutorDataService.GetTutorSlotAvalaibleById(tutorSlotAvalaibleId);
            return response;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTutorInformation([FromBody] TutorInformationUpdate tutorInformationUpdate)
        {
            var response = await _tutorDataService.UpdateTutorInformation(tutorInformationUpdate);
            return response;
        }

        [HttpGet("count/subject/{tutorID}")]
        public async Task<ActionResult<TutorCountSubjectResponse>> CountAllSubjectOfTutor(Guid tutorID)
        {
            var response = await _tutorDataService.CountAllSubjectOfTutor(tutorID);
            return response;
        }

        /// <summary>
        /// Thống kê ở trang tutor ( phần stats)
        /// </summary>
        [HttpGet("statistic/stats/{tutorID}")]
        public async Task<ActionResult<TutorCountResponse>> CountTutorMoney(Guid tutorID)
        {
            var response = await _tutorDataService.CountTutorMoney(tutorID);
            return response;
        }

        /// <summary>
        ///  Lấy 5 sinh viên học nhiều nhất của tutor
        ///</summary>   
        [HttpGet("get/top5/{tutorID}")]
        public async Task<ActionResult<List<StudentStatisticView>>> GetTop5StudentLearnMost(Guid tutorID)
        {
            var response = await _tutorDataService.GetTop5StudentLearnMost(tutorID);
            return response;
        }

        /// <summary>
        /// Lấy số lượng sinh viên theo ngày trong tuần
        /// </summary>
        [HttpGet("get/dayofweek/{tutorID}")]
        public async Task<ActionResult<List<StudentStatisticView>>> GetStudentStatisticByDayOfWeek(Guid tutorID, [FromQuery] int dayOfWeek)
        {
            var response = await _tutorDataService.GetStudentStatisticByDayOfWeek(tutorID, dayOfWeek);
            return response;
        }

        /// <summary>
        /// Lấy số lượng sinh viên theo tháng trong năm
        ///</summary>
        [HttpGet("get/monthofyear/{tutorID}")]
        public async Task<ActionResult<List<StudentStatisticView>>> GetStudentStatisticByMonthOfYear(Guid tutorID, [FromQuery] int monthOfYear)
        {
            var response = await _tutorDataService.GetStudentStatisticByMonthOfYear(tutorID, monthOfYear);
            return response;
        }

        /// <summary>
        /// Lấy 1 tutor info từ userid 
        /// </summary>
        [HttpGet("get/tutor-userId/{userId}")]
        public async Task<ActionResult<TutorView>> GetTutorByUserID(Guid userId)
        {
            try
            {
                if (!Guid.TryParse(userId.ToString(), out Guid validUserID))
                {
                    return BadRequest("UserID không hợp lệ.");
                }

                var response = await _tutorDataService.GetTutorByUserID(validUserID);
                if (response == null)
                {
                    return NotFound("Tutor không được tìm thấy.");
                }

                return Ok(response);
            }
            catch (CrudException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Thống kê số lượng học sinh theo thời gian trong ngày
        /// </summary>
        [HttpGet("get/number-student-by-time/{tutorId}")]
        public async Task<ActionResult<StudentStatisticNumberByTimeOfDatResponse>> GetNumberOfStudentPercentageByTimeOfDate(Guid tutorId)
        {
            var response = await _tutorDataService.GetNumberOfStudentPercentageByTimeOfDate(tutorId);
            return response;
        }

        /// <summary>
        /// Check kiểm tra tin nhắn và đếm 
        /// </summary>
        [HttpPost("count-message/{tutorId}")]
        public async Task<ActionResult<IActionResult>> CountTutorMessage(Guid tutorId)
        {
            var response = await _tutorDataService.CountTutorMessage(tutorId);
            return response;
        }
    }
}
