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
        Task<Tutor> RegisterTutorInformation(TutorInformationRequest tutorRequest);
        Task<List<TutorSubject>> RegisterTutorSubject(Guid tutorID, List<Guid> subjectIDs);
        Task<List<TutorCertificate>> TutorCertificatesRegister(Guid tutorID, List<IFormFile> certificateImages);
        Task<TutorRegisterReponse> GetTutorRegisterInformtaion(Guid tutorID);
    }   
}
