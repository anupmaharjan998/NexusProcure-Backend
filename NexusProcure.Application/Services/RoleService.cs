using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class RoleService : IRoleService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;

    public RoleService(NexusProcureDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                PermissionCount = r.RolePermissions.Count
            })
            .ToListAsync();
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .AsNoTracking()
            .Include(r => r.RolePermissions)
            .Where(r => r.Id == id)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                PermissionCount = r.RolePermissions.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        var incomingRoleName = (dto.Name ?? string.Empty).Trim();
        if (await _context.Roles.AsNoTracking().AnyAsync(u => u.Name.ToLower() == incomingRoleName.ToLower()))
        {
            throw new InvalidOperationException("A user with the provided email already exists.");
        }        
        
        var role = _mapper.Map<Role>(dto);

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            PermissionCount = 0
        };
    }

    public async Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleDto dto)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            return null;

        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            role.Name = dto.Name.Trim();

        role.Description = dto.Description?.Trim() ?? string.Empty;

        await _context.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            PermissionCount = role.RolePermissions.Count
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null)
            return false;

        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> CheckRoleNameAsync(string normalizedRoleName, Guid? excludeRoleId = null)
    {
        var exists = await _context.Roles.AnyAsync(r =>
            r.Name.ToLower() == normalizedRoleName &&
            (!excludeRoleId.HasValue || r.Id != excludeRoleId.Value)
        );

        return exists;
    }
}