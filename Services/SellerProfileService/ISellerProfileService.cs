using TokoOnline.Data;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public interface ISellerProfileService
    {
        public Task<Seller?> GetSellerFromAuth(int id);
        public Task<Seller> UpdateSeller(Seller seller, SellerProfile profile);
        public Task<Seller> InitiateSeller(Auth auth);
    }
}