using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TutorDataService : BaseService, ITutorDataService
    {
        public TutorDataService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        // Get All Tutors Version 1
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

        // Get All Tutors Version 2
        public async Task<PageResults<TutorAccountResponse>> GetAvalaibleTutorsV2(PagingRequest pagingRequest)
        {
            try
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
                var result = PagingHelper<TutorAccountResponse>.Paging(tutors, pagingRequest.Page, pagingRequest.PageSize);
                return result;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Available Tutor List Error ", ex.Message);
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Account Tutor List Error", ex.Message);
            }
        }

        // Get Tutor By ID
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
