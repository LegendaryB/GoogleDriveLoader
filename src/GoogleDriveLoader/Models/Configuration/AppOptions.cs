using Microsoft.Extensions.Configuration;

using System;
using System.IO;

namespace GoogleDriveLoader.Models.Configuration
{
    public class AppOptions
    {
        public string DownloadRootDirectory { get; set; }
        public int MaxParallelDownloads { get; set; }

        internal static AppOptions Get()
        {
            if (!File.Exists("appsettings.json"))
                throw new FileNotFoundException();

            var builder = new ConfigurationBuilder()
                   .SetBasePath(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))
                   .AddJsonFile("appsettings.json", false);

            var configurationRoot = builder.Build();
            var configuration = configurationRoot.Get<AppOptions>();

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(configuration.DownloadRootDirectory))
                throw new ArgumentNullException(nameof(configuration.DownloadRootDirectory));

            return configuration;
        }
    }
}
