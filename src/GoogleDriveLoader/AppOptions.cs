using Microsoft.Extensions.Configuration;

using System;
using System.IO;

namespace GoogleDriveLoader
{
    public class AppOptions
    {
        public string OutputFolder { get; set; }
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

            ConsoleOutput.WriteLine("Application configuration loaded!");
            Console.WriteLine($"-> Output folder = {configuration.OutputFolder}");
            Console.WriteLine($"-> Max parallel downloads = {configuration.MaxParallelDownloads}");
            Console.WriteLine();

            return configuration;
        }
    }
}
