using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using ReportingService.Library.Common;
using ReportingService.Library.Interfaces;
using Services;

namespace ReportingService.Library.Test
{
    [TestFixture]
    public class ReportGeneratorTest
    {
        private ReportGenerator _reportGenerator;
        private DateTime _testDate= new DateTime(2022, 2, 1);
        private IConfiguration _config;
        private IPowerService _mockPowerService;
        private Mock _mocker;
        private ILog _logger;

        [SetUp]
        public void SetUp()
        {
            _config = new Config() { ReportLocation = @"C:\temp" };
            
            _mockPowerService = Mock.Of<IPowerService>();
            
            Mock.Get(_mockPowerService)
                .Setup(x => x.GetTrades(_testDate))
                .Returns(GetSamplePowerTrades().AsEnumerable());
            
            _logger = new Mock<ILog>().Object;
            _reportGenerator = new ReportGenerator(_config, _mockPowerService, _logger);
        }

        [Test]
        public void AggregationTest()
        {
            var output = _reportGenerator.AggregatePositions((GetSamplePowerTrades().ToList()));
            
            Assert.AreEqual(110, output[1]);
            Assert.AreEqual(220, output[2]);
            Assert.AreEqual(330, output[3]);
            Assert.AreEqual(440, output[4]);
            Assert.AreEqual(50, output[5]);
        }

        public List<PowerTrade> GetSamplePowerTrades()
        {
            PowerTrade powerTrade1 = PowerTrade.Create(_testDate, 5);
            powerTrade1.Periods[0].Volume = 10;
            powerTrade1.Periods[1].Volume = 20;
            powerTrade1.Periods[2].Volume = 30;
            powerTrade1.Periods[3].Volume = 40;
            powerTrade1.Periods[4].Volume = 50;

            PowerTrade powerTrade2 = PowerTrade.Create(_testDate, 4);
            powerTrade2.Periods[0].Volume = 100;
            powerTrade2.Periods[1].Volume = 200;
            powerTrade2.Periods[2].Volume = 300;
            powerTrade2.Periods[3].Volume = 400;

            return  new List<PowerTrade>()  {powerTrade1, powerTrade2};
        }

        [Test]
        public void GetLocalTimeForPeriodTest()
        {
            Assert.AreEqual("23:00", _reportGenerator.GetLocalTimeForPeriod(1));
            Assert.AreEqual("00:00", _reportGenerator.GetLocalTimeForPeriod(2));
            Assert.AreEqual("01:00", _reportGenerator.GetLocalTimeForPeriod(3));
            Assert.AreEqual("22:00", _reportGenerator.GetLocalTimeForPeriod(24));
        }

        [Test]
        public void GetOutputFileNameTest()
        {
            Assert.AreEqual(@"C:\temp\PowerPosition_20220201_0000.csv", _reportGenerator.GetOutputFileName(_testDate));
        }

        [Test]
        public void RunReportTest()
        {
            _reportGenerator.RunReport(_testDate);
            Assert.True(File.Exists(@"C:\temp\PowerPosition_20220201_0000.csv"));
            var testFile = File.ReadLines(@"C:\temp\PowerPosition_20220201_0000.csv").ToArray();
            
            Assert.AreEqual("Local Time,Volume", testFile[0]);
            Assert.AreEqual("1,110", testFile[1]);
            Assert.AreEqual("2,220", testFile[2]);
            Assert.AreEqual("3,330", testFile[3]);
            Assert.AreEqual("4,440", testFile[4]);
            Assert.AreEqual("5,50", testFile[5]);
        }
        
    }
}