namespace TokoOnline.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public float Price { get; set; }
        public uint Stock { get; set; }
        public int SellerId { get; set; }
        public Seller Seller { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<ProductVariant> Variants { get; } = new List<ProductVariant>();
        public ICollection<ProductMedia> Media { get; } = new List<ProductMedia>();
    }
}