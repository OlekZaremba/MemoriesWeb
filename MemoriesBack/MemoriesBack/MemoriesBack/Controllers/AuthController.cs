using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MemoriesBack.DTO;
using MemoriesBack.Service;

namespace MemoriesBack.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly PasswordResetService _passwordResetService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthService authService,
            PasswordResetService passwordResetService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _passwordResetService = passwordResetService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
        {
            _logger.LogDebug(">>> POST /api/auth/register payload = {@Request}", request);

            try
            {
                await _authService.RegisterUser(request);
                _logger.LogDebug("<<< POST /api/auth/register OK");
                return Ok("Użytkownik został utworzony");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "!!! Błąd w registerUser");
                return StatusCode(500, $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, string> credentials)
        {
            try
            {
                var login = credentials.GetValueOrDefault("login");
                var password = credentials.GetValueOrDefault("password");

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    return BadRequest("Brak loginu lub hasła");
                }

                var result = await _authService.Login(login, password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "!!! Błąd w login");
                return Unauthorized($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] Dictionary<string, string> body)
        {
            var login = body.GetValueOrDefault("login");
            if (string.IsNullOrWhiteSpace(login))
            {
                return BadRequest("Brak loginu (e-maila)");
            }

            await _passwordResetService.RequestPasswordReset(login);
            return Ok("Jeśli konto istnieje, link resetujący został wysłany.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] Dictionary<string, string> body)
        {
            var token = body.GetValueOrDefault("token");
            var newPassword = body.GetValueOrDefault("newPassword");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Brak tokenu lub hasła");
            }

            var success = await _passwordResetService.ResetPassword(token, newPassword);
            if (success)
            {
                return Ok("Hasło zostało zaktualizowane");
            }
            return BadRequest("Nieprawidłowy lub wygasły token");
        }
    }
}
