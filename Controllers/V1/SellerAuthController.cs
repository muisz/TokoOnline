using Microsoft.AspNetCore.Mvc;
using TokoOnline.Data;
using TokoOnline.Enums;
using TokoOnline.Exceptions;
using TokoOnline.Models;
using TokoOnline.Services;

namespace TokoOnline.Controllers.V1
{
    [Route("/api/v1/seller/auth")]
    [ApiController]
    public class SellerAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOTPService _otpService;
        private readonly ITokenService _tokenService;

        public SellerAuthController(IAuthService authService, IOTPService otpService, ITokenService tokenService)
        {
            _authService = authService;
            _otpService = otpService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> PostRegister(AuthRegister payload)
        {
            try
            {
                Auth auth = await _authService.Register(payload, Role.Seller);
                OTP otp = await _otpService.CreateOTP(OTPUsage.AccountVerification, auth.Email);
                return StatusCode(StatusCodes.Status201Created, new { Id = auth.Id });
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> PostLogin(AuthLogin payload)
        {
            try
            {
                Auth auth = await _authService.Authenticate(payload.Email, payload.Password);
                if (auth.Role != Role.Seller)
                    throw new HttpException("account not found", StatusCodes.Status404NotFound);

                if (!auth.IsActive)
                    throw new HttpException("Inactive account. Please verify your account");
                
                AuthResponse response = new AuthResponse
                {
                    Id = auth.Id,
                    Name = auth.Name,
                    Email = auth.Email,
                    Token = _tokenService.CreatePairToken(auth),
                };
                return Ok(response);
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }

        [HttpPost("verification/send")]
        public async Task<ActionResult> PostSendVerification(SendOTPVerification payload)
        {
            try
            {
                // todo check if identifier is email or phone number
                Auth? auth = await _authService.GetFromEmail(payload.Identifier);
                if (auth == null)
                    throw new HttpException("account not found", StatusCodes.Status404NotFound);
                
                OTP otp = await _otpService.CreateOTP(OTPUsage.AccountVerification, payload.Identifier);
                return Ok();
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }

        [HttpPost("verification/verify")]
        public async Task<ActionResult<Token>> PostVerifyVerification(OTPVerification payload)
        {
            try
            {
                OTP? otp = await _otpService.GetOTP(OTPUsage.AccountVerification, payload.Identifier, payload.Code);
                if (otp == null)
                    throw new HttpException("OTP not found", StatusCodes.Status404NotFound);
                
                // todo check if identifier is email or phone number
                Auth? auth = await _authService.GetFromEmail(payload.Identifier);
                if (auth == null)
                    throw new HttpException("Account not found", StatusCodes.Status404NotFound);
                
                await _authService.Verify(auth);
                await _otpService.InactivateOTP(otp);
                
                Token token = _tokenService.CreatePairToken(auth);
                return Ok(token);
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }

        [HttpPost("token/refresh")]
        public ActionResult<Token> PostRefreshToken(RefreshToken payload)
        {
            try
            {
                Token newToken = _tokenService.Refresh(payload.Token);
                return Ok(newToken);
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
            catch (Exception)
            {
                return Problem("Invalid token", statusCode: StatusCodes.Status400BadRequest);
            }
        }
    }
}