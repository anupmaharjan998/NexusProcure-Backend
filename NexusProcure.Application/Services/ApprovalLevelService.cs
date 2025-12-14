using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class ApprovalLevelService : IApprovalLevelService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;

    public ApprovalLevelService(NexusProcureDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApprovalLevelResponseDto> CreateAsync(ApprovalLeveRequestlDto dto)
    {
        var approvalLevel = new ApprovalLevel
        {
            Id = Guid.NewGuid(),
            LevelName = dto.LevelName,
            MinAmount = dto.MinAmount,
            MaxAmount = dto.MaxAmount,
            RoleId = dto.RoleId
        };

        await _context.ApprovalLevels.AddAsync(approvalLevel);
        await _context.SaveChangesAsync();

        return _mapper.Map<ApprovalLevelResponseDto>(approvalLevel);
    }

    public async Task<List<ApprovalLevelResponseDto>> GetAllAsync()
    {
        var res =  await _context.ApprovalLevels
            .Include(a => a.Role)
            .ToListAsync();
        return _mapper.Map<List<ApprovalLevelResponseDto>>(res);
    }
}