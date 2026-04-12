using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities;

public class Vendor
{
    public Guid Id { get; set; }
    public string VendorName { get; set; } = null!;
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? TaxId { get; set; }
    public TaxType? TaxType { get; set; }
    public string Status { get; set; } = "Pending";
    
    public string? BankName { get; set; }
    public string? BankBranch { get; set; }
    public string? BankAccount { get; set; }
    
    public PaymentTerm PaymentTerms { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    
    public ICollection<VendorCategory> VendorCategories { get; set; } = new List<VendorCategory>();
    public ICollection<VendorDocument>? Documents { get; set; }
}