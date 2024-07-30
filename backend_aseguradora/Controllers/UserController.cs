using backend_aseguradora.Data;
using backend_aseguradora.Models;
using backend_aseguradora.Services;
using backend_aseguradora.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace backend_aseguradora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserIdFromToken()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : authHeader;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var claims = jwtToken.Claims.Select(c => new { c.Type, c.Value }).ToList();
                claims.ForEach(c => Console.WriteLine($"Claim Type: {c.Type}, Claim Value: {c.Value}"));

                // Ajusta el código para buscar el reclamo "unique_name"
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name"); // Cambiado de ClaimTypes.NameIdentifier a "unique_name"
                if (userIdClaim == null)
                {
                    throw new Exception("User ID claim not found in token.");
                }
                return int.Parse(userIdClaim.Value);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error decoding token: {ex.Message}");
                throw;
            }
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = GetUserIdFromToken();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> EditUserProfile([FromBody] EditUserModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(new { message = "Invalid data", errors });
            }

            try
            {
                var userId = GetUserIdFromToken();
                var result = await _userService.EditUserAsync(userId, model);
                if (result == "User not found.")
                {
                    return NotFound(new { message = result });
                }

                return Ok(new { message = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }


        [HttpPut("profile/password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(new { message = "Invalid data", errors });
            }

            try
            {
                var userId = GetUserIdFromToken();
                var result = await _userService.UpdatePasswordAsync(userId, model);
                if (result == "User not found.")
                {
                    return NotFound(new { message = result });
                }
                if (result != "Password updated successfully.")
                {
                    return BadRequest(new { message = result });
                }

                return Ok(new { message = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }


        [HttpDelete("profile")]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _userService.DeleteUserAsync(userId);
                if (result == "User not found.")
                {
                    return NotFound(new { message = result });
                }

                return Ok(new { message = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
