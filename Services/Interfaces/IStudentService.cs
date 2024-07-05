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
    public interface IStudentService
    {
        Task<ActionResult<List<Student>>> GetAllStudents();
        Task<ActionResult<PageResults<Student>>> GetAllStudentsPaging(PagingRequest request);
        Task<ActionResult<Student>> GetStudent(Guid id);
    }
}
