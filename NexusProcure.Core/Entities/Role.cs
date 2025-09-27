namespace NexusProcure.Core.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    // Examples: Admin, CEO, ProcurementOfficer, FinanceOfficer, DepartmentHead, Storekeeper, Employee

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
