using TokoOnline.Data;
using TokoOnline.Enums;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public interface IAuthService
    {
        public Task<Auth> Register(AuthRegister auth, Role role);
        public Task<Auth> Authenticate(string email, string password);
        public Task<Auth?> GetFromEmail(string value);
        public Task<Auth?> GetFromPhoneNumber(string value);
        public string FormatPhoneNumber(string value);
        public Task Verify(Auth auth);
    }
}