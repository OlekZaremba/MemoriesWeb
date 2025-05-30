using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MemoriesBack.Entities;
using MemoriesBack.Repository;

namespace MemoriesBack.Service
{
    public class PasswordResetService
    {
        private readonly SensitiveDataRepository _sensitiveDataRepository;
        private readonly EmailService _emailService;
        private readonly IPasswordHasher<User> _passwordHasher;

        
        private static readonly ConcurrentDictionary<string, SensitiveData> ResetTokens = new();

        public PasswordResetService(
            SensitiveDataRepository sensitiveDataRepository,
            EmailService emailService,
            IPasswordHasher<User> passwordHasher)
        {
            _sensitiveDataRepository = sensitiveDataRepository;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }

        public async Task RequestPasswordReset(string email)
        {
            var sensitive = await _sensitiveDataRepository.GetByEmailAsync(email);
            if (sensitive == null)
            {
                Console.WriteLine("Użytkownik o takim e-mailu nie istnieje.");
                return;
            }

            var token = Guid.NewGuid().ToString();
            ResetTokens[token] = sensitive;

            try
            {
                Console.WriteLine($"[RESET] Token dla {email}: {token}");
                await _emailService.SendPasswordResetEmailAsync(email, token);
                Console.WriteLine("E-mail z linkiem resetującym został wysłany.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy wysyłaniu e-maila: {ex.Message}");
            }
        }

        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            if (!ResetTokens.TryRemove(token, out var sensitiveData))
            {
                Console.WriteLine("Nieprawidłowy lub zużyty token.");
                return false;
            }

            var user = sensitiveData.User;
            sensitiveData.Password = _passwordHasher.HashPassword(user, newPassword);
            await _sensitiveDataRepository.UpdateAsync(sensitiveData);

            Console.WriteLine($"Hasło zaktualizowane dla użytkownika: {sensitiveData.Login}");
            return true;
        }
    }
}
