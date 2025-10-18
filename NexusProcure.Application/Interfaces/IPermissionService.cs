using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetAllAsync();
    Task<PermissionDto?> GetByIdAsync(Guid id);

    // Extra: Assign permissions to role
    Task<bool> AssignPermissionsToRole(Guid roleId, List<Guid> permissionIds);
}