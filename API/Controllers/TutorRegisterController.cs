using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Drawing;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class TutorRegisterController : ControllerBase
    {
        private readonly ITutorRegisterService _tutorRegisterService;
        public TutorRegisterController(ITutorRegisterService tutorRegisterService)
        {
            _tutorRegisterService = tutorRegisterService;
        }

        // Add Informtion
        [HttpPost("register")]
        public async Task<IActionResult> addRegisterInformationOfTutor([FromBody] TutorInformationRequest tutorRequest)
        {
            var response = await _tutorRegisterService.RegisterTutorInformation(tutorRequest);
            return Ok(response);
        }

        // Add Certificate
        [HttpPost("register/certificate/{tutorID}")]
        public async Task<IActionResult> addRegisterCertificateOfTutor(Guid tutorID, List<TutorRegisterCertificateRequest> certificateRequest)
        {
            var result = await _tutorRegisterService.TutorCertificatesRegister(tutorID, certificateRequest);
            return result;
        }

        // Add Subject
        [HttpPost("register/subjects/{tutorID}")]
        public async Task<IActionResult> addRegisterSubjectOfTutor(Guid tutorID, List<Guid> subjectIDs)
        {
            var result = await _tutorRegisterService.RegisterTutorSubject(tutorID, subjectIDs);
            return result;
        }
        
        // Add Experience
        [HttpPost("register/experiences/{tutorID}")]
        public async Task<IActionResult> addRegisterExperienceOfTutor(Guid tutorID, List<TutorExperienceRequest> tutorExperiences)
        {
            var result = await _tutorRegisterService.RegisterTutorExperience(tutorID, tutorExperiences);
            return result;
        }

        // Confirm and Create Notification
        [HttpPost("confirm/{tutorID}")]
        public async Task<IActionResult> confirmRegisterFormAndCreateNoti(Guid tutorID, decimal money)
        {
            var response = await _tutorRegisterService.CheckConfirmTutorInformationAndSendNotification(tutorID,money);
            return response;
        }

        //Create Tutor Slot Schedule
        [HttpPost("create/slot-schedule/{tutorID}")]
        public async Task<IActionResult> createTutorSlotSchedule(Guid tutorID, [FromBody] TutorRegistScheduleRequest tutorRegistScheduleRequest)
        {
            var response = await _tutorRegisterService.CreateTutorSlotSchedule(tutorID, tutorRegistScheduleRequest);
            return response;
        }

        // Get All Tutor Register Information
        [HttpGet("get/tutor-register/{tutorID}")]
        public async Task<ActionResult<TutorRegisterReponse>> getAllTutorRegisterInformation(Guid tutorID)
        {
            var result = await _tutorRegisterService.GetTutorRegisterInformtaion(tutorID);
            if (result is ActionResult<TutorRegisterReponse> tutorRegisterReponse)
            {
                if (tutorRegisterReponse.Value == null) return NotFound("Không tìm thấy thông tin gia sư");
                return Ok(tutorRegisterReponse.Value);
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Xảy ra lỗi không xác định");
        }


    }

}

