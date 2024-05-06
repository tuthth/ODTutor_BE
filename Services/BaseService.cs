using AutoMapper;
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

        public BaseService(ODTutorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
