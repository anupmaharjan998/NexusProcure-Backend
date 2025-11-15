using Microsoft.AspNetCore.Authorization;
using NexusProcure.Application.Interfaces;

namespace NexusProcure.Api.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly IHttpContextAccessor _http;

    public PermissionHandler(IPermissionService permissionService, IHttpContextAccessor http)
    {
        _permissionService = permissionService;
        _http = http;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        //var userIdClaim = _http.HttpContext?.User?.FindFirst("userId")?.Value;
        var userIdClaim = "a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4104";
        if (userIdClaim == null)
        {
            context.Fail();
            return;
        }

        var userId = Guid.Parse(userIdClaim);

        // Check if permission exists
        if (!await _permissionService.PermissionExistsAsync(requirement.Permission))
        {
            // Optional: set custom error
            context.Fail(new AuthorizationFailureReason(this, $"Permission '{requirement.Permission}' does not exist."));
            return;
        }

        var permissions = await _permissionService.GetPermissionsForUserAsync(userId);

        if (permissions.Contains(requirement.Permission))
            context.Succeed(requirement);
        else
            context.Fail(new AuthorizationFailureReason(this, $"User does not have permission: {requirement.Permission}"));
    }
}