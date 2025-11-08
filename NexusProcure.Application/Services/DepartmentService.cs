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
            var department = _mapper.Map<Department>(dto);
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto?> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return null;

            department.DepartmentName = dto.Name ?? department.DepartmentName;
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
    }