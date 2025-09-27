namespace NexusProcure.Core.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // Navigation
    public ICollection<Requisition> Requisitions { get; set; } = new List<Requisition>();
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();
}
