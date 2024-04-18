using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TokoOnline.Data;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public class JWTService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string accessKey;
        private readonly string refreshKey;
        private readonly string issuer;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;

            accessKey = _configuration["JWT:AccessKey"] ?? "";
            refreshKey = _configuration["JWT:RefreshKey"] ?? "";
            issuer = _configuration["JWT:Issuer"] ?? "";
        }

        public Token CreatePairToken(Auth auth)
        {
            Token token = new Token
            {
                Access = CreateAccessToken(auth),
                Refresh = CreateRefreshToken(auth),
            };
            return token;
        }

        public string CreateAccessToken(Auth auth)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, auth.Email),
                new Claim(ClaimTypes.NameIdentifier, auth.Id.ToString()),
                new Claim(ClaimTypes.Role, auth.Role.ToString("D")),
            };
            return CreateToken(accessKey, claims, 3);
        }

        public string CreateRefreshToken(Auth auth)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, auth.Email),
                new Claim(ClaimTypes.NameIdentifier, auth.Id.ToString()),
                new Claim(ClaimTypes.Role, auth.Role.ToString("D")),
            };
            return CreateToken(refreshKey, claims, 12);
        }

        public Token Refresh(string token)
        {
            List<Claim> claims = GetClaimsFromRefresh(token);
            Token newToken = new Token
            {
                Access = CreateToken(accessKey, claims, 3),
                Refresh = CreateToken(refreshKey, claims, 12),
            };
            return newToken;
        }

        public List<Claim> GetClaimsFromRefresh(string token)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshKey));
            TokenValidationParameters parameters = new TokenValidationParameters
            {
                ValidIssuer = issuer,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = handler.ValidateToken(token, parameters, out SecurityToken validatedToken);
            return principal.Claims.ToList();
        }

        public string CreateToken(string key, List<Claim> claims, int activeHours)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            SigningCredentials credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(activeHours),
                signingCredentials: credential,
                issuer: issuer
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}