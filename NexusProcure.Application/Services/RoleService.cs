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
        var roles = await _context.Roles.ToListAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id);

        return role == null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        var role = _mapper.Map<Role>(dto);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleDto dto)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return null;

        // Admin role cannot be modified
        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        
        if (!string.IsNullOrEmpty(dto.Name)) role.Name = dto.Name;
        role.Description = dto.Description;
        await _context.SaveChangesAsync();

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return false;

        // Admin role cannot be deleted
        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }
}