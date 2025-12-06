using Microsoft.AspNetCore.Http;
using NexusProcure.Core.DTOs.Vendor;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces;

public interface IVendorService
{
    Task<VendorResponseDto> CreateVendorAsync(VendorRequestDto dto);
    Task<VendorResponseDto?> UpdateVendorAsync(Guid id, VendorRequestDto dto);
    Task<bool> DeleteVendorAsync(Guid id);
    Task<VendorResponseDto?> GetVendorByIdAsync(Guid id);
    Task<IEnumerable<VendorResponseDto>> GetAllVendorsAsync(string? status = null, string? search = null);
    Task<bool> UpdateVendorStatusAsync(Guid id, string status);
    Task<VendorDocument> UploadVendorDocumentAsync(Guid vendorId, IFormFile file, Guid uploadedBy);
    Task<bool> DeleteVendorDocumentAsync(Guid documentId);
}