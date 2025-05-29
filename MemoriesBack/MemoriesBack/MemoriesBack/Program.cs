using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MemoriesBack.Data;
using MemoriesBack.Repository; // Upewnij się, że ta przestrzeń nazw zawiera interfejsy i klasy repozytoriów
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


            // Rejestracja repozytoriów i serwisów
            builder.Services.AddScoped<GradeRepository>();
            // ZMIANA TUTAJ: Mapowanie interfejsu na implementację
            builder.Services.AddScoped<IGroupMemberClassRepository, GroupMemberClassRepository>(); 
            builder.Services.AddScoped<GroupMemberRepository>();
            builder.Services.AddScoped<PasswordResetTokenRepository>();
            // ZMIANA TUTAJ: Mapowanie interfejsu na implementację
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>(); 
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
            builder.Services.AddScoped<ScheduleService>(); // Ten serwis będzie teraz poprawnie otrzymywał zależności


            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
            });

            // AddControllers() było już wyżej, można usunąć duplikat, jeśli nie ma specjalnego powodu
            // builder.Services.AddControllers(); 
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<MemoriesBack.Middlewares.ExceptionMiddleware>();

            // Apply migrations
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // Uważaj z automatyczną migracją w produkcji, ale OK dla developmentu
                db.Database.Migrate(); 
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
    
            app.UseCors("AllowAngularApp");
            app.UseRouting();
            app.UseStaticFiles(); // Jeśli serwujesz pliki statyczne
            app.UseAuthentication(); // Jeśli używasz autentykacji
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
