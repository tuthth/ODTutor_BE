using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Models.Views;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TutorDataService : BaseService, ITutorDataService
    {
        public TutorDataService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<ActionResult<List<TutorAccountResponse>>> GetAvalaibleTutors()
        {
            var tutors = await _context.Tutors
       .Where(t => t.UserNavigation.Banned == false)
       .Select(t => new TutorAccountResponse
       {
           TutorId = t.TutorId,
           Level = t.Level,
           PricePerHour = t.PricePerHour.Value,
           Description = t.Description,
           Status = t.Status,
           VideoUrl = t.VideoUrl,
           Courses = t.CoursesNavigation.Select(c => new Course
           {
               CourseId = c.CourseId
           }).ToList(),
           TutorCertificates = t.TutorCertificatesNavigation.Select(tc => new TutorCertificate
           {
               TutorCertificateId = tc.TutorCertificateId,
           }).ToList(),
           Subjects = t.TutorSubjectsNavigation.Select(ts => new TutorSubject
           {
               TutorSubjectId = ts.TutorSubjectId,
               SubjectId = ts.SubjectId
           }).ToList(),
           Ratings = t.TutorRatingsNavigation.Select(tr => new TutorRating
           {
               TutorRatingId = tr.TutorRatingId,
               RatePoints = tr.RatePoints,
               Content = tr.Content,
           }).ToList()
       }).ToListAsync();
            return tutors;
        }
        public async Task<ActionResult<TutorAccountResponse>> GetTutorById(Guid tutorId)
        {
            var tutor = await _context.Tutors
      .Where(t => t.TutorId == tutorId && t.UserNavigation.Banned == false)
      .Select(t => new TutorAccountResponse
      {
          TutorId = t.TutorId,
          Level = t.Level,
          PricePerHour = t.PricePerHour.Value,
          Description = t.Description,
          Status = t.Status,
          VideoUrl = t.VideoUrl,
          Courses = t.CoursesNavigation.Select(c => new Course
          {
              CourseId = c.CourseId
          }).ToList(),
          TutorCertificates = t.TutorCertificatesNavigation.Select(tc => new TutorCertificate
          {
              TutorCertificateId = tc.TutorCertificateId,
          }).ToList(),
          Subjects = t.TutorSubjectsNavigation.Select(ts => new TutorSubject
          {
              TutorSubjectId = ts.TutorSubjectId,
              SubjectId = ts.SubjectId
          }).ToList(),
          Ratings = t.TutorRatingsNavigation.Select(tr => new TutorRating
          {
              TutorRatingId = tr.TutorRatingId,
              RatePoints = tr.RatePoints,
              Content = tr.Content,
          }).ToList()
      }).FirstOrDefaultAsync();
            if (tutor == null) return new StatusCodeResult(404);
            return tutor;
        }
    }
}
