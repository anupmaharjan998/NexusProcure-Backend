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
        ValidateRange(dto);

        var overlapExists = await HasOverlapAsync(dto.MinAmount, dto.MaxAmount, null);

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
        ValidateRange(dto);

        var entity = await _context.TotalAmountRiskScores.FindAsync(id);

        if (entity == null)
            throw new KeyNotFoundException("Risk score rule not found");

        var overlapExists = await HasOverlapAsync(dto.MinAmount, dto.MaxAmount, id);

        if (overlapExists)
            throw new InvalidOperationException("Overlapping amount range detected");

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

    private static void ValidateRange(TotalAmountRiskScoreDto dto)
    {
        if (dto.MinAmount < 0)
            throw new InvalidOperationException("Minimum amount cannot be negative");

        if (dto.MaxAmount.HasValue && dto.MaxAmount.Value <= dto.MinAmount)
            throw new InvalidOperationException("Maximum amount must be greater than minimum amount");

        if (dto.RiskPoints < 0)
            throw new InvalidOperationException("Risk points cannot be negative");
    }

    private async Task<bool> HasOverlapAsync(
        decimal newMin,
        decimal? newMax,
        Guid? excludeId)
    {
        return await _context.TotalAmountRiskScores
            .Where(r => r.IsActive)
            .Where(r => excludeId == null || r.Id != excludeId.Value)
            .AnyAsync(r =>
                newMin < (r.MaxAmount ?? decimal.MaxValue) &&
                (newMax ?? decimal.MaxValue) > r.MinAmount
            );
    }
}