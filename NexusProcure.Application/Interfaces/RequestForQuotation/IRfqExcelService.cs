using NexusProcure.Core.DTOs.RFQ;

namespace NexusProcure.Application.Interfaces.RequestForQuotation;

public interface IRfqExcelService
{
    byte[] GenerateTemplate(PublicRfqDto rfq);
}
