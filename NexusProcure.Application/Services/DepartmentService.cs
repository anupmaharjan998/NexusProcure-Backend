using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class DepartmentService : IDepartmentService
    {
        private readonly NexusProcureDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(NexusProcureDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _context.Departments
                .Include(d => d.Head)
                .Include(d => d.Users)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto?> GetByIdAsync(Guid id)
        {
            var department = await _context.Departments
                .Include(d => d.Head)
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);

            return department == null ? null : _mapper.Map<DepartmentDto>(department);
        } 

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            var incomingDepartmentName = (dto.DepartmentName ?? string.Empty).Trim();
            if (await _context.Departments.AsNoTracking().AnyAsync(u => u.DepartmentName.ToLower() == incomingDepartmentName.ToLower()))
            {
                throw new InvalidOperationException("The provided department name already exists.");
            }
            var department = _mapper.Map<Department>(dto);
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto?> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return null;

            department.DepartmentName = dto.DepartmentName ?? department.DepartmentName;
            department.Description = dto.Description;
            if (dto.HeadId.HasValue) department.HeadId = dto.HeadId;
            await _context.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return false;

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> CheckDepartmentNameAsync(string normalizedDepartmentName, Guid? excludeDepartmentId = null)
            {
                var exists = await _context.Departments.AnyAsync(r =>
                    r.DepartmentName.ToLower() == normalizedDepartmentName &&
                    (!excludeDepartmentId.HasValue || r.Id != excludeDepartmentId.Value)
                );
        
                return exists;
            }
    }