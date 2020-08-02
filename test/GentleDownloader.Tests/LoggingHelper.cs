using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GentleDownloader.Tests
{
    public static class LoggingHelper
    {
        private static ServiceProvider InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(configure => configure.AddSerilog())
                
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace);
            return serviceCollection.BuildServiceProvider();
        }

        public static ILogger<T> CreateLogger<T>()
        {
            var serviceProvider = InitLogger();
            return serviceProvider.GetService<ILogger<T>>();
        }
    }
}
