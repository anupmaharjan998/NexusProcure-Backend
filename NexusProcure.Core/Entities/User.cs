namespace NexusProcure.Core.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    // Role & Department
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // Reporting Hierarchy
    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }
    public ICollection<User> Subordinates { get; set; } = new List<User>();

    // Approval Delegation (Optional)
    public Guid? DelegateUserId { get; set; }
    public User? DelegateUser { get; set; }

    // Password Reset
    public Guid? PasswordResetToken { get; set; }
    public DateTime PasswordResetTokenExpiration { get; set; }
    public bool PasswordResetTokenUsed { get; set; } = false;

    // Profile
    public string? ProfileImageUrl { get; set; }
    public string? ProfileImagePublicId { get; set; }

    // Navigation
    public ICollection<Requisition> Requisitions { get; set; } = new List<Requisition>();
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();
}