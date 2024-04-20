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
        private readonly IFileStorage _fileStorage;

        public SellerProductController(
            IProductService productService,
            ITokenService tokenService,
            ISellerProfileService sellerProfileService,
            IFileStorage fileStorage
        )
        {
            _productService = productService;
            _tokenService = tokenService;
            _sellerProfileService = sellerProfileService;
            _fileStorage = fileStorage;
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

        [HttpGet("{id}")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<DetailSellerProduct>> GetProduct(int id)
        {
            try
            {
                int authId = _tokenService.GetAuthIdFromClaim(User);
                Seller? seller = await _sellerProfileService.GetSellerFromAuth(authId);
                Product? product = await _productService.GetProduct(id);
                if (product == null || product.SellerId != seller!.Id)
                    throw new HttpException("Not found", StatusCodes.Status404NotFound);
                
                DetailSellerProduct detailProduct = new DetailSellerProduct
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CreatedAt = product.CreatedAt,
                };
                foreach (ProductVariant variant in product.Variants)
                {
                    detailProduct.Variants.Add(new DetailProductVariant
                    {
                        Id = variant.Id,
                        Name = variant.Name,
                        Value = variant.Value,
                    });
                }
                foreach (ProductMedia media in product.Media)
                {
                    detailProduct.Media.Add(new DetailProductMedia
                    {
                        Id = media.Id,
                        Type = media.Type,
                        Media = _fileStorage.GetAccessibleUrl(media.Media).Result,
                        IsFeatured = media.IsFeatured,
                        CreatedAt = media.CreatedAt,
                    });
                }

                return Ok(detailProduct);
            }
            catch (HttpException error)
            {
                return Problem(error.Message, statusCode: error.StatusCode);
            }
        }
    }
}