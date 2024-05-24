using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("get/{tutorId}")]
        public async Task<ActionResult<TutorAccountResponse>> GetTutorById(Guid tutorId)
        {
            var tutor = await _tutorDataService.GetTutorById(tutorId);
            if (tutor == null) return StatusCode(StatusCodes.Status404NotFound, "Không tìm thấy tài khoản, hoặc tài khoản bạn tìm đang bị đình chỉ");
            return tutor;
        }
    }
}
