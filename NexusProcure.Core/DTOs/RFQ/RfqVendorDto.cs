using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.RFQ;

public class RfqVendorDto
{
    public Guid VendorId { get; set; }
    public string CompanyName { get; set; }
    public string VendorName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Contact { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string PaymentTerms { get; set; }
}
