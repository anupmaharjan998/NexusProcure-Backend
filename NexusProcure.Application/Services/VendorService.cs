using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Vendor;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class VendorService : IVendorService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;
    private readonly Cloudinary _cloudinary; // injected if using Cloudinary

    public VendorService(NexusProcureDbContext context, IMapper mapper, Cloudinary cloudinary)
    {
        _context = context;
        _mapper = mapper;
        _cloudinary = cloudinary;
    }

    public async Task<VendorResponseDto> CreateVendorAsync(VendorRequestDto dto)
    {
        var vendor = _mapper.Map<Vendor>(dto);
        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync();
        return _mapper.Map<VendorResponseDto>(vendor);
    }

    public async Task<VendorResponseDto?> UpdateVendorAsync(Guid id, VendorRequestDto dto)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor == null) return null;
        _mapper.Map(dto, vendor);
        vendor.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return _mapper.Map<VendorResponseDto>(vendor);
    }

    public async Task<bool> DeleteVendorAsync(Guid id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor == null) return false;
        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<VendorResponseDto?> GetVendorByIdAsync(Guid id)
    {
        var vendor = await _context.Vendors.Include(v => v.Documents).FirstOrDefaultAsync(v => v.Id == id);
        if (vendor == null) return null;
        var dto = _mapper.Map<VendorResponseDto>(vendor);
        dto.Documents = vendor.Documents?.Select(d => d.FileUrl).ToList();
        return dto;
    }

    public async Task<IEnumerable<VendorResponseDto>> GetAllVendorsAsync(string? status = null, string? search = null)
    {
        var q = _context.Vendors.Include(v => v.Documents).AsQueryable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(v => v.Status == status);
        if (!string.IsNullOrEmpty(search)) q = q.Where(v => v.VendorName.Contains(search) || v.CompanyName.Contains(search));
        return await q.OrderByDescending(v => v.CreatedAt)
                     .Select(v => _mapper.Map<VendorResponseDto>(v))
                     .ToListAsync();
    }

    public async Task<bool> UpdateVendorStatusAsync(Guid id, string status)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor == null) return false;
        vendor.Status = status;
        vendor.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<VendorDocument> UploadVendorDocumentAsync(Guid vendorId, IFormFile file, Guid uploadedBy)
    {
        var vendor = await _context.Vendors.FindAsync(vendorId);
        if (vendor == null) throw new Exception("Vendor not found");

        // Upload to Cloudinary
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "nexusprocure/vendors"
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception("Upload failed");

        var doc = new VendorDocument
        {
            VendorId = vendor.Id,
            FileUrl = uploadResult.SecureUrl.ToString(),
            FileName = file.FileName,
            FileType = file.ContentType,
            PublicId = uploadResult.PublicId,
            UploadedBy = uploadedBy
        };

        _context.VendorDocuments.Add(doc);
        await _context.SaveChangesAsync();
        return doc;
    }

    public async Task<bool> DeleteVendorDocumentAsync(Guid documentId)
    {
        var doc = await _context.VendorDocuments.FindAsync(documentId);
        if (doc == null) return false;

        if (!string.IsNullOrEmpty(doc.PublicId))
        {
            var deletionParams = new DeletionParams(doc.PublicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }

        _context.VendorDocuments.Remove(doc);
        await _context.SaveChangesAsync();
        return true;
    }
}
