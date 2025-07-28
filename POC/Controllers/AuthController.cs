using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using POC.Models;
using POC.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace POC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(AuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var admin = await _authService.GetAdminByUsernameAsync(request.Username);
            if (admin == null)
            {
                Console.WriteLine("⚠️ Admin not found.");
                Console.WriteLine($"Received username: {request.Username}");
                return Unauthorized(new { message = "Invalid username or password" });

            }

            var isMatch = BCrypt.Net.BCrypt.Verify("Choss@17", "$2a$11$Oe1DHjIxY2WmuVSw2a5u4evTMjYbeRxq8cHVPw06g8Z3nuW3bSfVq");
            Console.WriteLine($"✅ Match: {isMatch}");

            bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash);

            if (!passwordMatches)
            {
                Console.WriteLine("⚠️ Password does not match.");
                return Unauthorized(new { message = "Invalid username or password" });
            }
            //if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            //    return Unauthorized(new { message = "Invalid username or password" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, admin.UserName),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}
