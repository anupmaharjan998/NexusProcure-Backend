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
    Task SubmitQuotationAsync(string token, QuotationSubmitDto dto, string? ipAddress);
    
    Task SubmitQuotationFromExcelAsync(string token, IFormFile file);
    
    
    Task<List<RfqDto>> GetAllRfqAsync();
    Task<QuotationListResponseDto> GetQuotationByRfqIdAsync(Guid rfqId);
    Task<QuotationDetailsDto> GetQuotationByIdAsync(Guid quotationId);

    Task<QuotationComparisonResponseDto> CompareQuotationsAsync(List<Guid> quotationIds);
    Task SelectQuotationAsync(Guid rfqId, Guid quotationId);
    Task ClearSelectedQuotationAsync(Guid rfqId);

    Task SubmitSelectedQuotationForApproval(Guid rfqId);

}
