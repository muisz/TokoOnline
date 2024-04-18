using TokoOnline.Enums;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public interface IOTPService
    {
        public Task<OTP> CreateOTP(OTPUsage usage, string destination);
        public Task<OTP?> GetOTP(OTPUsage usage, string destination, string code);
        public Task InactivatePreviousOTP(OTPUsage usage, string destination);
        public Task InactivateOTP(OTP otp);
        public bool IsExpired(OTP otp);
    }
}