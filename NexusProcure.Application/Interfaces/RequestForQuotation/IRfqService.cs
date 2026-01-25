using Microsoft.AspNetCore.Http;
using NexusProcure.Core.DTOs.RFQ;
using NexusProcure.Core.Entities.RequestForQuotations;

namespace NexusProcure.Application.Interfaces.RequestForQuotation;

public interface IRfqService
{
    Task<Core.Entities.RequestForQuotations.RequestForQuotation> CreateRfqAsync(Guid requisitionId);
    Task<RfqAccessToken> GenerateAccessTokenAsync(Guid rfqId, Guid vendorId);
    Task<PublicRfqDto?> GetRfqByTokenAsync(string token);
    Task<RfqTokenDto> ValidateRfqTokenAsync(string token);
    Task SubmitQuotationAsync(string token, QuotationSubmitDto dto);
    
    Task SubmitQuotationFromExcelAsync(string token, IFormFile file);

}
