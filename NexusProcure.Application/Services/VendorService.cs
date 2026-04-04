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

    // public async Task<VendorResponseDto> CreateVendorAsync(VendorRequestDto dto)
    // {
    //     var vendor = _mapper.Map<Vendor>(dto);
    //     _context.Vendors.Add(vendor);
    //     await _context.SaveChangesAsync();
    //     return _mapper.Map<VendorResponseDto>(vendor);
    // }
    //
    // public async Task<VendorResponseDto?> UpdateVendorAsync(Guid id, VendorRequestDto dto)
    // {
    //     var vendor = await _context.Vendors.FindAsync(id);
    //     if (vendor == null) return null;
    //     _mapper.Map(dto, vendor);
    //     vendor.UpdatedAt = DateTime.UtcNow;
    //     await _context.SaveChangesAsync();
    //     return _mapper.Map<VendorResponseDto>(vendor);
    // }
    
     public async Task<VendorResponseDto> CreateVendorAsync(VendorRequestDto dto)
        {
            var vendor = _mapper.Map<Vendor>(dto);
            vendor.Id = Guid.NewGuid();
            vendor.CreatedAt = DateTime.UtcNow;
            vendor.UpdatedAt = DateTime.UtcNow;
    
            vendor.VendorCategories = dto.CategoryIds
                .Distinct()
                .Select(categoryId => new VendorCategory
                {
                    VendorId = vendor.Id,
                    CategoryId = categoryId
                })
                .ToList();
    
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
    
            var createdVendor = await _context.Vendors
                .Include(v => v.VendorCategories)
                    .ThenInclude(vc => vc.Category)
                .Include(v => v.Documents)
                .AsNoTracking()
                .FirstAsync(v => v.Id == vendor.Id);
    
            return _mapper.Map<VendorResponseDto>(createdVendor);
        }
    
        public async Task<VendorResponseDto?> UpdateVendorAsync(Guid id, VendorRequestDto dto)
        {
            var vendor = await _context.Vendors
                .Include(v => v.VendorCategories)
                .Include(v => v.Documents)
                .FirstOrDefaultAsync(v => v.Id == id);
    
            if (vendor == null) return null;
    
            // update scalar fields
            vendor.VendorName = dto.VendorName;
            vendor.CompanyName = dto.CompanyName;
            vendor.Email = dto.Email;
            vendor.PhoneNumber = dto.PhoneNumber;
            vendor.Address = dto.Address;
            vendor.TaxId = dto.TaxId;
            vendor.TaxType = dto.TaxType;
            vendor.Status = dto.Status;
            vendor.BankName = dto.BankName;
            vendor.BankBranch = dto.BankBranch;
            vendor.BankAccount = dto.BankAccount;
            vendor.PaymentTerms = dto.PaymentTerms;
            vendor.UpdatedAt = DateTime.UtcNow;
    
            // sync many-to-many VendorCategories
            var incomingCategoryIds = dto.CategoryIds
                .Distinct()
                .ToHashSet();
    
            var existingCategoryIds = vendor.VendorCategories
                .Select(vc => vc.CategoryId)
                .ToHashSet();
    
            var categoriesToRemove = vendor.VendorCategories
                .Where(vc => !incomingCategoryIds.Contains(vc.CategoryId))
                .ToList();
    
            if (categoriesToRemove.Count > 0)
            {
                _context.RemoveRange(categoriesToRemove);
            }
    
            var categoriesToAdd = incomingCategoryIds
                .Where(categoryId => !existingCategoryIds.Contains(categoryId))
                .Select(categoryId => new VendorCategory
                {
                    VendorId = vendor.Id,
                    CategoryId = categoryId
                })
                .ToList();
    
            if (categoriesToAdd.Count > 0)
            {
                foreach (var vendorCategory in categoriesToAdd)
                {
                    vendor.VendorCategories.Add(vendorCategory);
                }
            }
    
            await _context.SaveChangesAsync();
    
            var updatedVendor = await _context.Vendors
                .Include(v => v.VendorCategories)
                    .ThenInclude(vc => vc.Category)
                .Include(v => v.Documents)
                .AsNoTracking()
                .FirstAsync(v => v.Id == id);
    
            return _mapper.Map<VendorResponseDto>(updatedVendor);
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
            .Include(c => c.VendorCategories)
            .ThenInclude(vc => vc.Category)
            .Include(v => v.Documents)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vendor == null) return null;

        return _mapper.Map<VendorResponseDto>(vendor);
    }


    // public async Task<IEnumerable<VendorResponseDto>> GetAllVendorsAsync(string? status = null, string? search = null)
    // {
    //     var q = _context.Vendors.Include(v => v.Documents)
    //         .Include(c => c.VendorCategories).AsQueryable();
    //     if (!string.IsNullOrEmpty(status)) q = q.Where(v => v.Status == status);
    //     if (!string.IsNullOrEmpty(search))
    //         q = q.Where(v => v.VendorName.Contains(search) || v.CompanyName.Contains(search));
    //     return await q.OrderByDescending(v => v.CreatedAt)
    //         .Select(v => _mapper.Map<VendorResponseDto>(v))
    //         .ToListAsync();
    // }
    
    public async Task<IEnumerable<VendorResponseDto>> GetAllVendorsAsync(string? status = null, string? search = null)
        {
            var query = _context.Vendors
                .Include(v => v.Documents)
                .Include(v => v.VendorCategories)
                    .ThenInclude(vc => vc.Category)
                .AsNoTracking()
                .AsQueryable();
    
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(v => v.Status == status);
            }
    
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
    
                query = query.Where(v =>
                    v.VendorName.Contains(searchTerm) ||
                    (v.CompanyName != null && v.CompanyName.Contains(searchTerm)) ||
                    (v.Email != null && v.Email.Contains(searchTerm)));
            }
    
            var vendors = await query
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
    
            return _mapper.Map<List<VendorResponseDto>>(vendors);
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