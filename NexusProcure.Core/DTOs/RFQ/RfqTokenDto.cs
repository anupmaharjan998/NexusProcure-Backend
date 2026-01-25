namespace NexusProcure.Core.DTOs.RFQ;

public class RfqTokenDto
{
    public Guid RfqId { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = default!;

    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}
