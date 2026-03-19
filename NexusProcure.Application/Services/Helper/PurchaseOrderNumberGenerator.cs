using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Helper;

public class PurchaseOrderNumberGenerator : IPurchaseOrderNumberGenerator
{
    private readonly NexusProcureDbContext _context;

    public PurchaseOrderNumberGenerator(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<string> GeneratePoNumberAsync()
    {
        var nextVal = await _context.Database
            .SqlQueryRaw<long>(
                "SELECT nextval('public.purchase_order_number_seq') AS \"Value\""
            )
            .SingleAsync();

        return $"PO-{nextVal:D6}";
    }
}