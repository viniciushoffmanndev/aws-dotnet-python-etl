using System;

namespace ProducerDotNet
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string AssetCode { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}