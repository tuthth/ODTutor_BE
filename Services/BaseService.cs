using AutoMapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BaseService
    {
        protected readonly Context _context;
        protected readonly IMapper _mapper;

        public BaseService(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
