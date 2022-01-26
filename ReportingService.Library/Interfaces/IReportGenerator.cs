using System;

namespace ReportingService.Library.Interfaces
{
    public interface IReportGenerator
    {
        void RunReport(DateTime runTime);
    }
}