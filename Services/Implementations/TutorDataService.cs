using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
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
           UserId = t.UserId,
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
                        UserId = t.UserId,
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
      .Where(t => (t.TutorId == tutorId || t.UserId == tutorId) && t.UserNavigation.Banned == false)
      .Select(t => new TutorAccountResponse
      {
          UserId = t.UserId,
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

        // Get Tutor Rating By Tutor ID
        public async Task<ActionResult<TutorRatingResponse>> GetTutorRating(Guid tutorId)
        {
            try
            {   
                var response = new TutorRatingResponse();
                if (tutorId == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Tutor ID is required or invalid", "Tutor ID is required");
                }
                // Get All Tutor Rating
                var tutorRatings = await _context.TutorRatings.Where(tr => tr.TutorId == tutorId).ToListAsync();

                // Get All Tutor Rating One Star
                var tutorRatingOneStart = tutorRatings.Where(tr => tr.RatePoints == (int)TutorRatingStartEnum.OneStart).ToList();

                // Get All Tutor Rating Two Star
                var tutorRatingTwoStart = tutorRatings.Where(tr => tr.RatePoints == (int)TutorRatingStartEnum.TwoStart).ToList();

                // Get All Tutor Rating Three Star
                var tutorRatingThreeStart = tutorRatings.Where(tr => tr.RatePoints == (int)TutorRatingStartEnum.ThreeStart).ToList();

                // Get All Tutor Rating Four Star
                var tutorRatingFourStart = tutorRatings.Where(tr => tr.RatePoints == (int)TutorRatingStartEnum.FourStart).ToList();

                // Get All Tutor Rating Five Star
                var tutorRatingFiveStart = tutorRatings.Where(tr => tr.RatePoints == (int)TutorRatingStartEnum.FiveStart).ToList();

                // Calculate Sum Total Rating
                var totalRating = tutorRatings.Sum(tr => tr.RatePoints);
                // Calculate Number Of Total Rating
                var totalRatingNumber = tutorRatings.Count();
                // Calculate Total End Rating
                if (totalRating == 0)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Hiện tại không ghi nhận feedback từ khách hàng!1", "");
                }
                var totalEndRating = (double) totalRating / totalRatingNumber;      
                response.TutorId = tutorId;
                response.TotalRatingNumber = tutorRatings.Count();
                response.TotalRatingNumberOneStart = tutorRatingOneStart.Count();
                response.TotalRatingNumberTwoStart = tutorRatingTwoStart.Count();
                response.TotalRatingNumberThreeStart = tutorRatingThreeStart.Count();
                response.TotalRatingNumberFourStart = tutorRatingFourStart.Count();
                response.TotalRatingNumberFiveStart = tutorRatingFiveStart.Count();
                response.TotalEndRating = totalEndRating;
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor Rating Error", ex.Message);
            }
        }

        // Get Tutor Rating List By Tutor ID
        public async Task<PageResults<TutorFeedBackResponse>> GetTutorFeedBackResponseByTutorID ( Guid tutorID, PagingRequest pagingRequest)
        {
            try
            {   
                if (tutorID == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Tutor ID is required or invalid", "Tutor ID is required");
                }
                var tutorFeedBack = await _context.TutorRatings
                    .Where(tr => tr.TutorId == tutorID)
                    .Select(tr => new TutorFeedBackResponse
                    {
                        TutorRatingId = tr.TutorRatingId,
                        TutorID = tr.TutorId,
                        StudentID = tr.StudentId,
                        BookingID = tr.BookingId,
                        StudentAvatar = tr.StudentNavigation.UserNavigation.ImageUrl,
                        StudentName = tr.StudentNavigation.UserNavigation.Name,
                        CreateAt = tr.CreatedAt,
                        RatePoints = tr.RatePoints,
                        Content = tr.Content
                    }).ToListAsync();
                if (tutorFeedBack.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Hiện tại không ghi nhận feedback từ khách hàng!", "");
                }
                var result = PagingHelper<TutorFeedBackResponse>.Paging(tutorFeedBack, pagingRequest.Page, pagingRequest.PageSize);
                return result;
            } catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor FeedBack Response Error", ex.Message);
            }
        }

        // Get Tutor Schedule By Tutor ID
        public async Task<ActionResult<List<TutorScheduleResponse>>> GetAllTutorSlotRegistered (Guid tutorID)
        {
            try
            {
                var tutorSchedule = await _context.TutorWeekAvailables
                    .Where(ts => ts.TutorId == tutorID)
                    .Select(ts => new TutorScheduleResponse
                    {
                        TutorID = ts.TutorId,
                        StartTime = ts.StartTime,
                        EndTime = ts.EndTime,
                        TutorSlots = ts.TutorDateAvailables.SelectMany(tda => tda.TutorSlotAvailables).Select(tsa => new TutorSlotResponse
                        {
                            TutorSlotID = tsa.TutorSlotAvailableID,
                            Date = tsa.TutorDateAvailable.Date,
                            DayOfWeek = tsa.TutorDateAvailable.DayOfWeek,
                            StartTime = tsa.StartTime,
                            IsBooked = tsa.IsBooked,
                            Status = tsa.Status
                        }).ToList()
                    }).ToListAsync();
                return tutorSchedule;
            } catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor Schedule Error", ex.Message);
            }
        }

    }
}
