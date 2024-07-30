using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using backend_aseguradora.Data;
using backend_aseguradora.Models;
using backend_aseguradora.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace backend_aseguradora.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(User user, Person person, string role)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
                return "Username or Email already exists.";

            if (!PasswordValidator.IsValid(user.Password))
                return "Password must contain at least one letter, one number, and one uppercase letter.";

            var (salt, hashedPassword) = HashFunction.CreateHashAndSalt(user.Password);
            user.Password = hashedPassword;
            user.Salt = salt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            person.UserId = user.Id;
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            // Asigna el rol "user" por defecto si no se especifica
            role = string.IsNullOrWhiteSpace(role) ? "user" : role;

            var roleEntity = await _context.Roles.SingleOrDefaultAsync(r => r.Name == role);
            if (roleEntity == null)
                return "Invalid role specified.";

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = roleEntity.Id
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            return "User registered successfully.";
        }


        public async Task<(string token, string role)> LoginAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null || !HashFunction.CheckPassword(password, user.Salt, user.Password))
                return ("Invalid username or password.", null);

            var role = await (from ur in _context.UserRoles
                              join r in _context.Roles on ur.RoleId equals r.Id
                              where ur.UserId == user.Id
                              select r.Name).FirstOrDefaultAsync();

            var token = JWTAuthentication.GenerateJwtToken(user.Id.ToString(), user.Username, role, _configuration["JwtSettings:Secret"]);
            return (token, role);
        }


        public string RefreshToken(string token)
        {
            var principal = JWTAuthentication.GetPrincipalFromExpiredToken(token, _configuration["JwtSettings:Secret"]);
            if (principal == null)
                return null;

            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return JWTAuthentication.GenerateJwtToken(userId, username, role, _configuration["JwtSettings:Secret"]);
        }
    }
}
