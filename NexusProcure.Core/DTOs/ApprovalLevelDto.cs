namespace NexusProcure.Core.DTOs;

public class ApprovalLevelResponseDto
{
    public Guid Id { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }

    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
}

public class ApprovalLeveRequestlDto
{
    public string LevelName { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }

    public Guid RoleId { get; set; }
}
