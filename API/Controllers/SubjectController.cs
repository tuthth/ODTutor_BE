﻿using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // Get All Subject 
        [HttpGet("get/allSubject")]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjects();
            return Ok(subjects);
        }

        // Get Each Subject By ID
        [HttpGet("get/subject/{subjectId}")]
        public async Task<IActionResult> GetSubjectById(Guid subjectId)
        {
            var subject = await _subjectService.GetSubjectById(subjectId);
            return Ok(subject);
        }

        // Add New Subject
        [HttpPost("add/subject")]
        public async Task<IActionResult> AddNewSubject([FromBody] SubjectAddNewRequest subject)
        {
            var newSubject = await _subjectService.AddNewSubject(subject);
            return Ok(newSubject);
        }
    }
}
