using AutoMapper;
using Microsoft.Extensions.Configuration;
using Models;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BaseService
    {
        protected readonly ODTutorContext _context;
        protected readonly IMapper _mapper;
        protected readonly AppExtension _appExtension;
        protected readonly IConfiguration _configuration;
        public BaseService(ODTutorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _appExtension = new AppExtension(_configuration);
        }
    }
}
