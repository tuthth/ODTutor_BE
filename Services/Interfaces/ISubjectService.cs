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
        Task<List<Subject>> GetAllSubjects();
        Task<Subject> GetSubjectById(Guid subjectId);
        Task<Subject> AddNewSubject(SubjectAddNewRequest subject);
    }
}
