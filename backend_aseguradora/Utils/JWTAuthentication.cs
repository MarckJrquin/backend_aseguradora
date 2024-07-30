using System;
using System.Text;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using backend_aseguradora.Models;

namespace backend_aseguradora.Utils
{
    public static class JWTAuthentication
    {
        public static string GenerateJwtToken(string userId, string username, string role, string secretKey)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(6),
                IssuedAt = DateTime.UtcNow,
                Issuer = "backend_aseguradora",
                Audience = "public",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    SecurityAlgorithms.HmacSha256Signature),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "backend_aseguradora",
                    ValidAudience = "public",
                    ValidateLifetime = false // Aquí es donde se permite validar tokens expirados
                }, out SecurityToken securityToken);

                var jwtToken = securityToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }


        public static string ValidateJwtToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "backend_aseguradora",
                    ValidAudience = "public",
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userid = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

                return userid;
            }
            catch
            {
                return null;
            }
        }
    }
}
