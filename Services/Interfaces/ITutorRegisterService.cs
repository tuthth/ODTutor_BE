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
        Task<ActionResult<TutorRegisterStepOneResponse>> RegisterTutorInformation(TutorInformationRequest tutorRequest, List<Guid> tutorSubjectId);
        Task<IActionResult> TutorCertificatesRegister(Guid tutorID, List<TutorRegisterCertificateRequest> tutorCertificateRequest);
        Task<IActionResult> RegisterTutorExperience(Guid tutorID, List<TutorExperienceRequest> tutorExperienceRegistList);
        Task<IActionResult> CheckConfirmTutorInformationAndSendNotification(TutorConfirmRequest request);
        Task<ActionResult<TutorRegisterReponse>> GetTutorRegisterInformtaion(Guid tutorID);
        Task<IActionResult> CreateTutorSlotSchedule(TutorRegistScheduleRequest tutorRegistScheduleRequest);
        Task<IActionResult> ApproveTheTutorRegister(TutorApprovalRequest request);
        Task<IActionResult> DenyTheTutorRegister(TutorApprovalRequest request);
        Task<ActionResult<List<TutorRegisterReponse>>> GetAllTutorRegisterInformation();
        Task<IActionResult> CreateTutorSlotInRegisterTutorStep(Guid TutorId, List<TutorRegisterSlotRequest> request);
        Task<IActionResult> RegisterSubTutorInformation(Guid tutorId, TutorSubInformationRequest tutorSubInformationRequest);
        Task<ActionResult<TutorRegisterStep1Response>> GetTutorStep1ByTutorID(Guid tutorID);
        Task<ActionResult<List<TutorRegisterStep2Response>>> GetTutorStep2ByTutorID(Guid tutorID);
        Task<ActionResult<List<TutorRegisterStep3Response>>> GetTutorStep3ByTutorID(Guid tutorID);
        Task<ActionResult<TutorRegisterStep4Response>> GetTutorStep4ByTutorID(Guid tutorID);
        Task<ActionResult<List<TutorRegisterStep5Reponse>>> GetTutorStep5ByTutorID(Guid tutorID);
        Task<ActionResult<TutorRegisterStep6Response>> GetTutorStep6TutorID(Guid tutorID);
        Task<ActionResult<PageResults<TutorAction>>> GetTutorActionByTutorId(Guid id, int size, int pageSize);
        Task<ActionResult<TutorAction>> GetTutorActionById(Guid id);
        Task<IActionResult> CreateTutorSubjectList(TutorSubjectRegisterRequest request);
        Task<ActionResult<PageResults<TutorSubjectListResponse>>> GetTutorSubjectList(Guid tutorID, int size, int Pagesize);
        Task<IActionResult> RemoveTutorSubject(Guid tutorID, Guid subjectID);
        Task<IActionResult> CreateTutorSlotByWeekDate(TutorRegistScheduleRequest request);
        Task<IActionResult> DeleteSlotByTutorSlotID(Guid tutorSlotID);
        Task<ActionResult<PageResults<TutorRatingListResponse>>> GetTutorRatingList(Guid TutorId, int size, int PageSize);
        Task<IActionResult> BlockOrUnBlockTutorByTutorID(Guid tutorId);
    }   
}
