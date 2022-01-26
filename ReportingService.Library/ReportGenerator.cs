using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using log4net;
using ReportingService.Library.Interfaces;
using Services;

namespace ReportingService.Library
{
    public class ReportGenerator : IReportGenerator
    {
        private readonly IConfiguration  _config;
        private readonly IPowerService _powerService;
        private ILog _logger;
        public ReportGenerator(IConfiguration config, IPowerService powerService, ILog logger)
        {
            _config = config;
            _logger = logger;
            _powerService = powerService;
        }

        public void RunReport(DateTime runDateTime)
        {
            _logger.Info($"Running report for datetime {runDateTime}");
            _logger.Info("Retrieving positions");
            var positions = _powerService.GetTrades(runDateTime);
            _logger.Info("Aggregating positions");
            var aggregates = AggregatePositions(positions);
            _logger.Info("Writing report");
            var fileName = GetOutputFileName(runDateTime);
            WriteReport(aggregates, fileName);
        }

        public string GetOutputFileName(DateTime runDateTime)
        {
            var outputFileName = $"PowerPosition_{runDateTime:yyyyMMdd_HHmm}.csv";
            return Path.Combine(_config.ReportLocation, outputFileName);
        }

        public Dictionary<int,double> AggregatePositions(IEnumerable<PowerTrade> positions)
        {
            var returnValue = new Dictionary<int, double>();

            foreach (var position in positions)
            {
                foreach (var period in position.Periods)
                {
                    if (returnValue.ContainsKey(period.Period))
                    {
                        returnValue[period.Period] += period.Volume;
                    }
                    else
                    {
                        returnValue[period.Period] = period.Volume;
                    }
                }
            }

            return returnValue;
        }

        public void WriteReport(Dictionary<int,double> aggregates, string fileName)
        {
            _logger.Info("Create report directory if it doesn't exist");
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            using (var textWriter = System.IO.File.CreateText(fileName))
            {
                CsvHelper.CsvWriter csvWriter = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
                csvWriter.WriteRecord(new { Time = "Local Time", Volume = "Volume" });
                foreach (var aggregate in aggregates)
                {
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(aggregate);
                }
                csvWriter.NextRecord();
            }
        }

        public string GetLocalTimeForPeriod(int period)
        {
            return DateTime.Today.AddHours(period - 2).ToString("HH:mm");
        }

    }
}