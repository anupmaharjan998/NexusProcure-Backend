using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

 public class PermissionService : IPermissionService
    {
        private readonly NexusProcureDbContext _context;
        private readonly IMapper _mapper;

        public PermissionService(NexusProcureDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var permissions = await _context.Permissions.ToListAsync();
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task<PermissionDto?> GetByIdAsync(Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            return permission == null ? null : _mapper.Map<PermissionDto>(permission);
        }

        public async Task<bool> AssignPermissionsToRole(Guid roleId, List<Guid> permissionIds)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return false;

            // If Admin, enforce all permissions and ignore requested changes
            if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                permissionIds = await _context.Permissions.Select(p => p.Id).ToListAsync();
            }

            // Remove old permissions
            var existing = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
            _context.RolePermissions.RemoveRange(existing);

            // Add new ones
            var newPermissions = permissionIds.Distinct().Select(pid => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            });

            _context.RolePermissions.AddRange(newPermissions);
            await _context.SaveChangesAsync();

            return true;
        }
    }