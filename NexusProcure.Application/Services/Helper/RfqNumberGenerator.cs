using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Helper;

public class RfqNumberGenerator : IRfqNumberGenerator
{
    private readonly NexusProcureDbContext _context;

    public RfqNumberGenerator(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateRfqNumberAsync()
    {
        var nextVal = await _context.Database
            .SqlQueryRaw<long>(
                "SELECT nextval('public.rfq_number_seq') AS \"Value\""
            )
            .SingleAsync();

        return $"RFQ-{nextVal:D6}";
    }
}
