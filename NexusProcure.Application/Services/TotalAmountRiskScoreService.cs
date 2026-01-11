using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Approval;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;


public class TotalAmountRiskScoreService : ITotalAmountRiskScoreService
{
    private readonly NexusProcureDbContext _context;

    public TotalAmountRiskScoreService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<List<TotalAmountRiskScoreResponseDto>> GetAllAsync()
    {
        return await _context.TotalAmountRiskScores
            .OrderBy(r => r.MinAmount)
            .Select(r => new TotalAmountRiskScoreResponseDto
            {
                Id = r.Id,
                MinAmount = r.MinAmount,
                MaxAmount = r.MaxAmount,
                RiskPoints = r.RiskPoints,
                IsActive = r.IsActive
            })
            .ToListAsync();
    }

    public async Task<TotalAmountRiskScoreResponseDto> GetByIdAsync(Guid id)
    {
        var entity = await _context.TotalAmountRiskScores.FindAsync(id);

        if (entity == null)
            throw new KeyNotFoundException("Risk score rule not found");

        return new TotalAmountRiskScoreResponseDto
        {
            Id = entity.Id,
            MinAmount = entity.MinAmount,
            MaxAmount = entity.MaxAmount,
            RiskPoints = entity.RiskPoints,
            IsActive = entity.IsActive
        };
    }

    public async Task<Guid> CreateAsync(TotalAmountRiskScoreDto dto)
    {
        // Optional validation: overlapping ranges
        var overlapExists = await _context.TotalAmountRiskScores.AnyAsync(r =>
            r.IsActive &&
            dto.MinAmount <= r.MaxAmount &&
            dto.MaxAmount >= r.MinAmount);

        if (overlapExists)
            throw new InvalidOperationException("Overlapping amount range detected");

        var entity = new TotalAmountRiskScore
        {
            Id = Guid.NewGuid(),
            MinAmount = dto.MinAmount,
            MaxAmount = dto.MaxAmount,
            RiskPoints = dto.RiskPoints,
            IsActive = dto.IsActive
        };

        _context.TotalAmountRiskScores.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(Guid id, TotalAmountRiskScoreDto dto)
    {
        var entity = await _context.TotalAmountRiskScores.FindAsync(id);

        if (entity == null)
            throw new KeyNotFoundException("Risk score rule not found");

        entity.MinAmount = dto.MinAmount;
        entity.MaxAmount = dto.MaxAmount;
        entity.RiskPoints = dto.RiskPoints;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.TotalAmountRiskScores.FindAsync(id);

        if (entity == null)
            throw new KeyNotFoundException("Risk score rule not found");

        _context.TotalAmountRiskScores.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
