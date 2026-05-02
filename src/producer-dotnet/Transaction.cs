namespace ProducerDotnet;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AssetCode { get; set; } = string.Empty; // Ex: CRI_VERT_001
    public string Issuer { get; set; } = string.Empty;    // Ex: Securitizadora S.A.
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}