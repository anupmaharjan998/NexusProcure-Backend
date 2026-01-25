namespace NexusProcure.Core.DTOs.RFQ;

public class PublicRfqItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }
}
