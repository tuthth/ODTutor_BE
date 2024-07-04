using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Models.Requests;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class SubjectService : BaseService, ISubjectService
    {
        public SubjectService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        { 
        }
        public async Task<ActionResult<List<Subject>>> GetAllSubjects()
        {
            try
            {
                var subjects = await _context.Subjects.ToListAsync();
                if (subjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return subjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Subject>> GetSubject(Guid id)
        {
            try
            {
                var subject = await _context.Subjects.FirstOrDefaultAsync(c => c.SubjectId == id);
                if (subject == null)
                {
                    return new StatusCodeResult(404);
                }
                return subject;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> AddNewSubject(SubjectAddNewRequest subjectRequest)
        {
            var checkSubject = await _context.Subjects.FirstOrDefaultAsync(x => x.Title.ToLower().Equals(subjectRequest.Title.ToLower()));
            if (checkSubject != null)
            {
                return new StatusCodeResult(409);
            }
            var subject = _mapper.Map<Subject>(subjectRequest);
            subject.SubjectId = Guid.NewGuid();
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> UpdateSubject(UpdateSubject subjectRequest)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == subjectRequest.SubjectId);
            if (subject == null)
            {
                return new StatusCodeResult(404);
            }
            _mapper.Map(subjectRequest, subject);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        // Delete Subject
        public async Task<IActionResult> DeleteSubject(Guid subjectId)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == subjectId);
            var tutorSubjects = await _context.TutorSubjects.Where(x => x.SubjectId == subjectId).ToListAsync();
            var studentRequests = await _context.StudentRequests.Where(x => x.SubjectId == subjectId).ToListAsync();
            if(studentRequests != null || tutorSubjects != null)
            {
                return new StatusCodeResult(400);
            }
            if(subject == null)
            {
                return new StatusCodeResult(404);
            }
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(204);
        }
    }
}
