namespace SkinHolderAPI.DTOs.External;

public class ExtensionUsageDto
{
    public int AvgDailyRequests3m { get; set; }
    public int TotalRequests3m { get; set; }
    public int AvgDailyRequests1m { get; set; }
    public int TotalRequests1m { get; set; }
    public double RequestsGrowthRateLastMonth { get; set; } 
    public int MaxRequestsInADay3m { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}
