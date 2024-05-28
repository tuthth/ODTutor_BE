using AutoMapper;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Wallet, WalletView>();
            CreateMap<TutorInformationRequest, Tutor>();
            CreateMap<AccountRegisterRequest, User>();
            CreateMap<User, AccountResponse>();
            CreateMap<User, UserAccountResponse>();
            CreateMap<TutorExperienceRequest,TutorExperience>();
        }
    }
}
