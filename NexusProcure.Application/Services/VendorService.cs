using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Vendor;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;
using Supabase.Storage;
using Client = Supabase.Client;
using FileOptions = Supabase.Storage.FileOptions;

namespace NexusProcure.Application.Services;

public class VendorService : IVendorService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;
    private readonly Cloudinary _cloudinary; // injected if using Cloudinary
    private readonly Client _supabase;

    public VendorService(NexusProcureDbContext context, IMapper mapper, Cloudinary cloudinary, Client supabase)
    {
        _context = context;
        _mapper = mapper;
        _cloudinary = cloudinary;
        _supabase = supabase;
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
        var vendor = await _context.Vendors
        .Include(c => c.Category)
            .Include(v => v.Documents)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vendor == null) return null;

        return _mapper.Map<VendorResponseDto>(vendor);
    }


    public async Task<IEnumerable<VendorResponseDto>> GetAllVendorsAsync(string? status = null, string? search = null)
    {
        var q = _context.Vendors.Include(v => v.Documents)
            .Include(c => c.Category).AsQueryable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(v => v.Status == status);
        if (!string.IsNullOrEmpty(search))
            q = q.Where(v => v.VendorName.Contains(search) || v.CompanyName.Contains(search));
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

    public async Task<VendorDocument> UploadVendorDocumentAsync(
        Guid vendorId,
        IFormFile file,
        Guid uploadedBy)
    {
        var vendor = await _context.Vendors.FindAsync(vendorId);
        if (vendor == null)
            throw new Exception("Vendor not found");

        var FileUrl = $"{vendorId}/{Guid.NewGuid()}_{file.FileName}";

        byte[] fileBytes;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            fileBytes = ms.ToArray();
        }

        var response = await _supabase.Storage
            .From("vendor-documents")
            .Upload(
                fileBytes,
                FileUrl,
                new FileOptions
                {
                    ContentType = file.ContentType,
                    Upsert = false
                });

        if (response == null)
            throw new Exception("File upload failed");

        var doc = new VendorDocument
        {
            VendorId = vendorId,
            FileUrl = FileUrl,
            FileName = file.FileName,
            FileType = file.ContentType,
            UploadedBy = uploadedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.VendorDocuments.Add(doc);
        await _context.SaveChangesAsync();
        return doc;
    }

    public async Task<bool> DeleteVendorDocumentAsync(Guid documentId)
    {
        var doc = await _context.VendorDocuments.FindAsync(documentId);
        if (doc == null) return false;

        await _supabase.Storage
            .From("vendor-documents")
            .Remove(new List<string> { doc.FileUrl });

        _context.VendorDocuments.Remove(doc);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<(byte[] Data, string ContentType, string FileName)> DownloadVendorDocumentAsync(Guid documentId)
    {
        var doc = await _context.VendorDocuments.FindAsync(documentId);
        if (doc == null)
            throw new Exception("Document not found");

        var data = await _supabase.Storage
            .From("vendor-documents")
            .Download(doc.FileUrl, (TransformOptions?)null);

        return (data, doc.FileType, doc.FileName);
    }

}
