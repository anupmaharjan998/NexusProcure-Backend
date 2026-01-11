namespace NexusProcure.Core.Entities;

public class TotalAmountRiskScore
{
    public Guid Id { get; set; }

    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }

    public int RiskPoints { get; set; }

    public bool IsActive { get; set; }
}
