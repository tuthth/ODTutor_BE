using AutoMapper;
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

        // Get All Subjects
        public async Task<List<Subject>> GetAllSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }
        // Get Subject By ID
        public async Task<Subject> GetSubjectById(Guid subjectId)
        {
            return await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == subjectId);
        }
        // Add New Subject
        public async Task<Subject> AddNewSubject(SubjectAddNewRequest subjectRequest)
        {   
            var subject = _mapper.Map<Subject>(subjectRequest);
            subject.SubjectId = Guid.NewGuid();
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return subject;
        }
        // Update Subject
        public async Task<Subject> UpdateSubject(UpdateSubject subjectRequest)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == subjectRequest.SubjectId);
            if (subject == null)
            {
                return null;
            }
            _mapper.Map(subjectRequest, subject);
            await _context.SaveChangesAsync();
            return subject;
        }
        // Delete Subject
        public async Task<bool> DeleteSubject(Guid subjectId)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == subjectId);
            if (subject == null)
            {
                return false;
            }
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
