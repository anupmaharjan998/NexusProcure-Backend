namespace NexusProcure.Application.Email;

public static class ProcurementEmailTemplates
{
    public static string ProcurementRequestSubject(string requestNumber)
    {
        return $"New Procurement Request - {requestNumber}";
    }

    public static string ProcurementRequestBody(
        string procurementOfficerName,
        string department,
        string requestedBy,
        string approvedBy,
        IEnumerable<(string itemName, int requestedQty, int availableQty, int procureQty)> items)
    {
        var itemRows = string.Join("", items.Select(x => $@"
            <tr>
                <td style='padding:8px;border:1px solid #ddd;'>{x.itemName}</td>
                <td style='padding:8px;border:1px solid #ddd;text-align:center;'>{x.requestedQty}</td>
                <td style='padding:8px;border:1px solid #ddd;text-align:center;'>{x.availableQty}</td>
                <td style='padding:8px;border:1px solid #ddd;text-align:center;font-weight:bold;'>{x.procureQty}</td>
            </tr>
        "));

        return $@"
            <div style='font-family:Arial,sans-serif;line-height:1.5;color:#222;'>
                <h2>New Procurement Request</h2>

                <p>Dear {procurementOfficerName},</p>

                <p>
                    A requisition request has been approved by the manager and requires procurement.
                </p>

                <p>
                    <strong>Department:</strong> {department}<br/>
                    <strong>Requested By:</strong> {requestedBy}<br/>
                    <strong>Approved By:</strong> {approvedBy}
                </p>

                <table style='border-collapse:collapse;width:100%;margin-top:16px;'>
                    <thead>
                        <tr style='background:#f3f4f6;'>
                            <th style='padding:8px;border:1px solid #ddd;text-align:left;'>Item</th>
                            <th style='padding:8px;border:1px solid #ddd;'>Requested Qty</th>
                            <th style='padding:8px;border:1px solid #ddd;'>Available Qty</th>
                            <th style='padding:8px;border:1px solid #ddd;'>Qty To Procure</th>
                        </tr>
                    </thead>
                    <tbody>
                        {itemRows}
                    </tbody>
                </table>

                <p style='margin-top:20px;'>
                    Please review this request from the Procurement Request page and start the purchasing process.
                </p>

                <p>Regards,<br/>Inventory Management System</p>
            </div>
        ";
    }
}