using Microsoft.AspNetCore.Mvc;
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
        Task<ActionResult<TutorAccountResponse>> GetTutorById(Guid tutorId);
    }
}
