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
        public TutorRegisterService(ODTutorContext odContext,IWebHostEnvironment env, IMapper mapper, IConfiguration cf) : base(odContext, mapper)
        {
            _cf = cf;
            _env = env;
        }

        // Register Tutor Information
        public async Task<IActionResult> RegisterTutorInformation(TutorInformationRequest tutorRequest)
        {
            try
            {
                Tutor tutor = _mapper.Map<Tutor>(tutorRequest);
                tutor.TutorId = new Guid();
                tutor.Status = 0; // "0" is Pending
                if (tutor == null)
                {
                    return new StatusCodeResult(404);
                }
                else
                {
                    _context.Tutors.Add(tutor);
                    await _context.SaveChangesAsync();
                    return new StatusCodeResult(200);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Register Tutor Subject
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
                    return new StatusCodeResult(201);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString()); // Replace 'StatusCodeResult' with 'BadRequestResult'
            }
        }

        // Register Tutor Certificate
        public async Task<IActionResult> TutorCertificatesRegister(Guid tutorID, List<IFormFile> certificateImages)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null) return new StatusCodeResult(404);
            try
            {   
                var tutorCertificateList = new List<TutorCertificate>();
                string apikey = _cf.GetValue<string>("ImgbbSettings:ApiKey");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", apikey);
                var certificateList = new List<string>();
                // List all of certificate images
                foreach (var image in certificateImages)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var content = new MultipartFormDataContent();
                        content.Add(new StreamContent(stream), "image", image.FileName);
                        var response = await client.PostAsync("https://api.imgbb.com/1/upload?key=" + apikey, content);
                        response.EnsureSuccessStatusCode();
                        var responseString = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(responseString);
                        var imageUrl = (string)jsonData.data.url;
                        certificateList.Add(imageUrl);
                        foreach (var urlLink in certificateList)
                        {
                            TutorCertificate certificate = new TutorCertificate();
                            certificate.TutorId = tutorID;
                            certificate.ImageUrl = urlLink;
                            _context.TutorCertificates.Add(certificate);
                            await _context.SaveChangesAsync();
                            tutorCertificateList.Add(certificate);
                        }
                    }
                }
                return new StatusCodeResult(201);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Register Tutor Experience
        public async Task<IActionResult> RegisterTutorExperience(Guid tutorID, List<TutorExperienceRequest> tutorExperienceRegistList)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                throw new CrudException( HttpStatusCode.NotFound, "Tutor not found", "");
            }
            try
            {
                foreach(var tutorExperienceRegist in tutorExperienceRegistList)
                {
                    TutorExperience tutorExperience = _mapper.Map<TutorExperience>(tutorExperienceRegist);
                    tutorExperience.TutorExperienceId = new Guid();
                    tutorExperience.TutorId = tutorID;
                    _context.TutorExperiences.Add(tutorExperience);
                }
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.Created, "Tutor Experience Created", "");
            }
            catch(CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }   

        // Get Tutor Register Information
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
                if ( tutor == null)
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
            } catch (Exception ex)
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
                subjectlist = _context.TutorSubjects.Where( x => x.TutorId == TutorId).Select(x => x.SubjectNavigation.Title).ToList();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }   
            return subjectlist;
        }

        // Get All Certificate Image Url
        private async Task<List<string>> getAllImagesUrlOfTutor( Guid TutorId)
        {
            List<string> imagesUrlList = new List<string>();
            try
            {
                imagesUrlList = _context.Users.Where(x => x.TutorNavigation.TutorId == TutorId)
                                .Select( x => x.TutorNavigation.TutorCertificatesNavigation
                                .Select(x => x.ImageUrl).ToList()).FirstOrDefault();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return imagesUrlList;
        }

        // Check the Photo of Account
        private async Task<IActionResult> checkPhotoAvatar( IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Photo is required", "");
            }
            using (var ms = new MemoryStream())
            {
                photo.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
                if (s.Length > 5 * 1024 * 1024)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Photo is too large", "");
                }
                using (var ms2 = new MemoryStream(fileBytes))
                {
                    Bitmap bitmap = new Bitmap(ms2);
                    Image<Bgr, byte> image = bitmap.ToImage<Bgr,byte>();
                    string facePath = Path.Combine(_env.WebRootPath, "haarcascade_frontalface_default.xml");
                    if(!System.IO.File.Exists(facePath))
                    {
                        throw new CrudException(HttpStatusCode.InternalServerError, "Face detection file not found", "");
                    }
                    var faceCascade = new CascadeClassifier(facePath);
                    var grayImage = image.Convert<Gray, byte>();
                    var faces = faceCascade.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);
                    if(faces.Length > 0)
                    {
                        return new OkObjectResult(new { message = "Face detected" });
                    }
                    else
                    {
                        throw new CrudException(HttpStatusCode.BadRequest, "No face detected", "");
                    }
                }
            }
        }
        // Accept Tutor + Notification

        // Deny Tutor + Notification
    }
}
