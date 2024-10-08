﻿using AutoMapper;
using Google.Cloud.Firestore;
using Grpc.Core;
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
        private ICloudFireStoreService _cloudFireStoreService;
        public TutorDataService(ODTutorContext context, IMapper mapper, ICloudFireStoreService cloudFireStoreService) : base(context, mapper)
        {
            _cloudFireStoreService = cloudFireStoreService;
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
           PricePerHour = t.PricePerHour.GetValueOrDefault(),
           Description = t.Description,
           Status = t.Status,
           VideoUrl = t.VideoUrl,
           Courses = t.CoursesNavigation.Select(c => new Course
           {
               CourseId = c.CourseId,
               Description = c.Description

           }).ToList(),
           Subjects = t.TutorSubjectsNavigation
           .Where(ts => ts.Status == (int)TutorSubjectEnum.Available)
           .Select(ts => new Subject
           {
               SubjectId = ts.SubjectId,
               Title = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Title,
               Content = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Content,
               Note = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Note
           }).ToList(),
           TutorCertificates = t.TutorCertificatesNavigation.Select(tc => new TutorCertificate
           {
               TutorCertificateId = tc.TutorCertificateId,
               CertificateTypeId = tc.CertificateTypeId,
               CertificateName = tc.CertificateName,
               CertificateFrom = tc.CertificateFrom,
               StartYear = tc.StartYear,
               EndYear = tc.EndYear
           }).ToList(),
           TutorSubjects = t.TutorSubjectsNavigation.Select(ts => new TutorSubject
           {
               TutorSubjectId = ts.TutorSubjectId,
               SubjectId = ts.SubjectId,

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
                        PricePerHour = t.PricePerHour.GetValueOrDefault(),
                        Description = t.Description,
                        Status = t.Status,
                        VideoUrl = t.VideoUrl,
                        Courses = t.CoursesNavigation.Select(c => new Course
                        {
                            CourseId = c.CourseId,
                            Description = c.Description

                        }).ToList(),
                        Subjects = t.TutorSubjectsNavigation
                        .Where(ts => ts.Status == (int)TutorSubjectEnum.Available)
                        .Select(ts => new Subject
                        {
                            SubjectId = ts.SubjectId,
                            Title = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Title,
                            Content = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Content,
                            Note = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Note
                        }).ToList(),
                        TutorCertificates = t.TutorCertificatesNavigation.Select(tc => new TutorCertificate
                        {
                            TutorCertificateId = tc.TutorCertificateId,
                            CertificateTypeId = tc.CertificateTypeId,
                            CertificateName = tc.CertificateName,
                            CertificateFrom = tc.CertificateFrom,
                            StartYear = tc.StartYear,
                            EndYear = tc.EndYear
                        }).ToList(),
                        TutorSubjects = t.TutorSubjectsNavigation.Select(ts => new TutorSubject
                        {
                            TutorSubjectId = ts.TutorSubjectId,
                            SubjectId = ts.SubjectId,

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
            }
            catch (Exception ex)
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
          PricePerHour = t.PricePerHour.GetValueOrDefault(),
          Description = t.Description,
          Status = t.Status,
          VideoUrl = t.VideoUrl,
          ActiveTitle = t.AttractiveTitle,
          EducationExperience = t.EducationExperience,
          Motivation = t.Motivation,
          HasBoughtExperiencedPackage = t.HasBoughtExperiencedPackage,
          HasBoughtSubscription = t.HasBoughtSubscription,
          SubcriptionStartDate = t.SubcriptionStartDate,
          SubcriptionEndDate = t.SubcriptionEndDate,
          SubcriptionType = t.SubcriptionType,
          CreateAt = t.CreateAt,
          Courses = t.CoursesNavigation.Select(c => new Course
          {
              CourseId = c.CourseId,
              Description = c.Description

          }).ToList(),
          Subjects = t.TutorSubjectsNavigation
          .Where(ts => ts.Status == (int)TutorSubjectEnum.Available)
          .Select(ts => new Subject
          {
              SubjectId = ts.SubjectId,
              Title = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Title,
              Content = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Content,
              Note = _context.Subjects.FirstOrDefault(s => s.SubjectId == ts.SubjectId).Note
          }).ToList(),
          TutorCertificates = t.TutorCertificatesNavigation.Select(tc => new TutorCertificate
          {
              TutorCertificateId = tc.TutorCertificateId,
              CertificateTypeId = tc.CertificateTypeId,
              CertificateName = tc.CertificateName,
              CertificateFrom = tc.CertificateFrom,
              StartYear = tc.StartYear,
              EndYear = tc.EndYear
          }).ToList(),
          TutorSubjects = t.TutorSubjectsNavigation.Select(ts => new TutorSubject
          {
              TutorSubjectId = ts.TutorSubjectId,
              SubjectId = ts.SubjectId,

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
                    throw new CrudException(HttpStatusCode.OK, "Hiện tại không ghi nhận feedback từ khách hàng!1", "");
                }
                var totalEndRating = (double)totalRating / totalRatingNumber;
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
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor Rating Error", ex.Message);
            }
        }

        // Get Tutor Rating List By Tutor ID
        public async Task<PageResults<TutorFeedBackResponse>> GetTutorFeedBackResponseByTutorID(Guid tutorID, PagingRequest pagingRequest)
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
                    throw new CrudException(HttpStatusCode.OK, "Hiện tại không ghi nhận feedback từ khách hàng!", "");
                }
                var result = PagingHelper<TutorFeedBackResponse>.Paging(tutorFeedBack, pagingRequest.Page, pagingRequest.PageSize);
                return result;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor FeedBack Response Error", ex.Message);
            }
        }

        // Get Tutor Schedule By Tutor ID
        public async Task<ActionResult<List<TutorScheduleResponse>>> GetAllTutorSlotRegistered(Guid tutorID)
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
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor Schedule Error", ex.Message);
            }
        }
        public async Task<ActionResult<TutorSlotResponse>> GetTutorSlotAvalaibleById(Guid tutorSlotAvalaibleId)
        {
            try
            {
                var tutorSLot = _context.TutorSlotAvailables.FirstOrDefault(c => c.TutorSlotAvailableID == tutorSlotAvalaibleId);
                if (tutorSLot == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor Slot Not Found", "Tutor Slot Not Found");
                }
                return _mapper.Map<TutorSlotResponse>(tutorSLot);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

        // Update Tutor Information
        public async Task<IActionResult> UpdateTutorInformation(TutorInformationUpdate tutorInformationUpdate)
        {
            try
            {
                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.TutorId == tutorInformationUpdate.TutorId);
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor Not Found", "Tutor Not Found");
                }
                tutor.PricePerHour = tutorInformationUpdate.PricePerHour;
                tutor.Description = tutorInformationUpdate.Description;
                tutor.EducationExperience = tutorInformationUpdate.EducationExperience;
                tutor.Motivation = tutorInformationUpdate.Motivation;
                tutor.AttractiveTitle = tutorInformationUpdate.AttractiveTitle;
                tutor.UpdateAt = DateTime.Now;
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Update Tutor Information Success", "Update Tutor Information Success");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Update Tutor Information Error", ex.Message);
            }
        }

        // Count All Subject of Tutor and get all Student Avatar
        public async Task<TutorCountSubjectResponse> CountAllSubjectOfTutor(Guid tutorID)
        {
            try
            {
                var tutorCountSubjectResponse = new TutorCountSubjectResponse();
                var totalSubject = await _context.TutorSubjects
                    .Where(ts => ts.TutorId == tutorID)
                    .CountAsync();
                tutorCountSubjectResponse.TotalSubject = totalSubject;
                tutorCountSubjectResponse.TutorSubjectActive = await _context.TutorSubjects
                    .Where(ts => ts.TutorId == tutorID && ts.Status == (int)TutorSubjectEnum.Available)
                    .CountAsync();
                tutorCountSubjectResponse.TutorSubjectInactive = await _context.TutorSubjects
                    .Where(ts => ts.TutorId == tutorID && ts.Status == (int)TutorSubjectEnum.NotAvailable)
                    .CountAsync();
                tutorCountSubjectResponse.TutorSubjectPending = await _context.TutorSubjects
                    .Where(ts => ts.TutorId == tutorID && ts.Status == (int)TutorSubjectEnum.InProgress)
                    .CountAsync();
                return tutorCountSubjectResponse;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Count All Subject Of Tutor Error", ex.Message);
            }
        }

        // Count Tutor Money, Tutor Student ,Tutor completed hours, Tutor Course
        public async Task<TutorCountResponse> CountTutorMoney(Guid tutorID)
        {   
            TutorCountResponse response  = new TutorCountResponse();
            try
            {   
                // Get All Student 
                var totalStudent = await _context.Bookings
                    .Where(b => b.TutorId == tutorID && b.Status == (int)BookingEnum.Finished)
                    .Select(b => b.StudentId)
                    .Distinct()
                    .CountAsync();
                response.TutorStudent = totalStudent;

                // Get All Hour
                var totalHour = await _context.Bookings
                    .Where(b => b.TutorId == tutorID && b.Status == (int)BookingEnum.Finished).CountAsync();
                double totalHourDouble = (totalHour * 50)/60;
                response.TutorHour = totalHourDouble;

                // Get All Money
                var totalMoneyBookingTransaction = await _context.BookingTransactions
                    .Include(bt => bt.BookingNavigation)
                    .Where(bt => bt.BookingNavigation.TutorId == tutorID && bt.BookingNavigation.Status == (int)BookingEnum.Finished)
                    .SumAsync(bt => bt.Amount);
                var totalMoneyCourseTransaction = await _context.CourseTransactions
                    .Include(ct => ct.CourseNavigation)
                    .Where(ct => ct.CourseNavigation.TutorId == tutorID)
                    .SumAsync(ct => ct.Amount);
                response.TutorMoney = totalMoneyBookingTransaction + totalMoneyCourseTransaction;

                // Get All Course
                var totalCourse = await _context.Courses
                    .Where(c => c.TutorId == tutorID)
                    .CountAsync();
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Count Tutor Money Error", ex.Message);
            }
        }

        // Get Top 5 Student Learn most of a tutor By tutor ID
        public async Task<ActionResult<List<StudentStatisticView>>> GetTop5StudentLearnMost(Guid tutorID)
        {
            try
            {
                var students = await _context.Bookings
                    .Where(b => b.TutorId == tutorID && b.Status == (int)BookingEnum.Finished)
                    .GroupBy(b => b.StudentId)
                    .OrderByDescending(b => b.Count())
                    .Take(5)
                    .Select(b => new StudentStatisticView
                    {
                        StudentId = b.Key,
                        StudentName = b.FirstOrDefault().StudentNavigation.UserNavigation.Name,
                        StudentAvatar = b.FirstOrDefault().StudentNavigation.UserNavigation.ImageUrl,
                        TotalHours = b.Count()
                    }).ToListAsync();
                return students;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Top 5 Student Learn Most Error", ex.Message);
            }
        }

        // Statistic all number of student who learn of a tutor and filtered by day in a week
        public async Task<ActionResult<List<StudentStatisticView>>> GetStudentStatisticByDayOfWeek(Guid tutorID, int dayOfWeek)
        {
            try
            {
                var students = _context.Bookings
                    .Include(b => b.StudentNavigation.UserNavigation) // Bổ sung Include để lấy thông tin User của Student
                    .Where(b => b.TutorId == tutorID && b.Status == (int)BookingEnum.Finished)
                    .AsEnumerable() // Chuyển đổi kết quả truy vấn thành một IEnumerable
                    .Where(b => ConvertDayOfWeek(b.CreatedAt.DayOfWeek) == dayOfWeek) // Thực hiện lọc trong bộ nhớ
                    .GroupBy(b => b.StudentId)
                    .Select(b => new StudentStatisticView
                    {
                        StudentId = b.Key,
                        StudentName = b.FirstOrDefault().StudentNavigation.UserNavigation.Name,
                        StudentAvatar = b.FirstOrDefault().StudentNavigation.UserNavigation.ImageUrl,
                        TotalHours = b.Count()
                    }).ToList(); // Sử dụng ToList() thay vì ToListAsync()

                return new ActionResult<List<StudentStatisticView>>(students);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Student Statistic By Day Of Week Error", ex.Message);
            }
        }

        // Statistic all number of student who learn of a tutor and filtered by month in a year
        public async Task<ActionResult<List<StudentStatisticView>>> GetStudentStatisticByMonthOfYear(Guid tutorID, int monthOfYear)
        {
            try
            {
                var students = _context.Bookings
                    .Include(b => b.StudentNavigation.UserNavigation) // Bổ sung Include để lấy thông tin User của Student
                    .Where(b => b.TutorId == tutorID && b.Status == (int)BookingEnum.Finished)
                    .AsEnumerable() // Chuyển đổi kết quả truy vấn thành một IEnumerable
                    .Where(b => ConvertMonthOfYear(b.CreatedAt.Month) == monthOfYear) // Thực hiện lọc trong bộ nhớ
                    .GroupBy(b => b.StudentId)
                    .Select(b => new StudentStatisticView
                    {
                        StudentId = b.Key,
                        StudentName = b.FirstOrDefault().StudentNavigation.UserNavigation.Name,
                        StudentAvatar = b.FirstOrDefault().StudentNavigation.UserNavigation.ImageUrl,
                        TotalHours = b.Count()
                    }).ToList(); // Sử dụng ToList() thay vì ToListAsync()

                return new ActionResult<List<StudentStatisticView>>(students);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Student Statistic By Month Of Year Error", ex.Message);
            }
        }


        // Get Tutor From UserID
        public async Task<ActionResult<TutorView>> GetTutorByUserID (Guid UserID)
        {
            try
            {
                var tutor = await _context.Tutors
                    .Where(t => t.UserId == UserID)
                    .Select(t => new TutorView
                    {
                        TutorId = t.TutorId,
                        UserId = t.UserId,
                        IdentityNumber = t.IdentityNumber,
                        PricePerHour = t.PricePerHour,
                        Description = t.Description,
                        Status = t.Status,
                        CreateAt = t.CreateAt,
                        UpdateAt = t.UpdateAt,
                        VideoUrl = t.VideoUrl
                    }).FirstOrDefaultAsync();
                return tutor;
            } catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor By UserID Error", ex.Message);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor By UserID Error", ex.Message);
            }
        }

        // Convert DayOfWeek to int
        private int ConvertDayOfWeek(DayOfWeek dayOfWeek)
        {
            // Chuyển đổi DayOfWeek thành giá trị số tương ứng
            return dayOfWeek switch
            {
                DayOfWeek.Monday => 2,
                DayOfWeek.Tuesday => 3,
                DayOfWeek.Wednesday => 4,
                DayOfWeek.Thursday => 5,
                DayOfWeek.Friday => 6,
                DayOfWeek.Saturday => 7,
                DayOfWeek.Sunday => 0,
                _ => -1 // Trường hợp không xác định
            };
        }

        //Convert MonthOfYear to int
        private int ConvertMonthOfYear(int monthOfYear)
        {
            return monthOfYear switch
            {
                1 => 1,
                2 => 2,
                3 => 3,
                4 => 4,
                5 => 5,
                6 => 6,
                7 => 7,
                8 => 8,
                9 => 9,
                10 => 10,
                11 => 11,
                12 => 12,
                _ => -1
            };
        }

        // Get Student Statistic By Morning, Afternoon, Evening ( Morning : From 00:00:00 - 07:00:00, Afternoon : From 08:00:00 - 15:00:00, Evening : From 16:00:00 - 23:59:59)
        public async Task<ActionResult<StudentStatisticNumberByTimeOfDatResponse>> GetNumberOfStudentPercentageByTimeOfDate (Guid tutorId)
        {
            try
            {
                var studentStatistic = new StudentStatisticNumberByTimeOfDatResponse();
                var totalStudent = await _context.Bookings
                    .Where(b => b.TutorId == tutorId && b.Status == (int)BookingEnum.Finished)
                    .CountAsync();
                var morningNumber = await _context.Bookings
                    .Where(b => b.TutorId == tutorId && b.Status == (int)BookingEnum.Finished && b.StudyTime.Value.Hour >= 0 && b.StudyTime.Value.Hour <= 7)
                    .CountAsync();
                var afternoonNumber = await _context.Bookings.Where(b => b.TutorId == tutorId && b.Status == (int)BookingEnum.Finished && b.StudyTime.Value.Hour >= 8 && b.StudyTime.Value.Hour <= 15)
                    .CountAsync();
                var eveningNumber = await _context.Bookings.Where(b => b.TutorId == tutorId && b.Status == (int)BookingEnum.Finished && b.StudyTime.Value.Hour >= 16 && b.StudyTime.Value.Hour <= 23).CountAsync();
                studentStatistic.Total = totalStudent;
                studentStatistic.MoringNumber = morningNumber/totalStudent * 100;
                studentStatistic.AfternoonNumber = afternoonNumber/totalStudent *100;
                studentStatistic.EveningNumber = eveningNumber / totalStudent * 100;
                return studentStatistic;
            } catch(CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Number Of Student Percentage By Time Of Date Error", ex.Message);
            }
        }

        // Count Tutor Message when they click to chat and plus 1 when they click 
        public async Task<ActionResult<IActionResult>> CountTutorMessage(Guid tutorId)
        {
            try
            {
                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.TutorId == tutorId);
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor Not Found", "Tutor Not Found");
                }

                // Check Limit message Chat 
                var maxMessages = tutor.HasBoughtSubscription == true ? 20 : 5;
                if (tutor.CountMessageChat >= maxMessages)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "You have reached the limit of messages", "You have reached the limit of messages");
                }
                tutor.CountMessageChat += 1;
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Count Tutor Message Success", "Count Tutor Message Success");
            }
            catch (CrudException ex)
            {
                throw ex;
            }

            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Count Tutor Message Error", ex.Message);
            }
        }

        // Get Tutor Subject From TutorId 
        public async Task<ActionResult<List<TutorSubjectResponse>>> GetTutorSubject(Guid tutorId)
        {
            try
            {
                var subjects = await _context.TutorSubjects
                    .Include(ts => ts.SubjectNavigation)
                    .Where(ts => ts.TutorId == tutorId && ts.Status == (Int32)TutorSubjectEnum.Available)
                    .Select(ts => new TutorSubjectResponse
                    {
                        TutorSubjectId = ts.TutorSubjectId,
                        TutorId = ts.TutorId,
                        Title = ts.SubjectNavigation.Title,
                    }).ToListAsync();
                return subjects;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Get Tutor Subject Error", ex.Message);
            }
        }

        // Count Message Chat of User 
        public async Task<int> GetMessagesCountToday(string userId)
        {
            // Lấy thời gian bắt đầu và kết thúc của ngày hiện tại theo UTC
            var startOfDay = DateTime.UtcNow.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            CollectionReference chatsRef = _cloudFireStoreService.GetCollectionReference("chats");
            Query query = chatsRef
                .WhereGreaterThanOrEqualTo("createAt", Timestamp.FromDateTime(startOfDay))
                .WhereLessThanOrEqualTo("createAt", Timestamp.FromDateTime(endOfDay));

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            int messageCount = 0;

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                var messages = documentSnapshot.GetValue<List<Dictionary<string, object>>>("messages");

                if (messages != null)
                {
                    foreach (var message in messages)
                    {
                        var senderId = message["senderId"].ToString();
                        var createdAtTimestamp = (Timestamp)message["createdAt"];
                        var messageCreatedAt = createdAtTimestamp.ToDateTime().ToUniversalTime();

                        if (senderId == userId && messageCreatedAt >= startOfDay && messageCreatedAt <= endOfDay)
                        {
                            messageCount++;
                        }
                    }
                }
            }

            return messageCount;
        }

    }
}
