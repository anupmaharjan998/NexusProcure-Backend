using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(Guid id);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
    Task<DepartmentDto?> UpdateAsync(Guid id, UpdateDepartmentDto dto);
    Task<bool> DeleteAsync(Guid id);
}