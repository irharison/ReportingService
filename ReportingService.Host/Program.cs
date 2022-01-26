using System.Configuration;
using ReportingService.Library.Common;
using ReportingService.Library.Interfaces;
using Topshelf;
using log4net;
using ReportingService.Library;
using Services;

namespace ReportingService
{
    public static class Program
    {
        public static ILog Logger = LogManager.GetLogger(nameof(ReportingService));
        public static IConfiguration Configuration = new Config();
        
        public static void Main(string[] args)
        {
            Logger.Info("Starting service");
            Logger.Info("Reading configuration");
            Configuration.ReportLocation = ConfigurationManager.AppSettings["ReportLocation"];
            Configuration.IntervalInMinutes = int.Parse(ConfigurationManager.AppSettings["IntervalInMinutes"]);
            ConfigureService();
        }

        public static void ConfigureService()
        {
            var reportGenerator = new ReportGenerator(Configuration, new PowerService(), Logger);
            
            Logger.Info("Configuring service");
            
            HostFactory.Run(configure =>
            {
                configure.Service<Service>(service =>
                {
                    service.ConstructUsing(s => new Service());
                    service.WhenStarted(s => s.Start(Configuration, reportGenerator));
                    service.WhenStopped(s => s.Stop());
                });
                configure.RunAsLocalService();
                configure.SetServiceName("Reporting Service");
                configure.SetDisplayName("Reporting Service");
                configure.SetDescription("Reporting Service for PowerService");
            });
            
            Logger.Info("End Configuring service");
        }
    }
}