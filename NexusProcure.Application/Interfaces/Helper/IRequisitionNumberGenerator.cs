namespace NexusProcure.Application.Interfaces.Helper;

public interface IRequisitionNumberGenerator
{
    Task<string> GenerateRequisitionNumberAsync();
}