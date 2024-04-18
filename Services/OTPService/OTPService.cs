using System.Security.Permissions;
using Microsoft.EntityFrameworkCore;
using TokoOnline.Enums;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public class OTPService : IOTPService
    {
        private readonly DatabaseContext _context;

        public OTPService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<OTP> CreateOTP(OTPUsage usage, string destination)
        {
            await InactivatePreviousOTP(usage, destination);

            Random random = new Random();
            OTP newOTP = new OTP
            {
                Code = random.Next(1000, 9999).ToString(),
                Destination = destination.ToLower(),
                Usage = usage,
                IsActive = true,
                ValidUntil = DateTime.Now.AddMinutes(3).ToUniversalTime(),
                CreatedAt = DateTime.Now.ToUniversalTime(),
            };
            await _context.OTPs.AddAsync(newOTP);
            await _context.SaveChangesAsync();
            return newOTP;
        }

        public async Task<OTP?> GetOTP(OTPUsage usage, string destination, string code)
        {
            return await _context.OTPs.SingleOrDefaultAsync(otp => 
                otp.Code == code &&
                otp.Usage == usage && 
                otp.Destination == destination && 
                otp.IsActive
            );
        }

        public async Task InactivatePreviousOTP(OTPUsage usage, string destination)
        {
            List<OTP> otps = await _context.OTPs
                .Where(otp => otp.Usage == usage && otp.Destination == destination && otp.IsActive)
                .ToListAsync();
            otps.ForEach(otp => otp.IsActive = false);
            await _context.SaveChangesAsync();
        }

        public async Task InactivateOTP(OTP otp)
        {
            otp.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public bool IsExpired(OTP otp)
        {
            return DateTime.Now > otp.ValidUntil;
        }
    }
}