namespace TokoOnline.Models
{
    public class Seller
    {
        public int Id { get; set; }
        public int AuthId { get; set; }
        public Auth Auth { get; set; } = null!;
        public string MerchantName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Product> Products { get; } = new List<Product>();
    }
}