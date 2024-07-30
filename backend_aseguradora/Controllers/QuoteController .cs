using backend_aseguradora.DTOs;
using backend_aseguradora.Models;
using backend_aseguradora.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace backend_aseguradora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _quoteService;
        private readonly IInsuranceTypeService _insuranceTypeService;
        private readonly ICoverageService _coverageService;
        private readonly IUserService _userService;

        public QuoteController(IQuoteService quoteService, IInsuranceTypeService insuranceTypeService, ICoverageService coverageService, IUserService userService)
        {
            _quoteService = quoteService;
            _insuranceTypeService = insuranceTypeService;
            _coverageService = coverageService;
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
                Console.WriteLine($"Error decoding token: {ex.Message}");
                throw;
            }
        }


        [HttpPost("create-quote")]
        public async Task<IActionResult> CreateQuote([FromBody] QuoteModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromToken();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Obtener detalles de InsuranceType y Coverage
            var insuranceType = await _insuranceTypeService.GetInsuranceTypeByIdAsync(model.InsuranceTypeId);
            if (insuranceType == null)
            {
                return NotFound(new { message = "Insurance Type not found." });
            }

            var coverage = await _coverageService.GetCoverageByIdAsync(model.CoverageId);
            if (coverage == null)
            {
                return NotFound(new { message = "Coverage not found." });
            }

            // Generar un ID provisional para la cotización
            var nextQuoteId = await _quoteService.GetNextProvisionalQuoteIdAsync();

            // Calcula el precio de la cotización
            var carAge = DateTime.UtcNow.Year - model.Year;

            // Se asegura de que la edad del coche no sea cero para evitar la división por cero
            if (carAge <= 0)
            {
                carAge = 1;
            }

            var quotePrice = Math.Round((model.Cost / carAge) * 0.0035m, 2); // Use 'm' for decimal literals

            // Crea una respuesta con todos los datos necesarios
            var quoteResponse = new
            {
                QuoteId = nextQuoteId,
                UserId = userId,
                User = user,
                Car = model,
                InsuranceType = new { insuranceType.Id, insuranceType.Name },
                Coverage = new { coverage.Id, coverage.Name },
                Price = quotePrice.ToString("C2"),
                Date = DateTime.UtcNow.ToString("dd/MM/yyyy - HH:mm")
            };

            return Ok(quoteResponse);
        }


        [HttpPost("save-quote")]
        public async Task<IActionResult> SaveQuote([FromBody] QuoteModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserIdFromToken();
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var insuranceType = await _insuranceTypeService.GetInsuranceTypeByIdAsync(model.InsuranceTypeId);
                if (insuranceType == null)
                {
                    return NotFound(new { message = "Insurance Type not found." });
                }

                var coverage = await _coverageService.GetCoverageByIdAsync(model.CoverageId);
                if (coverage == null)
                {
                    return NotFound(new { message = "Coverage not found." });
                }

                var carAge = DateTime.UtcNow.Year - model.Year;
                if (carAge <= 0)
                {
                    carAge = 1;
                }

                var quotePrice = Math.Round((model.Cost / carAge) * 0.0035m, 2); // Use 'm' for decimal literals

                var quote = new Quote
                {
                    Car = new Car
                    {
                        Brand = model.Brand,
                        Model = model.Model,
                        Year = model.Year,
                        Cost = Math.Round((decimal)model.Cost, 2)
                    },
                    UserId = userId,
                    InsuranceTypeId = model.InsuranceTypeId,
                    CoverageId = model.CoverageId,
                    Price = quotePrice,
                    CreatedAt = DateTime.UtcNow
                };

                var savedQuote = await _quoteService.SaveQuoteAsync(quote, userId);

                // Crear un DTO para la respuesta
                var quoteDto = new QuoteDto
                {
                    Id = savedQuote.Id,
                    Car = new CarDto
                    {
                        Brand = savedQuote.Car.Brand,
                        Model = savedQuote.Car.Model,
                        Year = savedQuote.Car.Year,
                        Cost = Math.Round(savedQuote.Car.Cost, 2)
                    },
                    InsuranceTypeId = savedQuote.InsuranceTypeId,
                    CoverageId = savedQuote.CoverageId,
                    Price = Math.Round(savedQuote.Price, 2),
                    CreatedAt = savedQuote.CreatedAt
                };


                return Ok(quoteDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving quote: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while saving the quote." });
            }
        }


        [HttpGet("insurance-types")]
        public async Task<IActionResult> GetInsuranceTypes()
        {
            var insuranceTypes = await _insuranceTypeService.GetInsuranceTypesAsync();
            return Ok(insuranceTypes);
        }


        [HttpGet("coverages")]
        public async Task<IActionResult> GetCoverages()
        {
            var coverages = await _coverageService.GetCoveragesAsync();
            return Ok(coverages);
        }


        [HttpGet("user-quotes")]
        public async Task<IActionResult> GetUserQuotes()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var quotes = await _quoteService.GetQuotesByUserIdAsync(userId);

                var quoteDetails = quotes.Select(q => new
                {
                    q.Id,
                    q.Car.Brand,
                    q.Car.Model,
                    q.Car.Year,
                    Cost = q.Car.Cost.ToString("C2"),
                    InsuranceType = q.InsuranceType.Name,
                    Coverage = q.Coverage.Name,
                    Price = q.Price.ToString("C2"),
                    createdAt = q.CreatedAt.ToString("yyyy-MM-dd")
                });

                return Ok(quoteDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user quotes: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching quotes." });
            }
        }


        [HttpGet("all-quotes")]
        public async Task<IActionResult> GetAllQuotes()
        {
            try
            {
                var quotes = await _quoteService.GetAllQuotesAsync();
                var quoteDetails = quotes.Select(q => new
                {
                    q.Id,
                    q.Car.Brand,
                    q.Car.Model,
                    q.Car.Year,
                    Cost = q.Car.Cost.ToString("C2"),
                    InsuranceType = q.InsuranceType.Name,
                    Coverage = q.Coverage.Name,
                    Price = q.Price.ToString("C2"),
                    q.CreatedAt,
                    User = new
                    {
                        q.User.Person.FirstName,
                        q.User.Person.LastName,
                        q.User.Username
                    }
                });

                return Ok(quoteDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all quotes: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching all quotes." });
            }
        }


        [HttpDelete("delete-quote/{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var userId = GetUserIdFromToken();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var success = await _quoteService.DeleteQuoteAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Quote not found." });
            }

            return Ok(new { message = "Quote deleted successfully." });
        }
    }
}
