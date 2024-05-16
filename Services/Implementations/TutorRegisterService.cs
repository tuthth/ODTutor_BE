using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Models.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TutorRegisterService : BaseService, ITutorRegisterService
    {
        private readonly ODTutorContext _odContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _cf;
        public TutorRegisterService(ODTutorContext odContext, IMapper mapper, IConfiguration cf) : base(odContext, mapper)
        {
            _odContext = odContext;
            _mapper = mapper;
            _cf = cf;
        }
        // Register Tutor Information
        public async Task<Tutor> RegisterTutorInformation(TutorInformationRequest tutorRequest)
        {
            try
            {
                Tutor tutor = _mapper.Map<Tutor>(tutorRequest);
                tutor.Status = 0; // "0" is Pending
                if (tutor == null)
                {
                    return null;
                }
                else
                {
                    _odContext.Tutors.Add(tutor);
                    await _odContext.SaveChangesAsync();
                    return tutor;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Register Tutor Subject
        public async Task<List<TutorSubject>> RegisterTutorSubject(Guid tutorID, List<Guid> subjectIDs)
        {
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
                    throw new Exception("BadRequest");
                }
                else
                {
                    _odContext.TutorSubjects.AddRange(tutorSubjects);
                    await _odContext.SaveChangesAsync();
                    return tutorSubjects;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // Replace 'StatusCodeResult' with 'BadRequestResult'
            }
        }

        // Register Tutor Certificate
        public async Task<List<TutorCertificate>> TutorCertificatesRegister(Guid tutorID, List<IFormFile> certificateImages)
        {
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
                            tutorCertificateList.Add(certificate);
                        }
                    }
                }
                return tutorCertificateList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
