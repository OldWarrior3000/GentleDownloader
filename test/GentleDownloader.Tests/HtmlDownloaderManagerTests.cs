using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace GentleDownloader.Tests
{
    public class HtmlDownloaderManagerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Uri _address;
        private readonly ILogger<HtmlDownloaderManager> _logger;
        private readonly Mock<IHtmlDownloader> _downloaderMock;
        private readonly StringWriter _stringWriter;
        private readonly IOptions<DownloaderSettings> _downloaderSettingsOptions;

        public HtmlDownloaderManagerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _address = new Uri("http://www.google.com");
            var downloaderSettings = new DownloaderSettings()
            {
                RequestsPerMinute = 10,
                NumberOfRetries = 20,
                Timeout = 10000
            };

            _downloaderSettingsOptions = Options.Create(downloaderSettings);
            _stringWriter = new StringWriter();
            _logger = LoggingHelper.CreateLogger<HtmlDownloaderManager>();
            Console.SetOut(_stringWriter);

            var htmlInfo = new HtmlInfo
            {
                Content = string.Empty,
                Link = new Uri("https://www.vecernji.hr/")
            };

            _downloaderMock = new Mock<IHtmlDownloader>();
            _downloaderMock.Setup(x => x.DownloadStringAsync(It.IsAny<Uri>()))
                .ReturnsAsync(htmlInfo);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TestSingleDownload()
        {
            
            var htmlDownloadManager = new HtmlDownloaderManager(_downloaderMock.Object, _downloaderSettingsOptions, _logger);

            var result = await htmlDownloadManager.Download(_address);

            result.Content.Should().Be(string.Empty);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TestMultipleDownloads()
        {
            const int max = 15;
            var htmlDownloadManager = new HtmlDownloaderManager(_downloaderMock.Object, _downloaderSettingsOptions, _logger);
            var results = new List<HtmlInfo>();

            for (var i = 0; i < max; i++)
            {
                results.Add(await htmlDownloadManager.Download(_address));
            }

            results.Should().HaveCount(max);
            _testOutputHelper.WriteLine(_stringWriter.ToString());
        }
    }
}
