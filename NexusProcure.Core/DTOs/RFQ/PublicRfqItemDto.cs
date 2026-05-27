namespace NexusProcure.Core.DTOs.RFQ;

public class PublicRfqItemDto
{
    public Guid RfqItemId { get; set; }
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }
}
