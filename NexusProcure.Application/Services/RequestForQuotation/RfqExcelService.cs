using NexusProcure.Application.Interfaces.RequestForQuotation;
using NexusProcure.Core.DTOs.RFQ;
using OfficeOpenXml;

namespace NexusProcure.Application.Services.RequestForQuotation;

public class RfqExcelService : IRfqExcelService
{
    public byte[] GenerateTemplate(PublicRfqDto rfq)
    {
        using var package = new ExcelPackage();

        var ws = package.Workbook.Worksheets.Add("Quotation");

        ws.Cells[1, 1].Value = "ItemName";
        ws.Cells[1, 2].Value = "Quantity";
        ws.Cells[1, 3].Value = "UnitPrice";
        ws.Cells[1, 4].Value = "TaxPercentage";
        ws.Cells[1, 5].Value = "Remarks";

        int row = 2;
        foreach (var item in rfq.Items)
        {
            ws.Cells[row, 1].Value = item.ItemName;
            ws.Cells[row, 2].Value = item.Quantity;

            ws.Cells[row, 1, row, 2].Style.Locked = true;
            row++;
        }

        ws.Cells.AutoFitColumns();
        ws.Protection.IsProtected = true;

        return package.GetAsByteArray();
    }
}