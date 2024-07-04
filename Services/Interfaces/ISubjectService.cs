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
    public interface ISubjectService
    {
        Task<IActionResult> AddNewSubject(SubjectAddNewRequest subject);
        Task<IActionResult> UpdateSubject(UpdateSubject subjectRequest);
        Task<IActionResult> DeleteSubject(Guid subjectId);
        Task<ActionResult<List<Subject>>> GetAllSubjects();
        Task<ActionResult<Subject>> GetSubject(Guid id);
    }
}
