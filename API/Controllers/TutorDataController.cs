using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;

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
    }
}
