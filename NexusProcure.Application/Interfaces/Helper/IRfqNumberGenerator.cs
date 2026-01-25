namespace NexusProcure.Application.Interfaces.Helper;

public interface IRfqNumberGenerator
{
    Task<string> GenerateRfqNumberAsync();
}