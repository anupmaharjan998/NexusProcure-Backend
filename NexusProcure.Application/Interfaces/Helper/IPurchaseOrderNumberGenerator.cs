namespace NexusProcure.Application.Interfaces.Helper;

public interface IPurchaseOrderNumberGenerator
{
    Task<string> GeneratePoNumberAsync();
}