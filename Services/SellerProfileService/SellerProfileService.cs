using Microsoft.EntityFrameworkCore;
using TokoOnline.Data;
using TokoOnline.Models;

namespace TokoOnline.Services
{
    public class SellerProfileService : ISellerProfileService
    {
        private readonly DatabaseContext _context;

        public SellerProfileService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Seller?> GetSellerFromAuth(int id)
        {
            return await _context.Sellers.SingleOrDefaultAsync(seller => seller.AuthId == id);
        }

        public async Task<Seller> UpdateSeller(Seller seller, SellerProfile profile)
        {
            if (profile.MerchantName != null & profile.MerchantName != seller.MerchantName)
                seller.MerchantName = profile.MerchantName!;
            
            if (profile.Description != null & profile.Description != seller.Description)
                seller.Description = profile.Description;
            
            await _context.SaveChangesAsync();
            return seller;
        }

        public async Task<Seller> InitiateSeller(Auth auth)
        {
            Seller seller = new Seller
            {
                AuthId = auth.Id,
                Auth = auth,
            };
            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();
            return seller;
        }
    }
}