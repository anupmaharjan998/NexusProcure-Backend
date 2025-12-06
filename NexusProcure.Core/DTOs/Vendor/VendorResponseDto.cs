namespace NexusProcure.Core.DTOs.Vendor;

public class VendorResponseDto
{
    public Guid Id { get; set; }
    public string VendorName { get; set; } = null!;
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? TaxId { get; set; }
    public string? Category { get; set; }
    public string Status { get; set; } = null!;
    public List<string>? Documents { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}