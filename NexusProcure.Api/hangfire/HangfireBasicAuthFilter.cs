using Hangfire.Dashboard;

namespace NexusProcure.Api.hangfire;

public class HangfireBasicAuthFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;

    public HangfireBasicAuthFilter(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        string authHeader = httpContext.Request.Headers["Authorization"];

        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            var decodedUsernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword));

            var parts = decodedUsernamePassword.Split(':');
            if (parts.Length == 2)
            {
                var username = parts[0];
                var password = parts[1];

                return username == _username && password == _password;
            }
        }

        httpContext.Response.Headers["WWW-Authenticate"] = "Basic";
        httpContext.Response.StatusCode = 401;

        return false;
    }
}
