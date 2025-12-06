namespace NexusProcure.Core.Entities;

public class VendorDocument : BaseEntity
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public string FileUrl { get; set; } = null!;
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public string? PublicId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid? UploadedBy { get; set; }
}