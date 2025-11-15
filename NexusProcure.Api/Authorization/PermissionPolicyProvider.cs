using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NexusProcure.Application.Interfaces;

namespace NexusProcure.Api.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;
    private readonly IServiceProvider _serviceProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options, IServiceProvider serviceProvider)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        _serviceProvider = serviceProvider;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() =>
        _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        using var scope = _serviceProvider.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        if (await permissionService.PermissionExistsAsync(policyName))
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(policyName));
            return policy.Build();
        }

        // Policy does not exist → return always fail policy
        var failPolicy = new AuthorizationPolicyBuilder();
        failPolicy.RequireAssertion(_ => false);
        return failPolicy.Build();
    }
}