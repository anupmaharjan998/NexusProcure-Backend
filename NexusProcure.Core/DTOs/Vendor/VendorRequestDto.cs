using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.Vendor;

public class VendorRequestDto
{
    public string VendorName { get; set; } = null!;
    public string CompanyName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public TaxType? TaxType { get; set; }
    public string? TaxId { get; set; }
    public Guid CategoryId { get; set; }
    public string? BankName { get; set; }
    public string? BankBranch { get; set; }
    public string? BankAccount { get; set; }
    public PaymentTerm PaymentTerms { get; set; }
    public string? Status { get; set; }
}