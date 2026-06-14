using Microsoft.AspNetCore.Mvc;
using Product.API.Auth;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Demo login endpoint. In a real system, validate credentials against a user store.
        /// For this assessment, any non-empty username/password with username == "admin"
        /// and password == "Password123!" is accepted.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username != "admin" || request.Password != "Password123!")
                return Unauthorized(new { message = "Invalid username or password." });

            var accessToken = _tokenService.GenerateAccessToken(request.Username);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return Ok(new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetAccessTokenExpiry()
            });
        }
    }
}