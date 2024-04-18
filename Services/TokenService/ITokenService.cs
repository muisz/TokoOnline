using System.Security.Claims;
using TokoOnline.Data;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public interface ITokenService
    {
        public Token CreatePairToken(Auth auth);
        public string CreateAccessToken(Auth auth);
        public string CreateRefreshToken(Auth auth);
        public Token Refresh(string token);
        public List<Claim> GetClaimsFromRefresh(string token);
        public int GetAuthIdFromClaim(ClaimsPrincipal principal);
    }
}