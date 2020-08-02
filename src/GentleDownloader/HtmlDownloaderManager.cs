using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GentleDownloader
{
    public sealed class HtmlDownloaderManager : IDownloaderManager
    {
        private readonly Dictionary<DateTime, Uri> _requestRegister;
        private readonly IHtmlDownloader _htmlDownloader;
        private readonly DownloaderSettings _downloaderSettings;
        private readonly ILogger<HtmlDownloaderManager> _logger;

        private int _retryCount;
        
        public HtmlDownloaderManager(IHtmlDownloader htmlDownloader, IOptions<DownloaderSettings> downloaderSettings, ILogger<HtmlDownloaderManager> logger)
        {
            _htmlDownloader = htmlDownloader;
            _downloaderSettings = downloaderSettings.Value ?? throw new ArgumentException(nameof(downloaderSettings));
            _logger = logger;
            _requestRegister = new Dictionary<DateTime, Uri>();
            
        }

        public async Task<HtmlInfo> Download(Uri uri)
        {
            _logger.LogInformation($"Downloading {uri.AbsolutePath}");
            QueueRequest(uri);
            AwaitRequestOrder();
            return await _htmlDownloader.DownloadStringAsync(uri);
        }

        private void AwaitRequestOrder()
        {
            while (NumberOfActiveRequestsReached())
            {
                _logger.LogInformation("Exceeded amount of concurrent downloads. Waiting for free slot");
                _retryCount += 1;
                System.Threading.Thread.Sleep(_downloaderSettings.Timeout);
                if (_retryCount >= _downloaderSettings.NumberOfRetries)
                {
                    throw new WebException($"Number of retries exceeded. Max. retries: {_downloaderSettings.NumberOfRetries}");
                }
            }
            _retryCount = 0;
        }

        private bool NumberOfActiveRequestsReached()
        {
            var startTime = DateTime.Now.AddMinutes(-1);
            var currentRequests = _requestRegister.Count(x => x.Key > startTime);
            return currentRequests >= _downloaderSettings.RequestsPerMinute;
        }

        private void QueueRequest(Uri uri)
        {
            _logger.LogInformation($"Adding request uri to downloader {uri.AbsolutePath}");
            _requestRegister.Add(DateTime.Now, uri);
        }
    }
}
