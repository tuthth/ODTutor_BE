using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models;
using Models.Entities;
using Settings.JWT;
using Settings.Mail;
using Settings.Subscription;
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
        protected readonly IConfiguration _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        protected readonly IConfiguration _tutorSubscriptionConfiguration = new ConfigurationBuilder().AddJsonFile("tutorSubscription.json").Build();
        protected readonly IConfiguration _studentSubscriptionConfiguration = new ConfigurationBuilder().AddJsonFile("studentSubscription.json").Build();
        protected readonly IOptions<MailSetting> _mailSettings;
        protected readonly IOptions<JWTSetting> _jwtSettings;
        public BaseService(ODTutorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _mailSettings = Options.Create(_configuration.GetSection("MailSettings").Get<MailSetting>());
            _jwtSettings = Options.Create(_configuration.GetSection("AppSettings").Get<JWTSetting>());
            _appExtension = new AppExtension(_configuration, _mailSettings);
        }
    }
}
