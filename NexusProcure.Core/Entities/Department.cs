namespace NexusProcure.Core.Entities;

public class Department
{
    public Guid Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    
    public Guid? HeadId { get; set; }
    public User? Head { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Requisition> Requisitions { get; set; } = new List<Requisition>();
}
