using System;
using System.Threading.Tasks;

namespace GentleDownloader
{
    public interface IHtmlDownloader
    {
        Task<HtmlInfo> DownloadStringAsync(Uri uri);
    }
}
