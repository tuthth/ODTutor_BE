using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICertificateTypeService
    {
        Task<IActionResult> Create(CertificateTypeRequest request);
        Task<IActionResult> Update(UpdateCertificateTypeRequest request);
        Task<IActionResult> Delete(Guid certificateTypeId);
        Task<ActionResult<List<CertificateType>>> GetAll();
        Task<ActionResult<CertificateType>> GetById(Guid certificateTypeId);
        Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByCertificateId(Guid certificateTypeId);
        Task<IActionResult> ModifyCertificateTypeToTutorCertificate(CertificateTypeToTutorCertificateRequest request);
        Task<IActionResult> RemoveCertificateTypeOfTutorCertificate(Guid tutorCertificateId);
    }
}
