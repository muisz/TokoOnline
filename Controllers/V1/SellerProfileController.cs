using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokoOnline.Data;
using TokoOnline.Enums;
using TokoOnline.Exceptions;
using TokoOnline.Models;
using TokoOnline.Services;

namespace TokoOnline.Controllers
{
    [Route("/api/v1/seller/profile")]
    [ApiController]
    public class SellerProfileController : ControllerBase
    {
        private readonly ISellerProfileService _sellerProfileService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;

        public SellerProfileController(
            ISellerProfileService sellerProfileService,
            ITokenService tokenService,
            IAuthService authService
        )
        {
            _sellerProfileService = sellerProfileService;
            _tokenService = tokenService;
            _authService = authService;
        }

        [HttpGet()]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<SellerProfile>> GetProfile()
        {
            try
            {
                int authId = _tokenService.GetAuthIdFromClaim(User);
                Seller? seller = await _sellerProfileService.GetSellerFromAuth(authId);
                
                SellerProfile profile = new SellerProfile();
                if (seller != null)
                {
                    profile.MerchantName = seller.MerchantName;
                    profile.Description = seller.Description;
                }
                return Ok(new SellerProfile
                { 
                    MerchantName = seller?.MerchantName,
                    Description = seller?.Description
                });
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }

        [HttpPatch()]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<SellerProfile>> PatchProfile(SellerProfile payload)
        {
            try
            {
                int authId = _tokenService.GetAuthIdFromClaim(User);
                Seller? seller = await _sellerProfileService.GetSellerFromAuth(authId);

                if (seller == null)
                {
                    Auth? auth = await _authService.GetFromId(authId);
                    seller = await _sellerProfileService.InitiateSeller(auth!);
                }
                
                seller = await _sellerProfileService.UpdateSeller(seller, payload);
                return Ok(new SellerProfile 
                { 
                    MerchantName = seller.MerchantName, 
                    Description = seller.Description 
                });
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }
    }
}