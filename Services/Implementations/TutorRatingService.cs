using AutoMapper;
using Models.Entities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TutorRatingService: BaseService, ITutorRatingService
    {
        public TutorRatingService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }

        // Add Tutor Rating Service
        public Task<IAccountService> AddTutorRating()
        {
            throw new NotImplementedException();
        }
    }
}
