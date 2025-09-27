namespace NexusProcure.Core.Entities;

public class ApprovalLevel
{
    public Guid Id { get; set; }
    public string LevelName { get; set; } = string.Empty; // DepartmentHead, FinanceOfficer, CEO
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }
}
