namespace NexusProcure.Application.Models;

public class RiskSettings
{
    public int CriticalScore { get; set; } = 70;
    public int HighScore { get; set; } = 50;
    public int MediumScore { get; set; } = 30;
    public int UrgentRiskScore { get; set; } = 15;
}