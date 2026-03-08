using System.ComponentModel.DataAnnotations;

namespace NexusProcure.Core.Enums;

public enum ApprovalReferenceType
{
    [Display(Name = "Requisition")]
    Requisition = 0,
    
    [Display(Name = "RFQ")]
    RFQ = 1,
}