using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITutorManageService
    {
        Task<IActionResult> DeleteSlotInTimeRange(TutorDateRemoveRangeRequest request);
        Task<IActionResult> DeleteSlotInTimeRangeInDate(TutorDateRemoveSlotRequest request);
        Task<IActionResult> DeleteTutorSlots(List<Guid> tutorSlotAvailableIDs);
        Task<IActionResult> UpdateTutorExperience(UpdateTutorExperienceRequest request);
        Task<IActionResult> DeleteTutorExperience(Guid id);
        Task<IActionResult> UpdateTutorSubject(UpdateTutorSubjectRequest request);
        Task<IActionResult> DeleteTutorSubject(Guid id);
        Task<IActionResult> UpdateTutorCertificate(UpdateTutorCertificateRequest request);
        Task<IActionResult> DeleteTutorCertificate(Guid id);
        Task<IActionResult> DeleteRatingImage(Guid id);
        Task<IActionResult> UpdateAllCountChatOfAllTutor();
        Task<IActionResult> UpdateTutorWhenEndTimeOfPackageIsOver();
    }
}
