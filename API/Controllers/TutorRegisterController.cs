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
        /// <summary>
        /// Step 1: Notice: "Get 3 fields: Fullname, Image, Email from Api Account" ,"Get Subject List from Api Get All Subject in System"
        /// </summary>
        // Add Informtion
        [HttpPost("register")]
        public async Task<IActionResult> addRegisterInformationOfTutor([FromBody] TutorInformationRequest tutorRequest, [FromQuery]List<Guid> tutorSubjectID)
        {
            var response = await _tutorRegisterService.RegisterTutorInformation(tutorRequest, tutorSubjectID);
            if (response is ActionResult<TutorRegisterStepOneResponse> tutorRegisterStepOneResponse && response.Value != null)
            {
                return Ok(tutorRegisterStepOneResponse.Value);
            }
            if((IActionResult)response.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại hình ảnh cá nhân nhập vào" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin người dùng" });
            }
            if((IActionResult)response.Result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }
        /// <summary>
        /// Step 2: Add Certificate
        /// </summary>
        // Add Certificate
        [HttpPost("register/certificate/{tutorID}")]
        public async Task<IActionResult> addRegisterCertificateOfTutor(Guid tutorID, List<TutorRegisterCertificateRequest> certificateRequest)
        {
            var result = await _tutorRegisterService.TutorCertificatesRegister(tutorID, certificateRequest);
            if(result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đăng ký bằng cấp thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu bằng cấp ở nhập liệu đầu vào" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }

        /*        // Add Subject
                [HttpPost("register/subjects/{tutorID}")]
                public async Task<IActionResult> addRegisterSubjectOfTutor(Guid tutorID, List<Guid> subjectIDs)
                {
                    var result = await _tutorRegisterService.RegisterTutorSubject(tutorID, subjectIDs);
                    if(result is StatusCodeResult statusCodeResult)
                    {
                        if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đăng ký môn học thành công" });
                        if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu môn học ở nhập liệu đầu vào" });
                        if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin gia sư" });
                    }
                    if (result is Exception exception)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
                    }
                    throw new Exception("Xảy ra lỗi không xác định");
                }*/
        /// <summary>
        /// step 3: Add Experience 
        /// </summary>
        // Add Experience
        [HttpPost("register/experiences/{tutorID}")]
        public async Task<IActionResult> addRegisterExperienceOfTutor(Guid tutorID, List<TutorExperienceRequest> tutorExperiences)
        {
            var result = await _tutorRegisterService.RegisterTutorExperience(tutorID, tutorExperiences);
            if(result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Đăng ký kinh nghiệm dạy học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu kinh nghiệm ở nhập liệu đầu vào" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }
        /// <summary>
        /// Step 4: Create Tutor Slot In Register Tutor Step
        /// </summary>
        /// <param name="tutorID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // Create Tutor Slot In Register Tutor Step
        [HttpPost("create/slot-schedule-v2/{tutorID}")]
        public async Task<IActionResult> createTutorSlotScheduleV2(Guid tutorID, List<TutorRegisterSlotRequest> request)
        {
            var response = await _tutorRegisterService.CreateTutorSlotInRegisterTutorStep(tutorID, request);
            return response;
        }
        /// <summary>
        /// Step 5 : Add Money + Confirm
        /// </summary>
        // Confirm and Create Notification
        [HttpPost("confirm")]
        public async Task<IActionResult> confirmRegisterFormAndCreateNoti([FromBody]TutorConfirmRequest request)
        {
            var response = await _tutorRegisterService.CheckConfirmTutorInformationAndSendNotification(request);
            if (response is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Yêu cầu xét duyệt trở thành gia sư đã được gửi, vui lòng đợi phản hồi từ hệ thống" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu môn học và bằng cấp ở nhập liệu đầu vào" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (response is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }
        /// <summary>
        /// step 6 fe, lưu ý ko cố tình tạo 1 khoảng quá dài tránh thời gian xử lý request lâu hơn 1 phút
        /// </summary>
        //Create Tutor Slot Schedule
        [HttpPost("create/slot-schedule")]
        public async Task<IActionResult> createTutorSlotSchedule([FromBody] TutorRegistScheduleRequest tutorRegistScheduleRequest)
        {
            var response = await _tutorRegisterService.CreateTutorSlotSchedule(tutorRegistScheduleRequest);
            if(response is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo lịch học thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu ngày ở nhập liệu đầu vào" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin gia sư" });
                if (statusCodeResult.StatusCode == 409) return StatusCode(StatusCodes.Status409Conflict ,new { Message = "Giờ kết thúc không thể xảy ra trước giờ bắt đầu, vui lòng kiểm tra lại" });
            }
            if(response is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new {Message = exception.ToString()});
            }
            throw new Exception("Xảy ra lỗi không xác định");
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
        [HttpGet("get/tutor-registers")]
        public async Task<ActionResult<List<TutorRegisterReponse>>> getAllTutorRegisterInformation()
        {
            var result = await _tutorRegisterService.GetAllTutorRegisterInformation();
            if (result is ActionResult<List<TutorRegisterReponse>> tutorRegisterReponses && tutorRegisterReponses.Value.Count > 0) return Ok(tutorRegisterReponses);
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }

        // Approve Tutor Register
        [HttpPost("approve")]
        public async Task<IActionResult> approveTutorRegister([FromBody]TutorApprovalRequest request)
        {
            var result = await _tutorRegisterService.ApproveTheTutorRegister(request);
            if(result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Duyệt thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu đầu vào" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }

        // Deny Tutor Register
        [HttpPost("deny/{tutorActionId}/{denyID}")]
        public async Task<IActionResult> denyTutorRegister([FromBody]TutorApprovalRequest request)
        {
            var result = await _tutorRegisterService.DenyTheTutorRegister(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Từ chối thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu đầu vào" });
                if (statusCodeResult.StatusCode == 404) return NotFound(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }
    }

}

