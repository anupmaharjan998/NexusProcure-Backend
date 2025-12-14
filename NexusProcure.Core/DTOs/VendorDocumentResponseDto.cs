namespace NexusProcure.Core.DTOs;

public class VendorDocumentResponseDto
{
    public Guid Id { get; set; }

    public string FileUrl { get; set; } = null!;
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    
    public DateTime CreatedAt { get; set; }
}