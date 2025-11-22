using System.IdentityModel.Tokens.Jwt;
using Hangfire.Dashboard;

namespace NexusProcure.Api.hangfire;

public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // 1. Check if Authorization header exists
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return false;

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            // 2. Decode JWT
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            // 3. Check the "role" claim
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            return role == "Admin"; // ONLY admins allowed
        }
        catch
        {
            return false;
        }
    }
}
