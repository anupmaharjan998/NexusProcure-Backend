namespace NexusProcure.Core.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    
    public Guid? PasswordResetToken { get; set; }
    public DateTime PasswordResetTokenExpiration { get; set; }
    public bool PasswordResetTokenUsed { get; set; } = false;

    // Navigation
    public ICollection<Requisition> Requisitions { get; set; } = new List<Requisition>();
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();
}
