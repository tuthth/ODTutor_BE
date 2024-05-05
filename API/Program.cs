
using API.Configurations;
using Models.Mappings;

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

            builder.Services.AddDependenceInjection();
            builder.Services.AddHttpContextAccessor();
            builder.Services.VnPaySettings(builder.Configuration);

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
            if (app.Environment.IsDevelopment())
            {
            }
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/api/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "production");
                c.RoutePrefix = "api/swagger";
            });
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
