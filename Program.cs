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
            var filesToConvert = new List<string>();
            filesToConvert.AddRange(Directory.GetFiles(options.InputDirectory, $"*.csv", SearchOption.AllDirectories));

            foreach (var file in filesToConvert)
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                var allLines = File.ReadAllLines(file);
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

                var outputFile = file switch
                {
                    string f when f.Contains("en-CA") => "ResourceProvider.en-CA.resx",
                    string f when f.Contains("en-US") => "ResourceProvider.en-US.resx",
                    string f when f.Contains("fr") => "ResourceProvider.fr.resx",
                    _ => string.Empty
                };

                var outputFullPath = Path.Combine(options.OutputDirectory, outputFile);
                using (var resx = new ResXResourceWriter(outputFullPath))
                {
                    foreach (var item in keyValuePairs)
                    {
                        resx.AddResource(item.Key, item.Value);
                    }
                }
            }

            Environment.Exit(0);
            return ValueTask.CompletedTask;
        }
    }
}