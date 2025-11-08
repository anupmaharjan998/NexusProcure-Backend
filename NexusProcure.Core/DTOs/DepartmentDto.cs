namespace NexusProcure.Core.DTOs;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? HeadName { get; set; }
    public int EmployeesCount { get; set; }
}

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public Guid? HeadId { get; set; }
}

public class UpdateDepartmentDto
{
    public string? Name { get; set; }
    public Guid? HeadId { get; set; }
}