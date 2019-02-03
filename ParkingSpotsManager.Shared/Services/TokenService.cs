using Microsoft.IdentityModel.Tokens;
using ParkingSpotsManager.Shared.Constants;
using ParkingSpotsManager.Shared.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ParkingSpotsManager.Shared.Services
{
    public static class TokenService
    {
        public static string Get(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetKey();
            var tokenDescriptor = new SecurityTokenDescriptor() {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static byte[] GetKey()
        {
            return Encoding.ASCII.GetBytes(APIConstants.TokenSecretKey);
        }
    }
}
