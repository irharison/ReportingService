using System;
using System.Threading;
using System.Threading.Tasks;
using ReportingService.Library.Interfaces;

namespace ReportingService
{
    public class Service
    {
        private IReportGenerator _reportGenerator; 
        public static bool Running = true;
        private IConfiguration _configuration;

        public void Start(IConfiguration configuration, IReportGenerator reportGenerator)
        {
            _reportGenerator = reportGenerator;
            _configuration = configuration;
            Task.Run(() => StartTimerLoop());
        }

        public void Stop()
        {
            Service.Running = false;
        }
        private void StartTimerLoop()
        {
            DateTime lastRunTime = DateTime.Now;
            
            _reportGenerator.RunReport(lastRunTime);

            while (Service.Running)
            {
                var timeSpan = DateTime.Now - lastRunTime;
                if (timeSpan.TotalMinutes >= _configuration.IntervalInMinutes)
                {
                    lastRunTime = DateTime.Now;
                    _reportGenerator.RunReport(lastRunTime);
                }
                
                Thread.Sleep(10000);
            }
        }
    }
}