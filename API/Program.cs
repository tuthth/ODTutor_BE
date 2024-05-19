
using API.Configurations;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Mappings;
using Services;
using Services.Implementations;
using Services.Interfaces;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

         
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
