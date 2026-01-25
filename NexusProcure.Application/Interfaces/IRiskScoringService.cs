using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;

namespace NexusProcure.Application.Interfaces;

public interface IRiskScoringService
{
    Task<int> CalculateRiskScoreAsync(Requisition requisition);
    RiskLevel ResolveRiskLevel(int score);
    // Task<RiskLevel> CalculateAsync(Guid requisitionId);
    Task<RiskLevel> CalculateRiskLevelAsync(Requisition requisition);
}
