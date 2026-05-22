using NexusProcure.Application.Interfaces;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

using System.Text.Json;

public class AuditService : IAuditService
{
    private readonly NexusProcureDbContext _context;

    public AuditService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string entityName, Guid entityId, string action,
        Guid? performedBy, object? oldValues, object? newValues)
    {
        var log = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            PerformedBy = performedBy,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}