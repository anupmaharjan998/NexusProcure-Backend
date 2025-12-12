namespace NexusProcure.Core.DTOs.Vendor;

public class VendorRequestDto
{
    public string VendorName { get; set; } = null!;
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? TaxId { get; set; }
    public string? Category { get; set; }
    public string? BankAccount { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Status { get; set; }
}