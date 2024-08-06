using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateTypeController : ControllerBase
    {
        private readonly ICertificateTypeService _certificateTypeService;
        private readonly IMapper _mapper;
        public CertificateTypeController(ICertificateTypeService certificateTypeService, IMapper mapper)
        {
            _certificateTypeService = certificateTypeService;
            _mapper = mapper;
        }
        [HttpGet("all")]
        public async Task<ActionResult<List<CertificateType>>> GetAll()
        {
            var rs = await _certificateTypeService.GetAll();
            if (rs is ActionResult<List<CertificateType>> certificateTypes)
            {
                if (rs.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy loại chứng chỉ"); }
                }
                if ((IActionResult)rs.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                return Ok(certificateTypes);
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("{certificateTypeId}")]
        public async Task<ActionResult<CertificateType>> GetById(Guid certificateTypeId)
        {
            var rs = await _certificateTypeService.GetById(certificateTypeId);
            if (rs is ActionResult<CertificateType> certificateType)
            {
                if (rs.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy loại chứng chỉ"); }
                }
                if ((IActionResult)rs.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                return Ok(certificateType);
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CertificateTypeRequest request)
        {
            var rs = await _certificateTypeService.Create(request);
            if (rs is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 201) { return StatusCode(StatusCodes.Status201Created, new { Message = "Loại chứng chỉ được tạo thành công" }); }
                else if (statusCodeResult.StatusCode == 409) { return Conflict(new { Message = "Loại chứng chỉ đã tồn tại" }); }
            }
            if (rs is Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateCertificateTypeRequest request)
        {
            var rs = await _certificateTypeService.Update(request);
            if (rs is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy loại chứng chỉ" }); }
                else if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật thành công" }); }
            }
            if (rs is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("delete/{certificateTypeId}")]
        public async Task<IActionResult> Delete(Guid certificateTypeId)
        {
            var rs = await _certificateTypeService.Delete(certificateTypeId);
            if (rs is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy loại chứng chỉ" }); }
                else if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Xóa thành công" }); }
            }
            if (rs is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpGet("tutor-certificates/{certificateTypeId}")]
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByCertificateId(Guid certificateTypeId)
        {
            var rs = await _certificateTypeService.GetTutorCertificatesByCertificateId(certificateTypeId);
            if (rs is ActionResult<List<TutorCertificate>> tutorCertificates)
            {
                if (rs.Result is StatusCodeResult statusCodeResult)
                {
                    if (statusCodeResult.StatusCode == 404) { return NotFound("Không tìm thấy loại chứng chỉ"); }
                }
                if ((IActionResult)rs.Result is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
                return Ok(tutorCertificates);
            }
            throw new Exception("Lỗi không xác định");
        }
        [HttpPost("modify-tutor-certificate")]
        public async Task<IActionResult> ModifyCertificateTypeToTutorCertificate([FromBody] CertificateTypeToTutorCertificateRequest request)
        {
            var rs = await _certificateTypeService.ModifyCertificateTypeToTutorCertificate(request);
            if (rs is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy loại chứng chỉ hoặc chứng chỉ của giáo viên" }); }
                else if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Cập nhật thành công" }); }
            }
            if (rs is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
        [HttpDelete("remove-tutor-certificate/{tutorCertificateId}")]
        public async Task<IActionResult> RemoveCertificateTypeOfTutorCertificate(Guid tutorCertificateId)
        {
            var rs = await _certificateTypeService.RemoveCertificateTypeOfTutorCertificate(tutorCertificateId);
            if (rs is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 404) { return NotFound(new { Message = "Không tìm thấy chứng chỉ của giáo viên" }); }
                else if (statusCodeResult.StatusCode == 200) { return Ok(new { Message = "Xóa thành công" }); }
            }
            if (rs is Exception exception) return StatusCode(StatusCodes.Status500InternalServerError, new { Message = exception.ToString() });
            throw new Exception("Lỗi không xác định");
        }
    }
}
