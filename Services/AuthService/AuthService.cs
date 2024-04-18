using Microsoft.EntityFrameworkCore;
using TokoOnline.Data;
using TokoOnline.Enums;
using TokoOnline.Exceptions;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public class AuthService : IAuthService
    {
        private readonly DatabaseContext _context;
        private readonly IPasswordHasher _hasher;

        public AuthService(DatabaseContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public async Task<Auth> Register(AuthRegister auth, Role role)
        {
            Auth? authWithSameEmail = await GetFromEmail(auth.Email);
            if (authWithSameEmail != null)
                throw new HttpException("Email already taken");
            
            Auth? authWithSamePhoneNumber = await GetFromPhoneNumber(auth.PhoneNumber);
            if (authWithSamePhoneNumber != null)
                throw new HttpException("Phone number already taken");
            
            Auth newAuth = new Auth
            {
                Name = auth.Name,
                Email = auth.Email.ToLower(),
                PhoneNumber = FormatPhoneNumber(auth.PhoneNumber),
                Password = _hasher.Hash(auth.Password),
                IsActive = false,
                Role = role,
                CreatedAt = DateTime.Now.ToUniversalTime(),
            };
            await _context.Auths.AddAsync(newAuth);
            await _context.SaveChangesAsync();

            return newAuth;
        }

        public async Task<Auth> Authenticate(string email, string password)
        {
            Auth? auth = await GetFromEmail(email);
            if (auth == null)
                throw new HttpException("Email not found", StatusCodes.Status404NotFound);
            
            if (!_hasher.Check(password, auth.Password))
                throw new HttpException("Wrong password");
            
            return auth;
        }

        public async Task<Auth?> GetFromEmail(string value)
        {
            return await _context.Auths.SingleOrDefaultAsync(auth => auth.Email == value.ToLower());
        }

        public async Task<Auth?> GetFromPhoneNumber(string value)
        {
            string phoneNumber = FormatPhoneNumber(value);
            return await _context.Auths.SingleOrDefaultAsync(auth => auth.PhoneNumber == phoneNumber);
        }

        public string FormatPhoneNumber(string value)
        {
            string countryCode = "62";
            string phoneNumber = value.Replace(" ", "").Replace("-", "").Replace("+", "").Replace("(", "").Replace(")", "");
            if (phoneNumber.StartsWith(countryCode))
                return phoneNumber;
            else if (phoneNumber.StartsWith("0"))
                phoneNumber = phoneNumber.Substring(1, phoneNumber.Length - 1);
            return $"{countryCode}{phoneNumber}";
        }

        public async Task Verify(Auth auth)
        {
            auth.IsActive = true;
            await _context.SaveChangesAsync();
        }
    }
}