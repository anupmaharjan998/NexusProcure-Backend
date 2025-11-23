using System.ComponentModel.DataAnnotations;

namespace NexusProcure.Core.DTOs;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Description { get; set; }
    
    public Guid? HeadId { get; set; }
    public string HeadName { get; set; }
    public int EmployeesCount { get; set; }
}

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Please enter department name.")]
    public string DepartmentName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? HeadId { get; set; }
}

public class UpdateDepartmentDto
{
    public string? DepartmentName { get; set; }
    public Guid? HeadId { get; set; }
    public string? Description { get; set; }
}