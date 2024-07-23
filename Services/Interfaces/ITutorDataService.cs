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
    public interface ITutorDataService
    {
        Task<ActionResult<List<TutorAccountResponse>>> GetAvalaibleTutors();
        Task<PageResults<TutorAccountResponse>> GetAvalaibleTutorsV2(PagingRequest pagingRequest);
        Task<ActionResult<TutorAccountResponse>> GetTutorById(Guid tutorId);
        Task<ActionResult<TutorRatingResponse>> GetTutorRating(Guid tutorId);
        Task<PageResults<TutorFeedBackResponse>> GetTutorFeedBackResponseByTutorID(Guid tutorID, PagingRequest pagingRequest);
        Task<ActionResult<List<TutorScheduleResponse>>> GetAllTutorSlotRegistered(Guid tutorID);
        Task<ActionResult<TutorSlotResponse>> GetTutorSlotAvalaibleById(Guid tutorSlotAvalaibleId);
        Task<IActionResult> UpdateTutorInformation(TutorInformationUpdate tutorInformationUpdate);
        Task<TutorCountSubjectResponse> CountAllSubjectOfTutor(Guid tutorID);
        Task<TutorCountResponse> CountTutorMoney(Guid tutorID);
        Task<ActionResult<List<StudentStatisticView>>> GetTop5StudentLearnMost(Guid tutorID);
        Task<ActionResult<List<StudentStatisticView>>> GetStudentStatisticByDayOfWeek(Guid tutorID, int dayOfWeek);
        Task<ActionResult<List<StudentStatisticView>>> GetStudentStatisticByMonthOfYear(Guid tutorID, int monthOfYear);
        Task<ActionResult<TutorView>> GetTutorByUserID(Guid UserID);
        Task<ActionResult<StudentStatisticNumberByTimeOfDatResponse>> GetNumberOfStudentPercentageByTimeOfDate(Guid tutorId);
    }
}
