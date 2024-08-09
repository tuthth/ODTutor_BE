using AutoMapper;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Emails;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TutorRegisterService : BaseService, ITutorRegisterService
    {

        private readonly IConfiguration _cf;
        private readonly IWebHostEnvironment _env;
        private readonly IFirebaseRealtimeDatabaseService _firebaseService;
        public TutorRegisterService(ODTutorContext odContext, IWebHostEnvironment env, IMapper mapper, IConfiguration cf, IFirebaseRealtimeDatabaseService firebaseService) : base(odContext, mapper)
        {
            _cf = cf;
            _env = env;
            _firebaseService = firebaseService;
        }

        /*Register Tutor Step By Step*/
        // Register Tutor Information
        // Step 1 : Get Information
        public async Task<ActionResult<TutorRegisterStepOneResponse>> RegisterTutorInformation(TutorInformationRequest tutorRequest, List<Guid> tutorSubjectId)
        {
            try
            {
                var user = findUserByUserID(tutorRequest.UserId);
                if (user == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "User not found", "");
                }
                //Map and save tutor information
                Tutor tutor = _mapper.Map<Tutor>(tutorRequest);
                tutor.TutorId = Guid.NewGuid();
                tutor.Status = (Int32)TutorEnum.Inprocessing; // "2" is InProcessing
                tutor.CreateAt = DateTime.UtcNow.AddHours(7);
                tutor.UpdateAt = DateTime.UtcNow.AddHours(7);
                _context.Tutors.Add(tutor);
                await _context.SaveChangesAsync();
                // Add Tutor Subject List
                List<TutorSubject> tutorSubject = new List<TutorSubject>();
                foreach (var subjectID in tutorSubjectId)
                {
                    TutorSubject tutorSubject1 = new TutorSubject();
                    tutorSubject1.TutorSubjectId = Guid.NewGuid();
                    tutorSubject1.TutorId = tutor.TutorId;
                    tutorSubject1.SubjectId = subjectID;
                    tutorSubject1.CreatedAt = DateTime.UtcNow.AddHours(7);
                    tutorSubject.Add(tutorSubject1);
                }
                if (tutorSubject.Count < 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "You have to choose at least 1 subject", "");
                }
                else
                {
                    _context.TutorSubjects.AddRange(tutorSubject);
                    await _context.SaveChangesAsync();
                }
                var response = new TutorRegisterStepOneResponse()
                {
                    TutorID = tutor.TutorId
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Register Tutor Certificate
        // Step 2 : Get Certificate
        public async Task<IActionResult> TutorCertificatesRegister(Guid tutorID, List<TutorRegisterCertificateRequest> tutorCertificateRequest)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                return new StatusCodeResult(404);
            }
            try
            {
                // List all of certificate images
                foreach (var urlLink in tutorCertificateRequest)
                {
                    TutorCertificate certificate = new TutorCertificate();
                    certificate.TutorId = tutorID;
                    certificate.ImageUrl = urlLink.ImageUrl;
                    certificate.CertificateFrom = urlLink.CertificateFrom;
                    certificate.CertificateName = urlLink.CertificateName;
                    certificate.CertificateTypeId = urlLink.CertificateTypeId;
                    certificate.CertificateDescription = urlLink.CertificateDescription;
                    certificate.StartYear = urlLink.StartYear;
                    certificate.EndYear = urlLink.EndYear;
                    certificate.IsVerified = false;
                    _context.TutorCertificates.Add(certificate);
                    await _context.SaveChangesAsync();
                }
                throw new CrudException(HttpStatusCode.Created, "Certificate is created", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Register Tutor Experience
        // Step 3 : Get Experience
        public async Task<IActionResult> RegisterTutorExperience(Guid tutorID, List<TutorExperienceRequest> tutorExperienceRegistList)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                throw new CrudException(HttpStatusCode.OK, "Not Found Tutor", "");
            }
            try
            {
                foreach (var tutorExperienceRegist in tutorExperienceRegistList)
                {
                    TutorExperience tutorExperience = _mapper.Map<TutorExperience>(tutorExperienceRegist);
                    tutorExperience.TutorExperienceId = Guid.NewGuid();
                    tutorExperience.TutorId = tutorID;
                    tutorExperience.IsVerified = false;
                    _context.TutorExperiences.Add(tutorExperience);
                }
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.Created, "Experience evidences are saved!", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //Register Tutor Sub Information
        // Step 4: Get Sub Information
        public async Task<IActionResult> RegisterSubTutorInformation(Guid tutorId, TutorSubInformationRequest tutorSubInformationRequest)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorId).FirstOrDefaultAsync();
            if (tutor == null)
            {
                throw new CrudException(HttpStatusCode.OK, "Not Found Tutor", "");
            }
            try
            {
                _mapper.Map(tutorSubInformationRequest, tutor); //error here, use for update
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.Created, "Tutor Sub Information is saved", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Check, Confirm and Send Notification
        // Step 6: Check, Confirm and Send Notification
        public async Task<IActionResult> CheckConfirmTutorInformationAndSendNotification(TutorConfirmRequest request)
        {
            try
            {
                var tutor = await _context.Tutors.Where(x => x.TutorId == request.TutorID).FirstOrDefaultAsync();
                //Update Tutor Money
                tutor.PricePerHour = request.Price;
                tutor.Status = (Int32)TutorEnum.Pending;
                tutor.HasBoughtSubscription = false;
                tutor.SubcriptionStartDate = null;
                tutor.SubcriptionEndDate = null;
                tutor.SubcriptionType = (Int32)TutorPackageEnum.Standard;
                await _context.SaveChangesAsync();
                // Create a Tutor Action Log
                TutorAction tutorRegister = new TutorAction();
                tutorRegister.TutorActionId = Guid.NewGuid();
                tutorRegister.TutorId = request.TutorID;
                tutorRegister.CreateAt = DateTime.UtcNow.AddHours(7);
                tutorRegister.Description = "Xử lý xét duyệt gia sư";
                tutorRegister.ActionType = (Int32)TutorActionTypeEnum.TutorRegister;
                tutorRegister.Status = (Int32)TutorActionEnum.Pending;
                await _context.TutorActions.AddAsync(tutorRegister);
                await _context.SaveChangesAsync();
                // Create a notification for user who want to become a tutor
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = new Guid();
                notification.UserId = tutor.UserId;
                notification.Title = "Yêu cầu xét duyệt thành gia sư đã được gửi";
                notification.Content = "Yêu cầu của bạn đã được gửi. Vui lòng đợi phản hồi qua email hoặc thông báo của hệ thống";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                notification.Status = 1; // "1" is sent
                Notification notification1x = _mapper.Map<Notification>(notification);
                await _context.Notifications.AddAsync(notification1x);
                _firebaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(201);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Register Tutor Schedule
        // Step 5: Create Schedule for Tutor

        public async Task<IActionResult> CreateTutorSlotSchedule(TutorRegistScheduleRequest tutorRegistScheduleRequest)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorRegistScheduleRequest.TutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                return new StatusCodeResult(404);
            }
            if (tutorRegistScheduleRequest.StartTime >= tutorRegistScheduleRequest.EndTime)
            {
                return new StatusCodeResult(400);
            }
            if (tutorRegistScheduleRequest.StartTime.TimeOfDay > tutorRegistScheduleRequest.EndTime.TimeOfDay)
            {
                return new StatusCodeResult(409);
            }
            try
            {
                // Calculate the number of weeks between StartTime and EndTime
                int weeks = (int)Math.Ceiling((tutorRegistScheduleRequest.EndTime - tutorRegistScheduleRequest.StartTime).TotalDays / 7);
                DateTime startOfWeek = tutorRegistScheduleRequest.StartTime.Date;
                DateTime endOfWeek;

                // Create a new TutorWeekAvailable for each week
                for (int i = 0; i < weeks; i++)
                {
                    endOfWeek = startOfWeek.AddDays(6); // End of the week is 6 days after the start of the week

                    TutorWeekAvailable tutorWeekAvailable = new TutorWeekAvailable();
                    tutorWeekAvailable.TutorWeekAvailableId = Guid.NewGuid();
                    tutorWeekAvailable.TutorId = tutorRegistScheduleRequest.TutorID;
                    tutorWeekAvailable.StartTime = startOfWeek;
                    tutorWeekAvailable.EndTime = endOfWeek < tutorRegistScheduleRequest.EndTime ? endOfWeek : tutorRegistScheduleRequest.EndTime;
                    _context.TutorWeekAvailables.Add(tutorWeekAvailable);

                    // Create a new TutorDateAvailable for each day in the current week
                    for (var date = startOfWeek; date <= tutorWeekAvailable.EndTime; date = date.AddDays(1))
                    {
                        TutorDateAvailable tutorDateAvailable = new TutorDateAvailable();
                        tutorDateAvailable.TutorDateAvailableID = Guid.NewGuid();
                        tutorDateAvailable.TutorID = tutorRegistScheduleRequest.TutorID;
                        tutorDateAvailable.TutorWeekAvailableID = tutorWeekAvailable.TutorWeekAvailableId;
                        tutorDateAvailable.Date = date;
                        tutorDateAvailable.DayOfWeek = (int)date.DayOfWeek;
                        tutorDateAvailable.StartTime = tutorRegistScheduleRequest.StartTime.TimeOfDay;
                        tutorDateAvailable.EndTime = tutorRegistScheduleRequest.EndTime.TimeOfDay;
                        _context.TutorDateAvailables.Add(tutorDateAvailable);
                        await _context.SaveChangesAsync();
                        // Generate Slot Based On Date
                        var slotList = await generateSlotBasedOnProvidedDate(tutorRegistScheduleRequest.TutorID, tutorDateAvailable);
                    }

                    startOfWeek = startOfWeek.AddDays(7); // Move to the next week
                }

                await _context.SaveChangesAsync();
                return new StatusCodeResult(201);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        /*        // Create Slots Based On Date, DayOfWeek, StartTime, EndTime
                private async Task<List<TutorSlotAvailable>> generateSlotBasedOnProvidedDate(Guid tutorID, TutorRegistDate time)
                {
                    List<TutorSlotAvailable> slotList = new List<TutorSlotAvailable>();
                    try
                    {
                        TimeSpan StartTime = time.StartTime;
                        TimeSpan EndTime = time.EndTime;
                        while (StartTime < EndTime)
                        {
                            TutorSlotAvailable tutorSlot = new TutorSlotAvailable();
                            tutorSlot.TutorSlotAvailableID = Guid.NewGuid();
                            tutorSlot.TutorDateAvailableID = time.TutorDateAvailableID;
                            tutorSlot.TutorID = tutorID;
                            tutorSlot.StartTime = StartTime;
                            tutorSlot.Status = (Int32)TutorSlotAvailabilityEnum.Available;
                            tutorSlot.IsBooked = false;

                            slotList.Add(tutorSlot);

                            StartTime = StartTime.Add(new TimeSpan(1, 0, 0));

                            // Check the time in next slot is over the end time 
                            if (StartTime >= EndTime)
                            {
                                break;
                            }
                        }
                    }*/

        // Create Tutor Schedule Part 2
        // Step 5: Create Schedule for Tutor
        public async Task<IActionResult> CreateTutorSlotInRegisterTutorStep(Guid TutorId, List<TutorRegisterSlotRequest> request)
        {
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                // Get Viet Nam Time 
                DateTime currentDateTimeUtc = DateTime.UtcNow;
                DateTime currentDateTimeVietNam = TimeZoneInfo.ConvertTimeFromUtc(currentDateTimeUtc, vietnamTimeZone);

                // Caculate the nearest Monday 
                DateTime nextMonday = GetNextMonday(currentDateTimeVietNam);
                List<DateTime> WeekDayFromMonday = new List<DateTime>();
                for (int i = 0; i < 7; i++)
                {
                    WeekDayFromMonday.Add(nextMonday.AddDays(i));
                }
                // Create Week Available
                TutorWeekAvailable tutorWeekAvailable = new TutorWeekAvailable();
                tutorWeekAvailable.TutorWeekAvailableId = Guid.NewGuid();
                tutorWeekAvailable.TutorId = TutorId;
                tutorWeekAvailable.StartTime = WeekDayFromMonday[0];
                tutorWeekAvailable.EndTime = WeekDayFromMonday[6];
                await _context.TutorWeekAvailables.AddAsync(tutorWeekAvailable);
                await _context.SaveChangesAsync();
                // Create Date Availble
                List<TutorDateAvailable> tutorDateAvailables = new List<TutorDateAvailable>();
                foreach (var date in WeekDayFromMonday)
                {
                    int dayOfWeek = (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                    var requestDate = request.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
                    if (requestDate != null)
                    {
                        foreach (var timeSlot in requestDate.TutorStartTimeEndTimRegisterRequests)
                        {
                            TutorDateAvailable tutorDateAvailable = new TutorDateAvailable
                            {
                                TutorDateAvailableID = Guid.NewGuid(),
                                TutorID = TutorId,
                                TutorWeekAvailableID = tutorWeekAvailable.TutorWeekAvailableId,
                                Date = date,
                                DayOfWeek = dayOfWeek,
                                StartTime = timeSlot.StartTime,
                                EndTime = timeSlot.EndTime
                            };
                            await _context.TutorDateAvailables.AddAsync(tutorDateAvailable);
                            await _context.SaveChangesAsync();
                            // Generate Slot Based On Date
                            var slotList = await generateSlotBasedOnProvidedDate(TutorId, tutorDateAvailable);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                throw new CrudException(HttpStatusCode.Created, "Slot is created", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }
        // Get Tutor Register Information --- Đây là nơi để dành cho những người xử lý tutor và duyệt request
        /*Đây là phần để lấy thông tin để admin hay moderator có thể hiểu và kiểm tra*/
        public async Task<ActionResult<TutorRegisterReponse>> GetTutorRegisterInformtaion(Guid tutorID)
        {
            TutorRegisterReponse response = new TutorRegisterReponse();
            List<string> subjectList = await getAllSubjectOfTutor(tutorID);
            List<string> imagesUrlList = await getAllImagesUrlOfTutor(tutorID);
            try
            {
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                User user = await _context.Tutors.Where(x => x.TutorId == tutorID).Select(x => x.UserNavigation).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    return null;
                }
                else
                {
                    response.TutorId = tutor.TutorId;
                    response.IdentityNumber = tutor.IdentityNumber;
                    response.Description = tutor.Description?.Replace("\n", "") ?? "";
                    response.PricePerHour = tutor.PricePerHour.Value;
                    response.Email = user.Email;
                    response.Username = user.Username;
                    response.ImageUrl = user.ImageUrl;
                    response.Name = user.Name;
                    response.Subjects = subjectList;
                    response.ImagesCertificateUrl = imagesUrlList;
                    response.Status = tutor.Status;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get All Tutor Information
        public async Task<ActionResult<List<TutorRegisterReponse>>> GetAllTutorRegisterInformation()
        {
            List<TutorRegisterReponse> responses = new List<TutorRegisterReponse>();
            try
            {
                List<Tutor> tutors = await _context.Tutors.Where(t => t.Status == (Int32)TutorEnum.Pending).ToListAsync();
                foreach (var tutor in tutors)
                {
                    TutorRegisterReponse response = new TutorRegisterReponse();
                    List<string> subjectList = await getAllSubjectOfTutor(tutor.TutorId);
                    List<string> imagesUrlList = await getAllImagesUrlOfTutor(tutor.TutorId);

                    User user = await _context.Users.Where(x => x.Id == tutor.UserId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        response.TutorId = tutor.TutorId;
                        response.IdentityNumber = tutor.IdentityNumber;
                        response.Description = tutor.Description?.Replace("\n", "") ?? "";
                        response.PricePerHour = tutor.PricePerHour.Value;
                        response.Email = user.Email;
                        response.Username = user.Username;
                        response.ImageUrl = user.ImageUrl;
                        response.Name = user.Name;
                        response.Subjects = subjectList;
                        response.ImagesCertificateUrl = imagesUrlList;
                        response.Status = tutor.Status;
                        responses.Add(response);
                    }
                }
                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ActionResult<TutorAction>> GetTutorActionById(Guid id)
        {
            var tutorAction = await _context.TutorActions.Where(x => x.TutorActionId == id).FirstOrDefaultAsync();
            if (tutorAction == null)
            {
                return null;
            }
            return tutorAction;
        }
        // Get Tutor Actions
        public async Task<ActionResult<PageResults<TutorAction>>> GetTutorActionByTutorId(Guid id, int size, int pageSize)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == id).FirstOrDefaultAsync();
            if (tutor == null)
            {
                return null;
            }
            try
            {
                var tutorActions = await _context.TutorActions.Where(x => x.TutorId == id).OrderByDescending(x => x.CreateAt).ToListAsync();
                var pageResults = PagingHelper<TutorAction>.Paging(tutorActions, size, pageSize);
                return pageResults;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Approval Tutor Register 
        public async Task<IActionResult> ApproveTheTutorRegister(TutorApprovalRequest request)
        {
            try
            {
                TutorAction tutorAction = _context.TutorActions.FirstOrDefault(ta => ta.TutorActionId == request.TutorActionId);
                if (tutorAction == null)
                {
                    return new StatusCodeResult(404);
                }
                tutorAction.ModeratorId = request.ApprovalID;
                tutorAction.ReponseDate = DateTime.UtcNow.AddHours(7);
                tutorAction.Status = (Int32)TutorActionEnum.Accept;
                var tutorCertificate = _context.TutorCertificates.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                foreach (var cert in tutorCertificate)
                {
                    cert.IsVerified = true;
                }
                _context.TutorCertificates.UpdateRange(tutorCertificate);
                var tutorExperience = _context.TutorExperiences.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                foreach (var exp in tutorExperience)
                {
                    exp.IsVerified = true;
                }
                _context.TutorExperiences.UpdateRange(tutorExperience);
                await _context.SaveChangesAsync();

                // Change the Status Of Tutor
                Tutor tutor = _context.Tutors.FirstOrDefault(t => t.TutorId == tutorAction.TutorId);
                tutor.Status = (Int32)TutorEnum.Active;
                await _context.SaveChangesAsync();

                // Create a notification for Tutor
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = Guid.NewGuid();
                notification.UserId = tutor.UserId;
                notification.Title = "Yêu cầu xét duyệt trở thành gia sư đã được chấp nhận";
                notification.Content = "Yêu cầu của bạn đã được chấp nhận. Bạn đã trở thành gia sư trên hệ thống";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                notification.Status = (Int32)NotificationEnum.UnRead;
                Notification notification1x = _mapper.Map<Notification>(notification);
                await _context.Notifications.AddAsync(notification1x);
                _firebaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Deny Tutor Register
        public async Task<IActionResult> DenyTheTutorRegister(TutorApprovalRequest request)
        {
            try
            {
                TutorAction tutorAction = _context.TutorActions.FirstOrDefault(ta => ta.TutorActionId == request.TutorActionId);
                if (tutorAction == null)
                {
                    return new StatusCodeResult(404);
                }
                tutorAction.ModeratorId = request.ApprovalID;
                tutorAction.ReponseDate = DateTime.UtcNow.AddHours(7);
                tutorAction.Status = (Int32)TutorActionEnum.Reject;
                await _context.SaveChangesAsync();

                // Change the Status Of Tutor
                Tutor tutor = _context.Tutors.FirstOrDefault(t => t.TutorId == tutorAction.TutorId);
                tutor.Status = (Int32)TutorEnum.Denny;
                var tutorCertificate = _context.TutorCertificates.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                _context.TutorCertificates.RemoveRange(tutorCertificate);
                var tutorExperience = _context.TutorExperiences.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                _context.TutorExperiences.RemoveRange(tutorExperience);
                await _context.SaveChangesAsync();

                // Create a notification for Tutor
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = Guid.NewGuid();
                notification.UserId = tutor.UserId;
                notification.Title = "Yêu cầu xét duyệt trở thành gia sư đã bị từ chối";
                notification.Content = "Yêu cầu của bạn đã bị từ chối. Vui lòng liên hệ với hệ thống để biết thêm thông tin.";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                notification.Status = (Int32)NotificationEnum.Deleted;
                Notification notification1x = _mapper.Map<Notification>(notification);
                await _context.Notifications.AddAsync(notification1x);
                _firebaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Tutor Step 1 By Tutor ID 
        public async Task<ActionResult<TutorRegisterStep1Response>> GetTutorStep1ByTutorID(Guid tutorID)
        {
            try
            {
                TutorRegisterStep1Response response = new TutorRegisterStep1Response();
                User user = await _context.Users.Where(x => x.TutorNavigation.TutorId == tutorID).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "User not found", "");
                }
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                response.Email = user.Email;
                response.identifyNumber = tutor.IdentityNumber;
                response.imageUrl = user.ImageUrl;
                response.Name = user.Name;
                response.Subjects = await getAllSubjectOfTutor(tutorID);
                response.videoUrl = tutor.VideoUrl;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get Tutor Step 2 By Tutor ID
        public async Task<ActionResult<List<TutorRegisterStep2Response>>> GetTutorStep2ByTutorID(Guid tutorID)
        {
            try
            {
                List<TutorRegisterStep2Response> response = new List<TutorRegisterStep2Response>();
                User user = await _context.Users.Where(x => x.TutorNavigation.TutorId == tutorID).FirstOrDefaultAsync();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (user == null || tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                List<TutorCertificate> tutorCertificateList = await _context.TutorCertificates.Where(x => x.TutorId == tutorID).ToListAsync();
                foreach (var cert in tutorCertificateList)
                {
                    response.Add(new TutorRegisterStep2Response
                    {
                        imageUrl = cert.ImageUrl,
                        CertificateDescription = cert.CertificateDescription,
                        CertifiateForm = cert.CertificateFrom,
                        CertificateName = cert.CertificateName,
                        StartYear = cert.StartYear,
                        EndYear = cert.EndYear,
                        IsVerified = cert.IsVerified
                    });
                }
                if (response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không ghi nhận chứng chỉ từ gia sư", "");
                }
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 3 By Tutor ID
        public async Task<ActionResult<List<TutorRegisterStep3Response>>> GetTutorStep3ByTutorID(Guid tutorID)
        {
            try
            {
                List<TutorRegisterStep3Response> response = new List<TutorRegisterStep3Response>();
                User user = await _context.Users.Where(x => x.TutorNavigation.TutorId == tutorID).FirstOrDefaultAsync();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (user == null || tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                List<TutorExperience> tutorExperiencesList = await _context.TutorExperiences.Where(x => x.TutorId == tutorID).ToListAsync();
                foreach (var experience in tutorExperiencesList)
                {
                    response.Add(new TutorRegisterStep3Response
                    {
                        imageUrl = experience.imageUrl,
                        Title = experience.Title,
                        Description = experience.Description,
                        Location = experience.Location,
                        StartDate = experience.StartDate,
                        EndYear = experience.EndYear,
                        IsVerified = experience.IsVerified
                    });
                }
                if (response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không ghi nhận kinh nghiệm từ gia sư", "");
                }
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 4 By Tutor ID
        public async Task<ActionResult<TutorRegisterStep4Response>> GetTutorStep4ByTutorID(Guid tutorID)
        {
            try
            {
                TutorRegisterStep4Response response = new TutorRegisterStep4Response();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                response.Description = tutor.Description;
                response.EducationExperience = tutor.EducationExperience;
                response.Motivation = tutor.Motivation;
                response.AttractiveTitle = tutor.AttractiveTitle;
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 5 By TutorID
        public async Task<ActionResult<List<TutorRegisterStep5Reponse>>> GetTutorStep5ByTutorID(Guid tutorID)
        {

            try
            {
                List<TutorRegisterStep5Reponse> response = new List<TutorRegisterStep5Reponse>();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tutor not found", "");
                }
                List<TutorWeekAvailable> tutorWeekAvailableList = await _context.TutorWeekAvailables.Where(x => x.TutorId == tutorID).ToListAsync();
                foreach (var week in tutorWeekAvailableList)
                {
                    List<TutorDateAvailable> tutorDateAvailableList = await _context.TutorDateAvailables.Where(x => x.TutorWeekAvailableID == week.TutorWeekAvailableId).ToListAsync();
                    foreach (var date in tutorDateAvailableList)
                    {
                        List<TutorSlotAvailable> tutorSlotAvailables = await _context.TutorSlotAvailables.Where(x => x.TutorDateAvailableID == date.TutorDateAvailableID && x.Status == (Int32)TutorSlotAvailabilityEnum.Available && x.IsBooked == false).ToListAsync();
                        foreach (var slot in tutorSlotAvailables)
                        {
                            response.Add(new TutorRegisterStep5Reponse
                            {
                                tutorSlotId = slot.TutorSlotAvailableID,
                                date = date.Date,
                                dayOfWeek = date.DayOfWeek,
                                startTime = slot.StartTime,
                            });
                        }
                    }
                }
                if (response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Chưa ghi nhận lịch học của gia sư", "");
                }
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 6 By TutorID
        public async Task<ActionResult<TutorRegisterStep6Response>> GetTutorStep6TutorID(Guid tutorID)
        {
            try
            {
                TutorRegisterStep6Response response = new TutorRegisterStep6Response();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                response.price = tutor.PricePerHour;
                if (tutor.PricePerHour == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Chưa ghi nhận giá tiền đăng ký", "");
                }
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        /*-------Internal Site---------*/

        // Get All Tutor Subject List
        private async Task<List<string>> getAllSubjectOfTutor(Guid TutorId)
        {
            List<string> subjectlist = new List<string>();
            try
            {
                subjectlist = _context.TutorSubjects.Where(x => x.TutorId == TutorId).Select(x => x.SubjectNavigation.Title).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return subjectlist;
        }

        // Get All Certificate Image Url
        private async Task<List<string>> getAllImagesUrlOfTutor(Guid TutorId)
        {
            List<string> imagesUrlList = new List<string>();
            try
            {
                imagesUrlList = _context.Users.Where(x => x.TutorNavigation.TutorId == TutorId)
                                .Select(x => x.TutorNavigation.TutorCertificatesNavigation
                                .Select(x => x.ImageUrl).ToList()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return imagesUrlList;
        }

        // Find User By UserID 
        private User findUserByUserID(Guid userID)
        {
            User user = _context.Users
                .FirstOrDefault(x => x.Id == userID);
            return user;
        }

        // Check the Tutor Information 

        // Check the Tutor Subject 

        private async Task<bool> checkTutorSubject(Guid tutorId)
        {
            List<TutorSubject> list = new List<TutorSubject>();
            try
            {
                list = await _context.TutorSubjects.Where(ts => ts.TutorId == tutorId).ToListAsync();
                return list.Any();
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Check the Tutor Certificate 
        private async Task<bool> checkTutorCertificate(Guid tutorId)
        {
            List<TutorCertificate> list = new List<TutorCertificate>();
            try
            {
                list = _context.TutorCertificates.Where(tc => tc.TutorId == tutorId).ToList();
                return list.Any();
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Create Tutor Slot Based On Date 
        private async Task<List<TutorSlotAvailable>> generateSlotBasedOnProvidedDate(Guid tutorID, TutorDateAvailable date)
        {
            List<TutorSlotAvailable> slotList = new List<TutorSlotAvailable>();
            try
            {
                TimeSpan StartTime = date.StartTime;
                TimeSpan EndTime = date.EndTime;
                while (StartTime < EndTime)
                {
                    TutorSlotAvailable tutorSlot = new TutorSlotAvailable();
                    tutorSlot.TutorSlotAvailableID = Guid.NewGuid();
                    tutorSlot.TutorDateAvailableID = date.TutorDateAvailableID;
                    tutorSlot.TutorID = tutorID;
                    tutorSlot.StartTime = StartTime;
                    tutorSlot.Status = (Int32)TutorSlotAvailabilityEnum.Available;
                    tutorSlot.IsBooked = false;

                    slotList.Add(tutorSlot);

                    StartTime = StartTime.Add(new TimeSpan(1, 0, 0));

                    // Check the time in next slot is over the end time 
                    if (StartTime >= EndTime)
                    {
                        break;
                    }
                }
                await _context.TutorSlotAvailables.AddRangeAsync(slotList);
                await _context.SaveChangesAsync();
                return slotList;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get the nearest Monday in Vietnam 
        private DateTime GetNextMonday(DateTime date)
        {
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
            if (daysUntilMonday == 0)
            {
                daysUntilMonday = 7;
            }
            return date.AddDays(daysUntilMonday);
        }

        // Create Tutor Subject List
        public async Task<IActionResult> CreateTutorSubjectList(TutorSubjectRegisterRequest request)
        {
            try
            {
                List<TutorSubject> tutorSubjects = new List<TutorSubject>();
                foreach (var subjectID in request.SubjectList)
                {
                    TutorSubject tutorSubject = new TutorSubject();
                    tutorSubject.TutorSubjectId = Guid.NewGuid();
                    tutorSubject.TutorId = request.TutorId;
                    tutorSubject.SubjectId = subjectID;
                    tutorSubject.CreatedAt = DateTime.UtcNow.AddHours(7);
                    tutorSubject.Status = (Int32)TutorSubjectEnum.InProgress;
                    tutorSubjects.Add(tutorSubject);
                }
                // Kiểm tra tutor có bị trùng môn đăng kí không
                var tutorSubjectList = await _context.TutorSubjects.Where(ts => ts.TutorId == request.TutorId).ToListAsync();
                foreach (var subject in tutorSubjects)
                {
                    foreach (var tutorSubject in tutorSubjectList)
                    {
                        if (subject.SubjectId == tutorSubject.SubjectId)
                        {
                            throw new CrudException(HttpStatusCode.BadRequest, "Bạn đã đăng kí môn học này rồi", "");
                        }
                    }
                }
                if (tutorSubjects.Count < 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "You have to choose at least 1 subject", "");
                }
                else
                {
                    _context.TutorSubjects.AddRange(tutorSubjects);
                    await _context.SaveChangesAsync();
                }
                throw new CrudException(HttpStatusCode.Created, "Bạn đã tạo môn học thành công", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Tutor Subject List and Paging
        public async Task<ActionResult<PageResults<TutorSubjectListResponse>>> GetTutorSubjectList(Guid tutorID, int size, int Pagesize)
        {
            List<TutorSubject> tutorSubjects = new List<TutorSubject>();
            try
            {
                tutorSubjects = await _context.TutorSubjects.Where(ts => ts.TutorId == tutorID).Include(ts => ts.SubjectNavigation).ToListAsync();
                List<TutorSubjectListResponse> response = new List<TutorSubjectListResponse>();
                foreach (var subject in tutorSubjects)
                {
                    response.Add(new TutorSubjectListResponse
                    {
                        TutorSubjectId = subject.TutorSubjectId,
                        SubjectName = subject.SubjectNavigation.Title,
                        SubjectDescription = subject.SubjectNavigation.Content,
                        CreatedDate = subject.CreatedAt,
                        Status = subject.Status
                    });
                }
                if (response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không tìm thấy môn học của gia sư", "");
                }
                var pageResults = PagingHelper<TutorSubjectListResponse>.Paging(response, size, Pagesize);
                return pageResults;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get Tutor Rating and Paging 
        public async Task<ActionResult<PageResults<TutorRatingListResponse>>> GetTutorRatingList(Guid TutorId, int size, int PageSize)
        {
            List<TutorRatingListResponse> tutorRatingListResponses = new List<TutorRatingListResponse>();
            try
            {
                var tutorRatingList = await _context.TutorRatings
                    .Where(x => x.TutorId == TutorId)
                    .ToListAsync();
                foreach (var rating in tutorRatingList)
                {
                    var student = await _context.Students
                        .Include(x => x.UserNavigation)
                        .Where(x => x.StudentId == rating.StudentId).FirstOrDefaultAsync();
                    var newResponse = new TutorRatingListResponse
                    {
                        TutorRatingId = rating.TutorRatingId,
                        FullName = student.UserNavigation.Name,
                        Image = student.UserNavigation.ImageUrl,
                        Rating = rating.RatePoints,
                        Content = rating.Content,
                        CreatedDate = rating.CreatedAt
                    };
                    tutorRatingListResponses.Add(newResponse);
                }
                if (tutorRatingListResponses.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.NoContent, "Không tìm thấy đánh giá của gia sư", "");
                }
                var pageResults = PagingHelper<TutorRatingListResponse>.Paging(tutorRatingListResponses, size, PageSize);
                return pageResults;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Remove Tutor Subject
        public async Task<IActionResult> RemoveTutorSubject(Guid tutorID, Guid subjectID)
        {
            try
            {
                var tutorSubject = await _context.TutorSubjects.Where(ts => ts.TutorId == tutorID && ts.TutorSubjectId == subjectID).FirstOrDefaultAsync();
                if (tutorSubject == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy môn học của gia sư", "");
                }
                if(tutorSubject.Status == (Int32)TutorSubjectEnum.InProgress)
                {
                    throw new CrudException(HttpStatusCode.Forbidden, "Môn học đang trong quá trình xử lý, không thể xóa", "");
                }
                if(tutorSubject.Status == (Int32)TutorSubjectEnum.Banned)
                {
                    TimeSpan timeRemaining = tutorSubject.ExpeireAt.Value - DateTime.UtcNow.AddHours(7);
                    throw new CrudException(HttpStatusCode.Conflict, "Môn học vẫn trong thời gian khóa vui lòng quay trở lại sau" + timeRemaining, "");
                }
                if (tutorSubject.Status == (Int32)TutorSubjectEnum.NotAvailable)
                {   

                    tutorSubject.Status = (Int32)TutorSubjectEnum.Available;
                }
                else if (tutorSubject.Status == (Int32)TutorSubjectEnum.Available)
                {   
                    
                    tutorSubject.Status = (Int32)TutorSubjectEnum.Banned;
                    tutorSubject.ExpeireAt = DateTime.UtcNow.AddHours(7).AddDays(14);
                }
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Cập nhật trạng thái môn học thành công", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Create A Tutor Slot By Week, Date, Time
        public async Task<IActionResult> CreateTutorSlotByWeekDate(TutorRegistScheduleRequest request)
        {
            try
            {
                // Tạo TutorWeekAvailable
                TutorWeekAvailable tutorWeekAvailable = new TutorWeekAvailable
                {
                    TutorWeekAvailableId = Guid.NewGuid(),
                    TutorId = request.TutorID,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime
                };
                await _context.TutorWeekAvailables.AddAsync(tutorWeekAvailable);

                // Tạo TutorDateAvailable và TutorSlotAvailable
                foreach (var date in request.dateList)
                {
                    int dayOfWeek = (int)date.datee.DayOfWeek == 0 ? 7 : (int)date.datee.DayOfWeek;
                    TutorDateAvailable tutorDateAvailable = new TutorDateAvailable
                    {
                        TutorDateAvailableID = Guid.NewGuid(),
                        TutorID = request.TutorID,
                        TutorWeekAvailableID = tutorWeekAvailable.TutorWeekAvailableId,
                        Date = date.datee,
                        DayOfWeek = dayOfWeek,
                        StartTime = date.timeinDate.FirstOrDefault().StartTime, // Lấy StartTime từ thời gian đầu tiên trong ngày
                        EndTime = date.timeinDate.LastOrDefault().EndTime // Lấy EndTime từ thời gian cuối cùng trong ngày
                    };
                    await _context.TutorDateAvailables.AddAsync(tutorDateAvailable);
                    await _context.SaveChangesAsync();

                    // Tạo các slot học dựa trên khoảng thời gian trong TutorStartTimeEndTimRegisterRequest
                    foreach (var time in date.timeinDate)
                    {
                        TimeSpan currentSlotStartTime = time.StartTime;
                        while (currentSlotStartTime < time.EndTime)
                        {
                            var slotEndTime = currentSlotStartTime.Add(TimeSpan.FromHours(1));
                            if (slotEndTime > time.EndTime)
                            {
                                slotEndTime= time.EndTime;
                            }
                            TutorSlotAvailable tutorSlotAvailable = new TutorSlotAvailable
                            {
                                TutorSlotAvailableID = Guid.NewGuid(),
                                TutorDateAvailableID = tutorDateAvailable.TutorDateAvailableID,
                                TutorID = request.TutorID,
                                StartTime = currentSlotStartTime,
                                IsBooked = false,
                                Status = (Int32)TutorSlotAvailabilityEnum.Available
                            };
                            await _context.TutorSlotAvailables.AddAsync(tutorSlotAvailable);
                            await _context.SaveChangesAsync();
                            currentSlotStartTime = slotEndTime;
                            if (currentSlotStartTime >= time.EndTime)
                            {
                                break;
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.Created, "Tạo lịch dạy thành công", "Thành công");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Delete Slot By TutorSlotID
        public async Task<IActionResult> DeleteSlotByTutorSlotID(Guid tutorSlotID)
        {
            try
            {
                var tutorSlot = await _context.TutorSlotAvailables.Where(ts => ts.TutorSlotAvailableID == tutorSlotID).FirstOrDefaultAsync();
                if (tutorSlot == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không tìm thấy slot học", "");
                }
                if(tutorSlot.Status == (Int32)TutorSlotAvailabilityEnum.Available)
                {
                    throw new CrudException(HttpStatusCode.OK, "Slot học đã được đặt, không thể xóa", "");
                }
                if(tutorSlot.IsBooked == true)
                {
                    throw new CrudException(HttpStatusCode.OK, "Slot học đã được đặt, không thể xóa", "");
                }
                _context.TutorSlotAvailables.Remove(tutorSlot);
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Xóa slot học thành công", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Block Tutor Status By TutorId
        public async Task<IActionResult> BlockOrUnBlockTutorByTutorID (Guid tutorId)
        {
            try
            {
                var tutor = await _context.Tutors.Where(t => t.TutorId == tutorId).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không tìm thấy gia sư", "");
                }
                // Check the tutor Booking Inprocessing 
                var tutorBooking = await _context.Bookings.Where(b => b.TutorId == tutorId && b.Status == (Int32)BookingEnum.Success || b.Status == (Int32)BookingEnum.Learning).ToListAsync();
                if (tutorBooking.Count > 0)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "Gia sư đang có lịch học đang diễn ra, không thể thay đổi trạng thái", "");
                }
                if (tutor.Status == (Int32)TutorEnum.Active)
                {
                    tutor.Status = (Int32)TutorEnum.Paused;
                }
                else if (tutor.Status == (Int32)TutorEnum.Paused)
                {
                    tutor.Status = (Int32)TutorEnum.Active;
                }
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Thay đổi trạng thái thành công", "");
            } catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Lấy tất cả các môn học mà tutor chưa đăng ký
        public async Task<ActionResult<List<SubjectView>>> GetAllSubjectWithoutTutorSubject(Guid tutorID)
        {
            try
            {
                // Lấy danh sách các môn học mà tutor đã đăng ký
                var tutorSubjectList = await _context.TutorSubjects
                    .Where(ts => ts.TutorId == tutorID)
                    .Select(ts => ts.SubjectId)
                    .ToListAsync();

                // Lấy danh sách tất cả các môn học
                var subjectList = await _context.Subjects.ToListAsync();

                // Lọc ra các môn học chưa được tutor đăng ký
                var response = subjectList
                    .Where(subject => !tutorSubjectList.Contains(subject.SubjectId))
                    .Select(subject => new SubjectView
                    {
                        SubjectId = subject.SubjectId,
                        Title = subject.Title,
                        Content = subject.Content,
                        Note = subject.Note // Giả sử Subject có thuộc tính Note
                    })
                    .ToList();
                if (response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không tìm thấy môn học", "");
                }

                throw new CrudException(HttpStatusCode.OK, "Lấy danh sách môn học thành công", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Change Status for All Tutor Subject When The Expried Day = DateTime.Now
        public async Task<IActionResult> ChangeStatusForAllTutorSubject()
        {
            try
            {
                var tutorSubjectList = await _context.TutorSubjects.ToListAsync();
                if(tutorSubjectList.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không tìm thấy môn học của gia sư", "");
                }
                foreach (var tutorSubject in tutorSubjectList)
                {
                    if (tutorSubject.Status == (Int32)TutorSubjectEnum.Banned)
                    {
                        if (tutorSubject.ExpeireAt <= DateTime.UtcNow.AddHours(7))
                        {
                            tutorSubject.Status = (Int32)TutorSubjectEnum.NotAvailable;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Cập nhật trạng thái môn học thành công", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }


        // Get All Tutor Subject List Need to solve of all Tutor Based On Tutor Action Type and Status
        // Lấy danh sách gia sư có yêu cầu đăng ký môn học
        public async Task<AcceptOrRegisterProcessOfTutorSubject> getAllTutorWantToRegisterSubject()
        {
            try
            {
                // Lấy tất cả gia sư và bao gồm thông tin người dùng liên quan
                var tutors = await _context.Tutors
                    .Include(t => t.UserNavigation)
                    .ToListAsync();

                // Danh sách người dùng sẽ được trả về
                var userViews = new List<UserView>();

                // Lặp qua danh sách gia sư để kiểm tra yêu cầu đăng ký môn học
                foreach (var tutor in tutors)
                {
                    // Tìm tất cả các yêu cầu đăng ký môn học của gia sư với ActionType và Status nhất định
                    var tutorActions = await _context.TutorActions
                        .Where(x => x.TutorId == tutor.TutorId &&
                                    x.ActionType == (int)TutorActionTypeEnum.TutorRegisterSubject &&
                                    x.Status == (int)TutorActionEnum.Pending)
                        .ToListAsync();

                    // Nếu có yêu cầu, thêm người dùng vào danh sách nếu chưa có
                    if (tutorActions.Any())
                    {
                        var user = tutor.UserNavigation;

                        // Kiểm tra xem người dùng đã có trong danh sách chưa
                        if (!userViews.Any(u => u.Id == user.Id))
                        {
                            userViews.Add(new UserView
                            {
                                Id = user.Id,
                                Name = user.Name,
                                Email = user.Email,
                                PhoneNumber = user.PhoneNumber,
                                DateOfBirth = user.DateOfBirth,
                                Status = user.Status,
                                Active = user.Active,
                                CreatedAt = user.CreatedAt,
                                IsPremium = user.IsPremium,
                                Banned = user.Banned,
                                BanExpiredAt = user.BanExpiredAt
                            });
                        }
                    }
                }
                // Tạo và trả về kết quả
                var response = new AcceptOrRegisterProcessOfTutorSubject
                {
                    TutorActionRegisterSubject = userViews
                };

                return response;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }


        // Lấy tất cả danh sách môn học của tutor đang cần duyệt đăng kí
        public async Task<ActionResult<List<TutorSubjectListResponse>>> getAllTutorSubjectNeedToApprove()
        {
            try
            {
                // Lấy tất cả các yêu cầu đăng ký môn học của gia sư
                var tutorActions = await _context.TutorActions
                    .Where(x => x.ActionType == (int)TutorActionTypeEnum.TutorRegisterSubject &&
                                                   x.Status == (int)TutorActionEnum.Pending)
                    .ToListAsync();

                // Danh sách môn học sẽ được trả về
                var tutorSubjectList = new List<TutorSubjectListResponse>();

                // Lặp qua danh sách yêu cầu đăng ký môn học để lấy thông tin môn học
                foreach (var tutorAction in tutorActions)
                {
                    // Lấy thông tin môn học của gia sư
                    var tutorSubjects = await _context.TutorSubjects
                        .Where(x => x.TutorId == tutorAction.TutorId)
                        .Include(x => x.SubjectNavigation)
                        .ToListAsync();

                    // Lặp qua danh sách môn học để thêm vào danh sách trả về
                    foreach (var tutorSubject in tutorSubjects)
                    {
                        tutorSubjectList.Add(new TutorSubjectListResponse
                        {
                            TutorSubjectId = tutorSubject.TutorSubjectId,
                            SubjectName = tutorSubject.SubjectNavigation.Title,
                            SubjectDescription = tutorSubject.SubjectNavigation.Content,
                            CreatedDate = tutorSubject.CreatedAt,
                            Status = tutorSubject.Status
                        });
                    }
                }

                // Trả về danh sách môn học
                return tutorSubjectList;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }



    }
}
