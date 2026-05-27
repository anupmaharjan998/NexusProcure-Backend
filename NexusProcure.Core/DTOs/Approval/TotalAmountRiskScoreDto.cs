namespace NexusProcure.Core.DTOs.Approval;

public class TotalAmountRiskScoreDto
{
    public decimal MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int RiskPoints { get; set; }
    public bool IsActive { get; set; }
}

public class TotalAmountRiskScoreResponseDto
{
    public Guid Id { get; set; }
    public decimal MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int RiskPoints { get; set; }
    public bool IsActive { get; set; }
}
