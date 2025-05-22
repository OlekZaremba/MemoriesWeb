using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using System;
using MemoriesBack.Repository;
using MemoriesBack.Entities;
using Microsoft.AspNetCore.Identity;

namespace MemoriesBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
                ));

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




            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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
            
            app.UseAuthorization(); 
            
            Console.WriteLine();


            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
