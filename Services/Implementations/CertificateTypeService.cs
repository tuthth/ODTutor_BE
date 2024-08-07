using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    public class CertificateTypeService : BaseService, ICertificateTypeService
    {
        public CertificateTypeService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> Create(CertificateTypeRequest request)
        {
            var certificateType = _mapper.Map<CertificateType>(request);
            var isExisted = _context.CertificateTypes.Any(x => x.Name == certificateType.Name);
            if (isExisted) return new StatusCodeResult(409);
            await _context.CertificateTypes.AddAsync(certificateType);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> Update(UpdateCertificateTypeRequest request)
        {
            var certificateType = _mapper.Map<CertificateType>(request);
            var existedCertificateType = _context.CertificateTypes.Find(certificateType.CertificateTypeId);
            if (existedCertificateType == null) return new StatusCodeResult(404);
            existedCertificateType.Name = certificateType.Name;
            existedCertificateType.Description = certificateType.Description;
            _context.CertificateTypes.Update(existedCertificateType);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> Delete(Guid certificateTypeId)
        {
            var certificateType = _context.CertificateTypes.Find(certificateTypeId);
            if (certificateType == null) return new StatusCodeResult(404);
            _context.CertificateTypes.Remove(certificateType);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<ActionResult<List<CertificateType>>> GetAll()
        {
            try
            {
                var list = _context.CertificateTypes.ToList();
                if(list == null) return new StatusCodeResult(404);
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CertificateType>> GetById(Guid certificateTypeId)
        {
            var certificateType = _context.CertificateTypes.Find(certificateTypeId);
            if (certificateType == null) return new StatusCodeResult(404);
            return certificateType;
        }
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByCertificateId(Guid certificateTypeId)
        {
           var tutorCertificates = _context.TutorCertificates.Where(x => x.CertificateTypeId == certificateTypeId).ToList();
            if (tutorCertificates == null) return new StatusCodeResult(404);
            return tutorCertificates;
        }
        public async Task<IActionResult> ModifyCertificateTypeToTutorCertificate(CertificateTypeToTutorCertificateRequest request)
        {
            var tutorCertificate = _context.TutorCertificates.Find(request.TutorCertificateId);
            if (tutorCertificate == null) return new StatusCodeResult(404);
            var certificateType = _context.CertificateTypes.Find(request.CertificateTypeId);
            if (certificateType == null) return new StatusCodeResult(404);
            tutorCertificate.CertificateTypeId = certificateType.CertificateTypeId;
            _context.TutorCertificates.Update(tutorCertificate);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> RemoveCertificateTypeOfTutorCertificate(Guid tutorCertificateId)
        {
            var tutorCertificate = _context.TutorCertificates.Find(tutorCertificateId);
            if (tutorCertificate == null) return new StatusCodeResult(404);
            tutorCertificate.CertificateTypeId = null;
            _context.TutorCertificates.Update(tutorCertificate);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
    }
}
