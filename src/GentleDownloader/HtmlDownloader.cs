using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GentleDownloader
{
    public sealed class HtmlDownloader : IHtmlDownloader
    {
        public readonly HttpClient _client;

        public HtmlDownloader(HttpClient client)
        {
            _client = client;
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
