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
    public interface IStudentRequestService
    {
        Task<IActionResult> CreateStudentRequest(CreateStudentRequest request);
        Task<IActionResult> UpdateStudentRequest(UpdateStudentRequest request);
        Task<ActionResult<List<StudentRequest>>> GetAllStudentRequests();
        Task<ActionResult<StudentRequest>> GetStudentRequest(Guid id);
        Task<ActionResult<List<StudentRequest>>> GetStudentRequestsByStudentId(Guid id);
        Task<ActionResult<List<StudentRequest>>> GetStudentRequestsBySubjectId(Guid id);
        Task<ActionResult<List<StudentRequestView>>> GetStudentRequestsByStatus();
        Task<ActionResult<PageResults<StudentRequest>>> GetStudentRequestsByStudentIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<PageResults<StudentRequest>>> GetStudentRequestsBySubjectIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<PageResults<StudentRequestView>>> GetStudentRequestsByStatusPaging(PagingRequest request);
        Task<ActionResult<PageResults<StudentRequest>>> GetAllStudentRequestsPaging(PagingRequest request);
    }
}
