using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class UserService : IUserService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;

    public UserService(NexusProcureDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
        var user = _mapper.Map<User>(dto);
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
        if (dto.RoleId.HasValue) user.RoleId = dto.RoleId.Value;
        if (dto.DepartmentId.HasValue) user.DepartmentId = dto.DepartmentId.Value;
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

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
}