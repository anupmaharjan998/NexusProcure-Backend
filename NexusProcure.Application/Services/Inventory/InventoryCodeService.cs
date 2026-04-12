using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryCodeService : IInventoryCodeService
{
    private readonly NexusProcureDbContext _context;

    public InventoryCodeService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<SkuPreviewResponseDto> GenerateSkuAndBarcodeAsync(string itemName, Guid categoryId)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new Exception("Inventory category not found.");

        var categoryCode = string.IsNullOrWhiteSpace(category.CategoryCode)
            ? "GEN"
            : category.CategoryCode.Trim().ToUpper();

        var itemPart = BuildItemCode(itemName);

        var prefix = $"{categoryCode}-{itemPart}";

        var existingCount = await _context.InventoryItems
            .CountAsync(i => i.InventoryCategoryId == categoryId &&
                             i.SKU.StartsWith(prefix));

        var nextNumber = existingCount + 1;
        var serialPart = nextNumber.ToString("D3");

        var sku = $"{prefix}-{serialPart}";
        var barcode = sku;

        return new SkuPreviewResponseDto
        {
            Sku = sku,
            Barcode = barcode
        };
    }

    private static string BuildItemCode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "ITEM";

        var words = Regex.Replace(input.Trim().ToUpper(), @"[^A-Z0-9\s]", "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
            return "ITEM";

        if (words.Length == 1)
            return words[0].Length <= 3 ? words[0] : words[0][..3];

        return string.Concat(words.Take(3).Select(w => w[0]));
    }
}