using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
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
        [HttpPost("registerInformation")]
        public async Task<IActionResult> addRegisterInformationOfTutor([FromBody] TutorInformationRequest tutorRequest)
        {
            try
            {
                var tutor = await _tutorRegisterService.RegisterTutorInformation(tutorRequest);
                if (tutor != null)
                {
                    return Ok(tutor);
                }
                else
                {
                    return BadRequest("Đăng ký thông tin gia sư thất bại");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Add Certificate
        [HttpPost("registerCertificate")]
        public async Task<IActionResult> addRegisterCertificateOfTutor(Guid tutorID, List<IFormFile> tutorRequest)
        {
            try
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
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get All Tutor Register Information
        [HttpGet("getTutorRegisterInformation")]
        public async Task<IActionResult> getAllTutorRegisterInformation(Guid tutorID)
        {
            try
            {
                var tutorRegister = await _tutorRegisterService.GetTutorRegisterInformtaion(tutorID);
                if (tutorRegister != null)
                {
                    return Ok(tutorRegister);
                }
                else
                {
                    return BadRequest("Không tìm thấy thông tin đăng ký của gia sư");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
    }

}

