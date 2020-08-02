namespace GentleDownloader
{
    public class DownloaderSettings
    {
        public int RequestsPerMinute { get; set; }
        public int NumberOfRetries { get; set; }
        public int Timeout { get; set; }
    }
}
