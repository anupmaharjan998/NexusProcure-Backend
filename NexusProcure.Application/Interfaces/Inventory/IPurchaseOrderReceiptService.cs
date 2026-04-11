using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IPurchaseOrderReceiptService
{
    Task<GoodsReceiptResultDto> ReceivePurchaseOrderAsync(ReceivePurchaseOrderDto dto, Guid receivedBy);
    
    Task<IEnumerable<PurchaseOrderDeliveryListDto>> GetReceivingDeliveriesAsync(PurchaseOrderDeliveryQueryDto query);
    Task<PurchaseOrderDeliveryListDto?> GetReceivingDeliveryByPurchaseOrderIdAsync(Guid purchaseOrderId);
}