namespace NexusProcure.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string entityName, Guid entityId, string action,
        Guid? performedBy, object? oldValues, object? newValues);
}