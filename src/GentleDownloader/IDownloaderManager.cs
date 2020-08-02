using System;
using System.Threading.Tasks;

namespace GentleDownloader
{
    public interface IDownloaderManager
    {
        Task<HtmlInfo> Download(Uri uri);
    }
}
