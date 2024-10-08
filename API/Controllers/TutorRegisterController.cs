﻿using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Drawing;
using AutoMapper;
using System.Drawing.Printing;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class TutorRegisterController : ControllerBase
    {
        private readonly ITutorRegisterService _tutorRegisterService;
        private readonly IMapper _mapper;
        public TutorRegisterController(ITutorRegisterService tutorRegisterService, IMapper mapper)
        {
            _tutorRegisterService = tutorRegisterService;
            _mapper = mapper;
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy thông tin người dùng" });
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }

        /// <summary>
        /// step 3: Add Experience 
        /// </summary>
        // Add Experience
        [HttpPost("register/experiences/{tutorID}")]
        public async Task<IActionResult> addRegisterExperienceOfTutor(Guid tutorID, List<TutorExperienceRequest> tutorExperiences)
        {
            var result = await _tutorRegisterService.RegisterTutorExperience(tutorID, tutorExperiences);
            return result;
        }

        /// <summary>
        /// Step 4 : Add Sub Tutor Inforamtion
        /// </summary>
        // Add Sub Tutor Inforamtion
        [HttpPost("register/sub-tutor/{tutorID}")]
        public async Task<IActionResult> addSubInformation( Guid tutorID, [FromBody] TutorSubInformationRequest request)
        {
            var response = await _tutorRegisterService.RegisterSubTutorInformation(tutorID, request);
            return response;
        }

        /// <summary>
        /// Step 5: Create Tutor Slot In Register Tutor Step
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
        /// Step 6 : Add Money + Confirm, Tutor Status :Pending = 0, Active = 1, Inprocessing = 2,Denny = 3, Banned = 4,
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy thông tin gia sư" });
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy thông tin gia sư" });
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
            if (result is ActionResult<List<TutorRegisterReponse>> tutorRegisterReponses && tutorRegisterReponses.Value.Count > 0) return Ok(tutorRegisterReponses.Value);
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
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }

        // Deny Tutor Register
        [HttpPost("deny")]
        public async Task<IActionResult> denyTutorRegister([FromBody]TutorApprovalRequest request)
        {
            var result = await _tutorRegisterService.DenyTheTutorRegister(request);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 200) return Ok(new { Message = "Từ chối thành công" });
                if (statusCodeResult.StatusCode == 400) return BadRequest(new { Message = "Vui lòng kiểm tra lại dữ liệu đầu vào" });
                if (statusCodeResult.StatusCode == 404) return Ok(new { Message = "Không tìm thấy thông tin gia sư" });
            }
            if (result is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Xảy ra lỗi không xác định");
        }
        [HttpGet("get/tutor-actions/{tutorId}")]
        public async Task<ActionResult<PageResults<TutorActionView>>> getTutorActionByTutorId(Guid tutorId, int size, int pageSize)
        {
            var result = await _tutorRegisterService.GetTutorActionByTutorId(tutorId, size, pageSize);
            if (result is ActionResult<PageResults<TutorAction>> tutorActions)
            {
                if (tutorActions.Value == null) return NotFound("Không tìm thấy thông tin gia sư");
                var tutorActionViews = _mapper.Map<PageResults<TutorAction>, PageResults<TutorActionView>>(tutorActions.Value);
                return Ok(tutorActionViews);
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Xảy ra lỗi không xác định");
        }
        [HttpGet("get/tutor-action/{id}")]
        public async Task<ActionResult<TutorActionView>> getTutorActionById(Guid id)
        {
            var result = await _tutorRegisterService.GetTutorActionById(id);
            if (result is ActionResult<TutorAction> tutorAction)
            {
                if (tutorAction.Value == null) return NotFound("Không tìm thấy thông tin gia sư");
                var tutorActionView = _mapper.Map<TutorAction, TutorActionView>(tutorAction.Value);
                return Ok(tutorActionView);
            }
            if ((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Xảy ra lỗi không xác định");
        }
        /// <summary>
        /// Get Step 1 Tutor Information
        /// </summary>
        /// <param name="tutorID"></param>
        /// <returns></returns>
        // Get Tutor Step 1 By Tutor ID
        [HttpGet("get/tutor-step1/{tutorID}")]
        public async Task<ActionResult<TutorRegisterStep1Response>> GetTutorStep1Response (Guid tutorID)
        {
            var result = await _tutorRegisterService.GetTutorStep1ByTutorID(tutorID);
            return result;
        }

        /// <summary>
        /// Get Step 2 Tutor Certificate
        /// </summary>
        /// <param name="tutorID"></param>
        /// <returns></returns>
        // Get Tutor Step 2 By Tutor ID
        [HttpGet("get/tutor-step2/{tutorID}")]
        public async Task<ActionResult<List<TutorRegisterStep2Response>>> GetTutorStep2Response(Guid tutorID)
        {
            var result = await _tutorRegisterService.GetTutorStep2ByTutorID(tutorID);
            return result;
        }

        /// <summary>
        /// Get Step 3 Tutor Experience
        /// </summary>
        /// <param name="tutorID"></param>
        /// <returns></returns>
        // Get Tutor Step 3 By Tutor ID
        [HttpGet("get/tutor-step3/{tutorID}")]
        public async Task<ActionResult<List<TutorRegisterStep3Response>>> GetTutorStep3Response(Guid tutorID)
        {
            var result = await _tutorRegisterService.GetTutorStep3ByTutorID(tutorID);
            return result;
        }

        /// <summary>
        /// Get Step 4 Tutor Sub Information
        /// </summary>
        /// <param name="tutorID"></param>
        /// <returns></returns>
        //Get Tutor Step 4 By Tutor ID
        [HttpGet("get/tutor-step4/{tutorID}")]
        public async Task<ActionResult<TutorRegisterStep4Response>> GetTutorStep4Response(Guid tutorID)
        {
            var result = await _tutorRegisterService.GetTutorStep4ByTutorID(tutorID);
            return result;
        }

        /// <summary>
        /// Get Step 5 Tutor Schedule
        /// </summary>
        /// <param name="tutorID"></param>
        /// <returns></returns>
        // Get Tutor Step 5 By Tutor ID
        [HttpGet("get/tutor-step5/{tutorID}")]
        public async Task<ActionResult<List<TutorRegisterStep5Reponse>>> GetTutorStep5Response(Guid tutorID)
        {
            var result = await _tutorRegisterService.GetTutorStep5ByTutorID(tutorID);
            return result;
        }

        /// <summary>
        /// Get Step 6 Tutor Price
        /// </summary>
        /// <param name="tutorID"></param>
        /// <returns></returns>
        // Get Tutor Step 6 By Tutor ID
        [HttpGet("get/tutor-step6/{tutorID}")]
        public async Task<ActionResult<TutorRegisterStep6Response>> GetTutorStep6Response(Guid tutorID)
        {
            var result = await _tutorRegisterService.GetTutorStep6TutorID(tutorID);
            return result;
        }

        /// <summary>
        ///  Đăng kí môn học cho tutor
        /// </summary>
        [HttpPost("register/subject")]
        public async Task<IActionResult> createTutorSubjectList([FromBody] TutorSubjectRegisterRequest request)
        {
            var response = await _tutorRegisterService.CreateTutorSubjectList(request);
            return response;
        }

        /// <summary>
        /// Lấy danh sach môn học của tutor
        /// </summary>
        [HttpGet("get/tutor-subject")]
        public async Task<ActionResult<PageResults<TutorSubjectListResponse>>> getTutorSubjectList(Guid tutorID, int size, int pageSize)
        {
            var result = await _tutorRegisterService.GetTutorSubjectList(tutorID, size, pageSize);
            return result;
        }

        /// <summary>
        /// Xóa môn học dành cho tutor
        /// </summary>
        [HttpDelete("remove/tutor-subject/{tutorID}/{subjectID}")]
        public async Task<IActionResult> removeTutorSubject(Guid tutorID, Guid subjectID)
        {
            var response = await _tutorRegisterService.RemoveTutorSubject(tutorID, subjectID);
            return response;
        }

        ///// <summary>
        ///  Tạo slot theo thời gian đăng kí của Tutor 
        /// </summary> 
        [HttpPost("create/slot-by-week-date")]
        public async Task<IActionResult> createTutorSlotByWeekDate([FromBody] TutorRegistScheduleRequest request)
        {
            var response = await _tutorRegisterService.CreateTutorSlotByWeekDate(request);
            return response;
        }

        /// <summary>
        /// Xóa slot theo tutorSlotID
        /// </summary>
        [HttpDelete("delete/slot/{tutorSlotID}")]
        public async Task<IActionResult> deleteSlotByTutorSlotID(Guid tutorSlotID)
        {
            var response = await _tutorRegisterService.DeleteSlotByTutorSlotID(tutorSlotID);
            return response;
        }

        /// <summary>
        /// Lấy tutor Rating List có Paging 
        /// </summary>
        [HttpGet("get/tutor-rating-list")]
        public async Task<ActionResult<PageResults<TutorRatingListResponse>>> getTutorRatingList(Guid TutorId, int size, int PageSize)
        {
            var result = await _tutorRegisterService.GetTutorRatingList(TutorId, size, PageSize);
            return result;
        }

        /// <summary>
        /// Khóa hoặc mở tutor theo tutorID
        /// </summary>
        [HttpPut("block-or-unblock-tutor/{tutorId}")]
        public async Task<IActionResult> blockOrUnBlockTutorByTutorID(Guid tutorId)
        {
            var response = await _tutorRegisterService.BlockOrUnBlockTutorByTutorID(tutorId);
            return response;
        }

        ///<summary>
        ///Lấy danh sach subject dựa trên cái tutor đã đăng kí
        ///</summary>
        [HttpGet("get-tutor-not-register/{tutorId}")]
        public async Task<ActionResult<List<SubjectView>>> getTutorNotRegisterSubject(Guid tutorId)
        {
            var result = await _tutorRegisterService.GetAllSubjectWithoutTutorSubject(tutorId);
            return result;
        }

        /// <summary>
        /// Change Status of Tutor Subject When they has expried Time is end 
        /// </summary>
        [HttpPut("change-status-tutor-subject")]
        public async Task<IActionResult> changeStatusTutorSubject()
        {
            var response = await _tutorRegisterService.ChangeStatusForAllTutorSubject();
            return response;
        }

        /// <summary>
        /// Get Schedule Tutor Step 5
        /// </summary>
        [HttpGet("get/tutor-step5-ver2/{tutorId}")]
        public async Task<ActionResult<List<TutorStep5ResponseVer2>>> getTutorStep5RegisterResponse(Guid TutorId)
        {
            var result = await _tutorRegisterService.GetTutorStep5RegisterResponse(TutorId);
            return result;
        }
    }
}

