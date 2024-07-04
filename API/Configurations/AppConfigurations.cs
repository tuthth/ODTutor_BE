using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Implementations;
using Services.Interfaces;
using Settings.JWT;
using Settings.Mail;
using Settings.VNPay;
using System.Reflection;
using System.Text;

namespace API.Configurations
{
    public static class AppConfigurations
    {
        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            var appsettings = configuration.GetSection("AppSettings");
            services.Configure<JWTSetting>(appsettings);


            var secretKey = configuration["AppSettings:SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        public static void VnPaySettings(this IServiceCollection services, IConfiguration _configuration)
        {
            services.AddOptions();
            var vnpaysettings = _configuration.GetSection("VnPaySettings");
            services.Configure<VNPaySetting>(vnpaysettings);
        }

        public static void MailSettings(this IServiceCollection services, IConfiguration _configuration)
        {
            services.AddOptions();
            var mailsettings = _configuration.GetSection("MailSettings");
            services.Configure<MailSetting>(mailsettings);
        }

        public static void AddDependenceInjection(this IServiceCollection services)
        {
            services.AddScoped<ISendMailService, SendMailService>();
            services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ITutorRegisterService, TutorRegisterService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITutorDataService, TutorDataService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IStudentRequestService, StudentRequestService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ITutorManageService, TutorManageService>();
            services.AddScoped<IFirebaseMessagingService, FirebaseMessagingService>();
            services.AddScoped<IStudentCourseService, StudentCourseService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IFirebaseRealtimeDatabaseService, FirebaseRealtimeDatabaseService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICloudFireStoreService, CloudFireStoreService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IUserInteractService, UserInteractionService>();
        }
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "On-Demand Tutor Service Interface", Description = "APIs for ODTutor Application", Version = "v1" });
                c.DescribeAllParametersInCamelCase();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter your token below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                 });
            });
        }
    }
}
