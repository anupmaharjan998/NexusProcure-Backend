using Microsoft.AspNetCore.Http;

namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationExcelUploadDto
{
    public IFormFile File { get; set; } = null!;
}
