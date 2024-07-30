using backend_aseguradora.Models;
using backend_aseguradora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_aseguradora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(new { message = "Invalid data", errors });
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password
            };

            var person = new Person
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth
            };

            var result = await _authService.RegisterAsync(user, person, model.Role);

            if (result != "User registered successfully.")
                return BadRequest(new { message = result });

            var (token, role) = await _authService.LoginAsync(user.Username, model.Password);

            if (token == "Invalid username or password.")
                return BadRequest(new { message = token });

            return Ok(new { token, role });
        }


        [HttpPost("signin")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (token, role) = await _authService.LoginAsync(model.Username, model.Password);

            if (token == "Invalid username or password.")
                return BadRequest(new { message = token });

            return Ok(new { token, role });
        }


        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] string token)
        {
            var newToken = _authService.RefreshToken(token);

            if (newToken == null)
                return BadRequest(new { message = "Invalid token" });

            return Ok(new { token = newToken });
        }


        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok("This is an admin endpoint.");
        }


        [Authorize(Roles = "user")]
        [HttpGet("user")]
        public IActionResult UserEndpoint()
        {
            return Ok("This is a user endpoint.");
        }
    }
}
