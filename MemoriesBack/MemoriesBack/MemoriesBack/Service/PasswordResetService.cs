using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MemoriesBack.Entities;
using MemoriesBack.Repository;

namespace MemoriesBack.Service
{
    public class PasswordResetService
    {
        private readonly UserRepository _userRepository;
        private readonly SensitiveDataRepository _sensitiveDataRepository;
        private readonly PasswordResetTokenRepository _tokenRepository;
        private readonly EmailService _emailService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public PasswordResetService(
            UserRepository userRepository,
            SensitiveDataRepository sensitiveDataRepository,
            PasswordResetTokenRepository tokenRepository,
            EmailService emailService,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _sensitiveDataRepository = sensitiveDataRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }

        public async Task RequestPasswordReset(string login)
        {
            var sensitive = await _sensitiveDataRepository.GetByLoginAsync(login);
            if (sensitive == null) return;

            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddMinutes(30);

            var resetToken = new PasswordResetToken
            {
                Token = token,
                User = sensitive.User,
                ExpiryDate = expiry
            };

            await _tokenRepository.AddAsync(resetToken);
            await _emailService.SendPasswordResetEmailAsync(login, token);
        }

        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            var resetToken = await _tokenRepository.GetByTokenAsync(token);
            if (resetToken == null) return false;

            if (resetToken.ExpiryDate < DateTime.UtcNow)
            {
                await _tokenRepository.DeleteAsync(resetToken);
                return false;
            }

            var user = resetToken.User;
            var sensitiveData = await _sensitiveDataRepository.GetByUserAsync(user);
            if (sensitiveData == null)
                throw new InvalidOperationException("Brak danych logowania");

            sensitiveData.Password = _passwordHasher.HashPassword(user, newPassword);
            await _sensitiveDataRepository.UpdateAsync(sensitiveData);
            await _tokenRepository.DeleteAsync(resetToken);

            return true;
        }
    }
}
