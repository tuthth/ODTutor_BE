﻿using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IStudentCourseService
    {
        Task<IActionResult> UpdateStudentCourse(UpdateStudentCourseRequest request);
        Task<IActionResult> FinishStudentCourse(Guid studentCourseId);
        Task<ActionResult<List<StudentCourse>>> GetAllStudentCourses();
        Task<ActionResult<StudentCourse>> GetStudentCourse(Guid id);
        Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByCourseId(Guid id);
        Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByStudentId(Guid id);
    }
}
