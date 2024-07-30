using Microsoft.Extensions.Hosting;
using System.Resources.NetStandard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandLine;
using static CommandLine.Parser;

namespace MyBlueprint.ResxConverter
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args).Build();
            ParserResult<ActionInputs> parser = Default.ParseArguments<ActionInputs>(() => new(), args);
            parser.WithNotParsed(
                errors =>
                {
                    host.Services
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("DotNet.GitHubAction.Program")
                        .LogError("{Errors}", string.Join(
                            Environment.NewLine, errors.Select(error => error.ToString())));

                    Environment.Exit(2);
                });

            parser.WithParsed(
                options => ConvertToResX(options));

            await host.RunAsync();
        }

        public static ValueTask ConvertToResX(ActionInputs options)
        {
            var fileName = "PcspResources.csv";
            var fullPath = Path.Combine(options.InputDirectory, fileName);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            var allLines = File.ReadAllLines(fullPath);
            foreach (var line in allLines)
            {
                var columns = line.Split(new[] { ',' }, 3);
                if (columns.Length == 3)
                {
                    string key = $"{columns[0].Trim()}.{columns[1].Trim()}";
                    string value = columns[2].Trim().TrimStart('\'').TrimEnd('\'');
                    keyValuePairs[key] = value;
                }
            }
            var outputFileName = "ResourceProvider.resx";
            var outputFullPath = Path.Combine(options.OutputDirectory, outputFileName);
            using (var resx = new ResXResourceWriter(outputFullPath))
            {
                foreach (var item in keyValuePairs)
                {
                    resx.AddResource(item.Key, item.Value);
                }
            }
            Environment.Exit(0);
            return ValueTask.CompletedTask;
        }
    }
}