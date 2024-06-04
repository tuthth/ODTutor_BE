using Microsoft.AspNetCore.Mvc;
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
    }
}
