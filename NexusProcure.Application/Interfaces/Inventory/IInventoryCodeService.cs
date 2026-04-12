using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryCodeService
{
    Task<SkuPreviewResponseDto> GenerateSkuAndBarcodeAsync(string itemName, Guid categoryId);
}