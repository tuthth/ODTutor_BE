using Microsoft.AspNetCore.Authentication.Google;
using API.Configurations;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Mappings;
using Services;
using Services.Implementations;
using Services.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Serilog;
using Serilog.Formatting.Json;
using Microsoft.AspNetCore.RateLimiting;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (!builder.Environment.IsDevelopment())
                builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.ListenAnyIP(5260); });

            //FCM
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "capstone-c0906-firebase-adminsdk-kxbti-1521aadccf.json");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = builder.Configuration.GetValue<string>("Firebase:ProjectId")
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ODTutorContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            
            //
            builder.Services.AddDependenceInjection();
            builder.Services.AddHttpContextAccessor();
            builder.Services.VnPaySettings(builder.Configuration);
            builder.Services.MailSettings(builder.Configuration);
            builder.Services.AddJwt(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(GeneralProfile));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(policyName: "default", options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromMinutes(5);
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 100;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Google:ClientId"];
                options.ClientSecret = builder.Configuration["Google:ClientSecret"];
            });
            builder.Host.UseSerilog((ctx, config) => 
            {
                config.WriteTo.Console().MinimumLevel.Information();
                config.WriteTo.File(
                    path: AppDomain.CurrentDomain.BaseDirectory + "logs/log~.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    formatter: new JsonFormatter()).MinimumLevel.Information();
            });

            builder.Services.AddSwagger();
                
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // if (app.Environment.IsDevelopment())
            // {
                app.UseSwagger();
                app.UseSwaggerUI();
            // }
            app.UseCors("AllowAll");
            app.UseMiddleware(typeof(ProcessCrudExceptions));
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
         
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}
