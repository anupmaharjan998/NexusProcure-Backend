using System.Net;
using System.Security.Cryptography;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;
using Supabase;

namespace NexusProcure.Application.Services;

public class UserService : IUserService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly Client _supabase;

    public UserService(NexusProcureDbContext context, IMapper mapper, IEmailService emailService, Client supabase)
    {
        _context = context;
        _mapper = mapper;
        _emailService = emailService;
        _supabase = supabase;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var incomingEmail = (dto.Email ?? string.Empty).Trim();
        if (await _context.Users.AsNoTracking().AnyAsync(u => u.Email.ToLower() == incomingEmail.ToLower()))
        {
            throw new InvalidOperationException("A user with the provided email already exists.");
        }

        var user = _mapper.Map<User>(dto);
        
        var generatedPassword = GenerateSecurePassword();
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, generatedPassword);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        BackgroundJob.Enqueue<IEmailJobService>(job =>
            job.SendUserCreatedEmailAsync(
                user.Email,
                user.FullName,
                user.Username,
                generatedPassword
            )
        );

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
        if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
        if (dto.RoleId.HasValue) user.RoleId = dto.RoleId.Value;
        if (dto.DepartmentId.HasValue) user.DepartmentId = dto.DepartmentId.Value;
        user.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<ProfileImageResponse> UploadProfilePictureAsync(string email, IFormFile file)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null)
        return new ProfileImageResponse { Success = false, Message = "User not found" };

    if (file == null || file.Length == 0)
        return new ProfileImageResponse { Success = false, Message = "Invalid file" };

    // Your bucket
    var bucket = "profile-pictures";

    // File path inside bucket
    var filePath = $"{user.Id}/{file.FileName}";

    try
    {
        // Convert IFormFile → byte[]
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        // Upload to Supabase Storage
        var uploadResponse = await _supabase.Storage
            .From(bucket)
            .Upload(
                fileBytes,
                filePath,
                new Supabase.Storage.FileOptions
                {
                    ContentType = file.ContentType,
                    Upsert = true
                }
            );

        if (uploadResponse == null)
            return new ProfileImageResponse { Success = false, Message = "Upload failed" };

        // Delete old profile image
        if (!string.IsNullOrWhiteSpace(user.ProfileImagePublicId))
        {
            await _supabase.Storage
                .From(bucket)
                .Remove(new List<string> { user.ProfileImagePublicId });
        }

        // Generate public URL
        string publicUrl = _supabase.Storage
            .From(bucket)
            .GetPublicUrl(filePath);

        // Save changes in database
        user.ProfileImageUrl = publicUrl;
        user.ProfileImagePublicId = filePath;

        await _context.SaveChangesAsync();

        return new ProfileImageResponse
        {
            Success = true,
            Url = publicUrl
        };
    }
    catch (Exception ex)
    {
        return new ProfileImageResponse
        {
            Success = false,
            Message = "Upload error: " + ex.Message
        };
    }
}

    public async Task<UserDto?> UserProfileUpdateAsync(Guid id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(dto.PhoneNumber))user.PhoneNumber = dto.PhoneNumber;
        if (!string.IsNullOrEmpty(dto.Address))user.Address = dto.Address;

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }


    private static string GenerateSecurePassword(int length = 12)
    {
        // Generate a password with letters, digits, and symbols ensuring complexity
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string symbols = "!@$%^&*?-_";
        var all = upper + lower + digits + symbols;

        var result = new char[length];

        // Ensure at least one from each category
        result[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        result[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        result[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        result[3] = symbols[RandomNumberGenerator.GetInt32(symbols.Length)];

        for (int i = 4; i < length; i++)
        {
            result[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }
        
        for (int i = result.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }

        return new string(result);
    }
}