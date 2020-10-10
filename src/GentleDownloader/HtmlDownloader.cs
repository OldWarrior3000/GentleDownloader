using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GentleDownloader
{
    public sealed class HtmlDownloader : IHtmlDownloader
    {
        public readonly HttpClient _client;
        
        private readonly string _userAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:79.0) Gecko/20100101 Firefox/79.0";

        public HtmlDownloader(HttpClient client)
        {
            _client = client;

            _client.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
        }

        public async Task<HtmlInfo> DownloadStringAsync(Uri uri)
        {
            if (!uri.AbsoluteUri.StartsWith("http"))
            {
                return new HtmlInfo();
            }

            var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync());
            var content = await streamReader.ReadToEndAsync();

            return new HtmlInfo
            {
                Content = content,
                Link = uri
            };
        }
    }
}
