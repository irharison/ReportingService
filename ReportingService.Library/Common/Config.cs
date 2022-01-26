using ReportingService.Library.Interfaces;

namespace ReportingService.Library.Common
{
    public class Config : IConfiguration
    {
        public string ReportLocation { get; set; }
        public int IntervalInMinutes { get; set; }
    }
}