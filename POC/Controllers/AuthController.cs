using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using POC.Models.Auth;
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
            var loginResult = await _authService.GetAdminByUsernameAsync(request.Username);
            if (loginResult?.PasswordHash == null)
            {
                Console.WriteLine("⚠️ Admin not found.");
                Console.WriteLine($"Received username: {request.Username}");
                return Unauthorized(new { message = "Invalid username or password" });

            }

            bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, loginResult.PasswordHash);

            if (!passwordMatches)
            {
                Console.WriteLine("⚠️ Password does not match.");
                return Unauthorized(new { message = "Invalid username or password" });
            }
            //if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            //    return Unauthorized(new { message = "Invalid username or password" });
            // added comment

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim(ClaimTypes.Name, admin.UserName),
                    new Claim(ClaimTypes.Role, loginResult.RoleName)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }



        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignUpRequest request)
        {
            //if (request.Password != request.ConfirmPassword)
            //    return BadRequest(new { message = "Passwords do not match" });

            if (await _authService.UserExistsAsync(request.Username))
                return BadRequest(new { message = "Username already taken" });

            var signupResult = await _authService.CreateAdminAsync(request);

            // Auto-login after signup (optional)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            //new Claim(ClaimTypes.Name, admin.UserName),
            new Claim("Role", signupResult.RoleName)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        [HttpGet("RoleOptions")]
        public async Task<IActionResult> GetRolesOptionsAsync()
        {
            var result = await _authService.GetRoleOptionsAsync();
            if (result != null)
            {
                return Ok(new { roleOptionList = result });
            }
            else
            {
                return StatusCode(500, "Failed to fetch role options");
            }
        }

        [HttpGet("GetAuthUser")]
        public async Task<IActionResult> GetAuth()
        {
            try
            {
                var AuthUsers = await _authService.GetAuthUser();
                return Ok(AuthUsers);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An Error occured while fetching AuthUser");
            }
        }

    }
}
