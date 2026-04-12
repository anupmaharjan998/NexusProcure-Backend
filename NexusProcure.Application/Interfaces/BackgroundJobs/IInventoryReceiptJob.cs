namespace NexusProcure.Application.Interfaces.BackgroundJobs;

public interface IInventoryReceiptJob
{
    Task InsertReceivedItemsIntoInventoryAsync(Guid goodsReceiptId);
}