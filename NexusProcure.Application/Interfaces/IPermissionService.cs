using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetAllAsync();
    Task<PermissionDto?> GetByIdAsync(Guid id);

    Task<bool> AssignPermissionsToRole(Guid roleId, List<Guid> permissionIds);

    Task<IEnumerable<PermissionDto>> GetByRoleIdAsync(Guid roleId);

    Task<List<string>> GetPermissionsForUserAsync(Guid userId);
    
    Task<bool> PermissionExistsAsync(string permissionName);
}