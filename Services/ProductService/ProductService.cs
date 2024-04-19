using Microsoft.EntityFrameworkCore;
using TokoOnline.Data;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public class ProductService : IProductService
    {
        private readonly DatabaseContext _context;
        private readonly IFileStorage _fileStorage;

        public ProductService(DatabaseContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<Product> AddProduct(AddProduct item, Seller seller)
        {
            Product product = new Product
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Stock = item.Stock,
                SellerId = seller.Id,
                CreatedAt = DateTime.Now.ToUniversalTime(),
            };
            
            foreach (AddProductVariant variant in item.Variants)
            {
                product.Variants.Add(new ProductVariant
                {
                    Name = variant.Name,
                    Value = variant.Value,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                });
            }

            foreach (AddProductMedia media in item.Media)
            {
                string file = await _fileStorage.UploadFromBase64(media.Media, media.Filename);
                product.Media.Add(new ProductMedia
                {
                    Type = media.Type,
                    Media = file,
                    IsFeatured = media.IsFeatured,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<ICollection<Product>> GetProductsFromSeller(int id)
        {
            return await _context.Products
                .Where(product => product.SellerId == id)
                .ToListAsync();
        }
    }
}