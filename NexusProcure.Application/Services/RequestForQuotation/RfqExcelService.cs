using ClosedXML.Excel;
using NexusProcure.Application.Interfaces.RequestForQuotation;
using NexusProcure.Core.DTOs.RFQ;
using OfficeOpenXml;

namespace NexusProcure.Application.Services.RequestForQuotation;

public class RfqExcelService : IRfqExcelService
{
    public byte[] GenerateTemplate(PublicRfqDto rfq)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Quotation");

        // ================= HEADER =================
        ws.Cell("A1").Value = "RFQ Number:";
        ws.Cell("B1").Value = rfq.RfqNumber;

        ws.Cell("A2").Value = "Submission Deadline:";
        ws.Cell("B2").Value = rfq.SubmissionDeadline.ToString("yyyy-MM-dd");

        ws.Range("A1:B2").Style.Font.Bold = true;

        // ================= TABLE HEADER =================
        ws.Cell("A4").Value = "Item Name";
        ws.Cell("B4").Value = "Quantity";
        ws.Cell("C4").Value = "Unit Price";
        ws.Cell("D4").Value = "VAT %";
        ws.Cell("E4").Value = "Line Total";

        ws.Range("A4:E4").Style
            .Font.SetBold()
            .Fill.SetBackgroundColor(XLColor.LightGray)
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

        // ================= ITEMS =================
        int row = 5;
        foreach (var item in rfq.Items)
        {
            ws.Cell(row, 1).Value = item.ItemName;
            ws.Cell(row, 2).Value = item.Quantity;

            // Vendor editable
            ws.Cell(row, 3).Value = 0;
            ws.Cell(row, 4).Value = 0;

            // Formula: (Qty * UnitPrice) + VAT
            ws.Cell(row, 5).FormulaA1 =
                $"=B{row}*C{row} + (B{row}*C{row}*D{row}/100)";

            // Lock RFQ columns
            ws.Cell(row, 1).Style.Protection.Locked = true;
            ws.Cell(row, 2).Style.Protection.Locked = true;

            // Unlock vendor columns
            ws.Cell(row, 3).Style.Protection.Locked = false;
            ws.Cell(row, 4).Style.Protection.Locked = false;

            row++;
        }

        // ================= TOTAL =================
        ws.Cell(row + 1, 4).Value = "Grand Total:";
        ws.Cell(row + 1, 5).FormulaA1 = $"=SUM(E5:E{row - 1})";

        ws.Range($"D{row + 1}:E{row + 1}")
            .Style.Font.SetBold();

        // ================= PROTECTION =================
        var protection = ws.Protect("rfq");
        // protection.SelectLockedCells = false;
        // protection.SelectUnlockedCells = true;


        ws.Columns().AdjustToContents();

        // ================= EXPORT =================
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}