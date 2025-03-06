using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfig _tokenConfig;
        public TokenService(IOptions<TokenConfig> options)
        {
            _tokenConfig = options.Value;
        }

        public string GenerateToken(string username,List<string> roles)
        {
            var claims = new List<Claim>
         {
             new Claim(JwtRegisteredClaimNames.Sub, username),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(ClaimTypes.NameIdentifier, username)
            };

            roles.ForEach(role =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_tokenConfig.JwtExpireMinutes));

            var token = new JwtSecurityToken(
                _tokenConfig.JwtIssuer,
                _tokenConfig.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    public interface ITokenService
    {
        string GenerateToken(string username,List<string> roles);
    }
    public class TokenConfig
    {
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtExpireMinutes { get; set; }
    }
}
