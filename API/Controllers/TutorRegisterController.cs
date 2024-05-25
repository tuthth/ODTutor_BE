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
            
                var result = await _tutorRegisterService.RegisterTutorInformation(tutorRequest);
                if(result is StatusCodeResult statusCodeResult)
                {
                    if(statusCodeResult.StatusCode == 200){return Ok("Đăng ký thông tin gia sư thành công");}
                    else if(statusCodeResult.StatusCode == 404){return BadRequest("Đăng ký thông tin gia sư thất bại");}
                }
                if(result is Exception exception)return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                throw new Exception("Xảy ra lỗi không xác định");
        }

        // Add Certificate
        [HttpPost("register/certificate/{tutorID}")]
        public async Task<IActionResult> addRegisterCertificateOfTutor(Guid tutorID, List<IFormFile> tutorRequest)
        {
            var result = await _tutorRegisterService.TutorCertificatesRegister(tutorID, tutorRequest);
            if(result is StatusCodeResult statusCodeResult)
            {
                var tutorEvidence = await _tutorRegisterService.TutorCertificatesRegister(tutorID, tutorRequest);
                if (tutorEvidence != null)
                {
                    return Ok("Bạn đã thêm thành công giấy chứng nhận");
                }
                else
                {
                    return BadRequest("Đăng ký bằng cấp gia sư thất bại");
                }
                if(statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created,"Đăng ký chứng chỉ gia sư thành công");}
                else if(statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy thông tin gia sư"); }
            }
            if(result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Xảy ra lỗi không xác định");
        }
        [HttpPost("register/subjects/{tutorID}")]
        public async Task<IActionResult> addRegisterSubjectOfTutor(Guid tutorID, List<Guid> subjectIDs)
        {
            var result = await _tutorRegisterService.RegisterTutorSubject(tutorID, subjectIDs);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created, "Đăng ký môn học gia sư thành công"); }
                else if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy thông tin gia sư"); }
                else if (statusCodeResult.StatusCode == 400) { return BadRequest("Đăng ký môn học gia sư thất bại"); }
            }
            if (result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
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
            if((IActionResult)result.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            throw new Exception("Xảy ra lỗi không xác định");
        }

       
    }

}

