using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.WebApi.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DataAggregator.WebApi.Authorization
{
    public class JwtUtils : IJwtUtils
    {
        private readonly AppSettings appSettings;

        public JwtUtils(IOptions<AppSettings> appOptions)
        {
            this.appSettings = appOptions.Value;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var subscriptionKey = Encoding.ASCII.GetBytes(this.appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(subscriptionKey), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public int? ValidateJwtToken(string token)
        {
            if (token is null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var subscriptionKey = Encoding.ASCII.GetBytes(this.appSettings.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(subscriptionKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
