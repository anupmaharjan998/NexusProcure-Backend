using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Helper;

public class RequisitionNumberGenerator : IRequisitionNumberGenerator
{
    private readonly NexusProcureDbContext _context;

    public RequisitionNumberGenerator(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateRequisitionNumberAsync()
    {
        var nextVal = await _context.Database
            .SqlQueryRaw<long>(
                "SELECT nextval('public.requisition_number_seq') AS \"Value\""
            )
            .SingleAsync();

        return $"REQ-{nextVal:D6}";
    }

}
