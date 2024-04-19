using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokoOnline.Data;
using TokoOnline.Exceptions;
using TokoOnline.Models;
using TokoOnline.Services;

namespace TokoOnline.Controllers.V1
{
    [Route("/api/v1/seller/products")]
    [ApiController]
    public class SellerProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ITokenService _tokenService;
        private readonly ISellerProfileService _sellerProfileService;

        public SellerProductController(IProductService productService, ITokenService tokenService, ISellerProfileService sellerProfileService)
        {
            _productService = productService;
            _tokenService = tokenService;
            _sellerProfileService = sellerProfileService;
        }

        [HttpPost()]
        [Authorize(Roles = "1")]
        public async Task<ActionResult> PostAddProduct(AddProduct payload)
        {
            try
            {
                int authId = _tokenService.GetAuthIdFromClaim(User);
                Seller? seller = await _sellerProfileService.GetSellerFromAuth(authId);
                Product product = await _productService.AddProduct(payload, seller!);
                return StatusCode(StatusCodes.Status201Created, new { Id = product.Id });
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }

        [HttpGet()]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<ICollection<ListProduct>>> GetProducts()
        {
            try
            {
                int authId = _tokenService.GetAuthIdFromClaim(User);
                Seller? seller = await _sellerProfileService.GetSellerFromAuth(authId);
                ICollection<Product> products = await _productService.GetProductsFromSeller(seller!.Id);
                ICollection<ListProduct> listProducts = new List<ListProduct>();
                
                foreach(Product product in products)
                {
                    listProducts.Add(new ListProduct
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        CreatedAt = product.CreatedAt,
                    });
                }
                return Ok(listProducts);
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }
    }
}