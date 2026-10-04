using api_clinic.src.Interfaces;
using api_clinic.src.Models.Base;
using Microsoft.AspNetCore.Mvc;
using api_clinic.src.Requests;
using Microsoft.AspNetCore.Authorization;
using api_clinic.src.Requests.User;

namespace api_clinic.src.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {

        [HttpPost("register-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserAdminRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<dynamic?> response = await service.CreateUserAdminAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }
       
        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<dynamic?> response = await service.CreateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            request.Device.Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            request.Device.UserAgent = Request.Headers.UserAgent.ToString();

            ResponseApi<dynamic?> response = await service.LoginAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpPost("new-code")]
        public async Task<IActionResult> NewCode([FromBody] NewCodeRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<dynamic?> response = await service.NewCodeAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<dynamic?> response = await service.ForgotPasswordAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<dynamic?> response = await service.ResetPasswordAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }
        
        [HttpPost("clean-incorrect-password")]
        [Authorize]
        public async Task<IActionResult> CleanIncorrectPassword([FromBody] CleanIncorrectPasswordRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            bool isAdmin = User.FindFirst("admin")?.Value == "True";
            if (!isAdmin) return Forbid();

            ResponseApi<dynamic?> response = await service.CleanIncorrectPasswordAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpGet("theme/{code}")]
        public async Task<IActionResult> GetThemeByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return BadRequest("Código inválido.");

            ResponseApi<dynamic?> response = await service.GetThemeByCodeAsync(code);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpGet("theme/clinic/{clinicId}")]
        public async Task<IActionResult> GetThemeByClinicId(string clinicId)
        {
            if (string.IsNullOrWhiteSpace(clinicId)) return BadRequest("Id inválido.");

            ResponseApi<dynamic?> response = await service.GetThemeByClinicIdAsync(clinicId);
            return StatusCode(response.StatusCode, response.Result);
        }
    }
}