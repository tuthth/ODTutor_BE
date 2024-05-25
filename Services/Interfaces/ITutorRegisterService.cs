using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITutorRegisterService
    {
        Task<IActionResult> RegisterTutorInformation(TutorInformationRequest tutorRequest);
        Task<IActionResult> RegisterTutorSubject(Guid tutorID, List<Guid> subjectIDs);
        Task<IActionResult> TutorCertificatesRegister(Guid tutorID, List<IFormFile> certificateImages);
        Task<ActionResult<TutorRegisterReponse>> GetTutorRegisterInformtaion(Guid tutorID);
    }   
}
