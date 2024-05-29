﻿using AutoMapper;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Models.Emails;
using Models.Models.Requests;
using Models.Models.Views;
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
        public TutorRegisterService(ODTutorContext odContext, IWebHostEnvironment env, IMapper mapper, IConfiguration cf) : base(odContext, mapper)
        {
            _cf = cf;
            _env = env;
        }

        /*Register Tutor Step By Step*/
        // Register Tutor Information
        // Step 1 : Get Information
        public async Task<IActionResult> RegisterTutorInformation(TutorInformationRequest tutorRequest)
        {
            try
            {
                var user = findUserByUserID(tutorRequest.UserId);
                if (user == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "User not found", "");
                }
                // check the avatar photo
                if (!await checkPhotoAvatar(user.ImageUrl))
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Photo do not have human face", "");
                }
                //Map and save tutor information
                Tutor tutor = _mapper.Map<Tutor>(tutorRequest);
                tutor.TutorId = Guid.NewGuid();
                tutor.Status = 0; // "0" is Pending
                if (tutor == null)
                {
                    return new StatusCodeResult(404);
                }
                else
                {
                    _context.Tutors.Add(tutor);
                    await _context.SaveChangesAsync();
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = user.Email,
                        Subject = "Yêu cầu xét duyệt trở thành gia sư",
                        Body = "Yêu cầu của bạn đã được gửi. Vui lòng đợi phản hồi qua email hoặc thông báo của hệ thống"
                    });
                    return new StatusCodeResult(200);
                }
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

        // Register Tutor Subject
        // Step 2:  Get Subject
        public async Task<IActionResult> RegisterTutorSubject(Guid tutorID, List<Guid> subjectIDs)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null) return new StatusCodeResult(404);
            List<TutorSubject> tutorSubjects = new List<TutorSubject>();
            try
            {
                foreach (var subjectID in subjectIDs)
                {
                    TutorSubject tutorSubject = new TutorSubject();
                    tutorSubject.TutorId = tutorID;
                    tutorSubject.SubjectId = subjectID;

                    tutorSubjects.Add(tutorSubject);
                }

                if (tutorSubjects.Count < 0)
                {
                    return new StatusCodeResult(400);
                }
                else
                {
                    _context.TutorSubjects.AddRange(tutorSubjects);
                    await _context.SaveChangesAsync();
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = tutor.UserNavigation.Email,
                        Subject = "Yêu cầu xét duyệt môn học trở thành gia sư",
                        Body = "Yêu cầu của bạn đã được gửi. Vui lòng đợi phản hồi qua email hoặc thông báo của hệ thống"
                    });
                    return new StatusCodeResult(201);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString()); // Replace 'StatusCodeResult' with 'BadRequestResult'
            }
        }

        // Register Tutor Certificate
        // Step 3 : Get Certificate
        public async Task<IActionResult> TutorCertificatesRegister(Guid tutorID, List<IFormFile> certificateImages)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null) return new StatusCodeResult(404);
            try
            {
                var tutorCertificateList = new List<TutorCertificate>();

                var certificateList = await _appExtension.UploadImagesToImgBB(certificateImages);
                // List all of certificate images
                foreach (var urlLink in certificateList)
                {
                    TutorCertificate certificate = new TutorCertificate();
                    certificate.TutorId = tutorID;
                    certificate.ImageUrl = urlLink;
                    _context.TutorCertificates.Add(certificate);
                    await _context.SaveChangesAsync();
                    tutorCertificateList.Add(certificate);
                }
                await _appExtension.SendMail(new MailContent()
                {
                    To = tutor.UserNavigation.Email,
                    Subject = "Yêu cầu xét duyệt chứng chỉ trở thành gia sư",
                    Body = "Yêu cầu của bạn đã được gửi. Vui lòng đợi phản hồi qua email hoặc thông báo của hệ thống"
                });
                return new StatusCodeResult(201);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Register Tutor Experience
        // Step 4 : Get Experience
        public async Task<IActionResult> RegisterTutorExperience(Guid tutorID, List<TutorExperienceRequest> tutorExperienceRegistList)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tutor not found", "");
            }
            try
            {
                foreach (var tutorExperienceRegist in tutorExperienceRegistList)
                {
                    TutorExperience tutorExperience = _mapper.Map<TutorExperience>(tutorExperienceRegist);
                    tutorExperience.TutorExperienceId = new Guid();
                    tutorExperience.TutorId = tutorID;
                    _context.TutorExperiences.Add(tutorExperience);
                }
                await _context.SaveChangesAsync();
                await _appExtension.SendMail(new MailContent()
                {
                    To = tutor.UserNavigation.Email,
                    Subject = "Yêu cầu xét duyệt kinh nghiệm dạy môn học trở thành gia sư",
                    Body = "Yêu cầu của bạn đã được gửi. Vui lòng đợi phản hồi qua email hoặc thông báo của hệ thống"
                });
                throw new CrudException(HttpStatusCode.Created, "Tutor Experience Created", "");
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
        // Step 5: Check, Confirm and Send Notification
        public async Task<IActionResult> CheckConfirmTutorInformationAndSendNotification(Guid tutorID)
        {       
            try
            {   
                var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (!await checkTutorCertificate(tutorID))
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Tutor Certificate is required", "");
                }
                if (!await checkTutorSubject(tutorID))
                {
                    throw new CrudException (HttpStatusCode.BadRequest, "Tutor Subject is required", "");
                }

                // Create a Tutor Action Log

                // Create a notification for user who want to become a tutor
                Notification notification = new Notification();
                notification.NotificationId = new Guid();
                notification.UserId = tutor.UserId;
                notification.Title =  "Yêu cầu xét duyệt thành gia sư đã được gửi";
                notification.Content = "Yêu cầu của bạn đã được gửi. Vui lòng đợi phản hồi qua email hoặc thông báo của hệ thống";
                notification.CreatedAt = DateTime.Now;
                notification.Status = 1; // "1" is sent
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                throw new CrudException (HttpStatusCode.Created, "Register Sent", "");
            } catch(CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
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
                    response.IdentityNumber = tutor.IdentityNumber;
                    response.Level = tutor.Level;
                    response.Description = tutor.Description;
                    response.PricePerHour = tutor.PricePerHour;
                    response.Email = user.Email;
                    response.Username = user.Username;
                    response.ImageUrl = user.ImageUrl;
                    response.Name = user.Name;
                    response.Subjects = subjectList;
                    response.ImagesCertificateUrl = imagesUrlList;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

        // Check the Photo of Account
        private async Task<bool> checkPhotoAvatar(string base64Photo)
        {
            if (string.IsNullOrEmpty(base64Photo))
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Photo is required", "");
            }
            // Chuyển đổi chuỗi base64 thành mảng byte
            byte[] fileBytes;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    fileBytes = await httpClient.GetByteArrayAsync(base64Photo);
                }
            }
            catch (FormatException)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Invalid base64 string", "");
            }

            if (fileBytes.Length > 5 * 1024 * 1024)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Photo is too large", "");
            }

            using (var ms = new MemoryStream(fileBytes))
            {
                Bitmap bitmap = new Bitmap(ms);
                Image<Bgr, byte> image = bitmap.ToImage<Bgr, byte>();
                string facePath = Path.Combine(_env.WebRootPath, "haarcascade_frontalface_default.xml");

                if (!System.IO.File.Exists(facePath))
                {
                    throw new CrudException(HttpStatusCode.InternalServerError, "Face detection file not found", "");
                }

                var faceCascade = new CascadeClassifier(facePath);
                var grayImage = image.Convert<Gray, byte>();
                var faces = faceCascade.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);
                return faces.Length > 0;
            }
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
                list = await _context.TutorSubjects.Where( ts => ts.TutorId == tutorId).ToListAsync();
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
        private async Task<bool> checkTutorCertificate (Guid tutorId)
        {   
            List<TutorCertificate>list = new List<TutorCertificate>();
            try
            {
                list = _context.TutorCertificates.Where(tc => tc.TutorId == tutorId).ToList();
                return list.Any();
            } catch(CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Accept Tutor + Notification

        // Deny Tutor + Notification
    }
}
