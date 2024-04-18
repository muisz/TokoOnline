using Microsoft.EntityFrameworkCore;

namespace TokoOnline.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Auth> Auths { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductMedia> ProductMedias { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Seller> Sellers { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
