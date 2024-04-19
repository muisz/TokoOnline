using TokoOnline.Data;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public interface IProductService
    {
        public Task<Product> AddProduct(AddProduct item, Seller seller);
        public Task<ICollection<Product>> GetProductsFromSeller(int id);
    }
}