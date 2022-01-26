namespace ReportingService.Library.Interfaces
{
    public interface IConfiguration
    {
        string ReportLocation { get; set; }
        int IntervalInMinutes { get; set; }
    }
}