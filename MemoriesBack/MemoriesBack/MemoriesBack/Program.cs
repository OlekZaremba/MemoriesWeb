using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Repository;
using MemoriesBack.Entities;
using Microsoft.AspNetCore.Identity;
using MemoriesBack.Service;
using System.Text.Json.Serialization;


namespace MemoriesBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
                ));
            
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


            builder.Services.AddScoped<GradeRepository>();
            builder.Services.AddScoped<GroupMemberClassRepository>();
            builder.Services.AddScoped<GroupMemberRepository>();
            builder.Services.AddScoped<PasswordResetTokenRepository>();
            builder.Services.AddScoped<ScheduleRepository>();
            builder.Services.AddScoped<SchoolClassRepository>();
            builder.Services.AddScoped<SensitiveDataRepository>();
            builder.Services.AddScoped<UserGroupRepository>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<PasswordResetService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<AssignmentService>();
            builder.Services.AddScoped<UserGroupService>();
            builder.Services.AddScoped<GroupMemberClassService>();
            builder.Services.AddScoped<GradeService>();
            builder.Services.AddScoped<ScheduleService>();


            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<MemoriesBack.Middlewares.ExceptionMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
    
            app.UseCors("AllowAngularApp");
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
