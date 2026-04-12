using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs.Email;
using NexusProcure.Core.Entities.RequestForQuotations;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.BackgroundJobs;

public class PurchaseRequestJob : IPurchaseRequestJob
{
    private readonly NexusProcureDbContext _context;
    private readonly IPurchaseOrderService _purchaseOrderService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public PurchaseRequestJob(NexusProcureDbContext context, IPurchaseOrderService purchaseOrderService,
        IEmailService emailService,
        IConfiguration config)
    {
        _context = context;
        _purchaseOrderService = purchaseOrderService;
        _emailService = emailService;
        _config = config;
    }

    public async Task RunAsync()
    {
        var now = DateTime.UtcNow;

        var purchaseOrderArrival = await _context.PurchaseOrders
            .Where(a => a.Quotation.DeliveryDate >= now.Date &&
                        a.Quotation.DeliveryDate < now.Date.AddDays(1))
            .Include(a => a.Quotation)
            .ThenInclude(q => q.Items)
            .Include(a => a.Vendor)
            .ToListAsync();

        var roles = new[] { "Admin", "ProcurementOfficer", "Inventory" };

        var users = await _context.Users
            .Where(u => _context.Roles
                .Where(r => roles.Contains(r.Name))
                .Select(r => r.Id)
                .Contains(u.RoleId))
            .ToListAsync();

        if (!purchaseOrderArrival.Any())
            return;

// Build order table
        var ordersHtml = "";

        foreach (var po in purchaseOrderArrival)
        {
            var itemsRows = "";

            foreach (var item in po.Quotation.Items)
            {
                itemsRows += $@"
                <tr>
                    <td>{item.ItemName}</td>
                    <td>{item.Quantity}</td>
                </tr>";
            }

            ordersHtml += $@"
            <h4>Purchase Order: {po.Id}</h4>
            <p>
                Vendor: {po.Vendor.VendorName}<br/>
                Delivery Date: {po.Quotation.DeliveryDate:yyyy-MM-dd}
            </p>

            <table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;'>
                <thead>
                    <tr>
                        <th>Item</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                    </tr>
                </thead>
                <tbody>
                    {itemsRows}
                </tbody>
            </table>
            <br/>";
        }

        foreach (var user in users)
        {
            await _emailService.SendAsync(new SendEmailDto
            {
                To = user.Email,
                Subject = $"Purchase Orders Arriving Today",
                HtmlBody = $@"
                <p>Dear {user.FullName},</p>

                <p>
                    This is an automated notification that the following purchase
                    orders are scheduled to arrive <strong>today</strong>.
                </p>

                {ordersHtml}

                <p>
                    Please ensure that the receiving process is prepared and the
                    inventory team is ready to verify the delivered items.
                </p>

                <p>
                    If there are any discrepancies during receiving, please report
                    them immediately to the procurement team.
                </p>

                <br/>

                <p>
                    Best regards,<br/>
                    Procurement System<br/>
                    NexusProcure
                </p>"
            });
        }
    }

    public async Task CreatePurchaseRequestAsync(Guid referenceId)
    {
        var po = await _purchaseOrderService.CreateAsync(referenceId);

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Id == po.VendorId);

        if (vendor == null)
            throw new Exception("Vendor not found");

        // 1️⃣ Send email (external side effect)
        await _emailService.SendAsync(new SendEmailDto
        {
            To = vendor.Email,
            Subject = $"Purchase Order Award Notification",
            HtmlBody = $@"
            <p>Dear {vendor.VendorName},</p>

            <p>
                We are pleased to inform you that your quotation has been
                <strong>successfully selected</strong> for our procurement request.
            </p>

            <p>
                Based on your submitted quotation, your company has been
                awarded the purchase order.
            </p>

            <p>
                <strong>Purchase Order Details</strong><br/>
                Purchase Order ID: {po.PurchaseOrderNumber}<br/>
                Order Date: {po.OrderDate:yyyy-MM-dd}
            </p>

            <p>
                Our procurement team will contact you shortly regarding the
                delivery schedule and further coordination.
            </p>

            <p>
                Thank you for your participation and we look forward to
                working with you.
            </p>

            <br/>

            <p>
                Best regards,<br/>
                Procurement Team<br/>
                NexusProcure
            </p>"
        });

        // 2️⃣ Atomic DB changes
        await using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.RfqAudits.Add(new RfqAudit
            {
                Id = Guid.NewGuid(),
                RfqId = referenceId,
                Action = "PurchaseOrderCreated",
                CreatedAt = DateTime.UtcNow,
                PerformedBy = "System"
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}