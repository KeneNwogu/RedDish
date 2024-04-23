using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using RedDish.Models;
//using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RedDish.DTOs;


namespace RedDish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RedDishDbContext _context;

        public UsersController(RedDishDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto userDto)
        {
            User? user = _context.Users.FirstOrDefault(
                u => u.Username == userDto.Username || u.Email == userDto.Email);

            if (user != null) return BadRequest(new { message = "User with username or email already exists." });

            User newUser = new User
            {
                Email = userDto.Email,
                Username = userDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            var response = new
            {
                user = newUser
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto userDto)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Username == userDto.Username);
            if (user == null) return BadRequest(new { message = "Invalid username or password" });

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
                return BadRequest(new { message = "Invalid username or password" });

            var response = new
            {
                token = GenerateJSONWebToken(user),
                user
            };

            return Ok(response);
        }


        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ??
                throw new ArgumentNullException("No JWT_KEY was provided."))
                );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Environment.GetEnvironmentVariable("JWT_ISSUER"),
                  new List<Claim> {
                      new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                      new Claim(ClaimTypes.Email, userInfo.Email)
                  },
                  expires: DateTime.Now.AddMinutes(120),
                  signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
