using Microsoft.Extensions.Hosting;
using System.Resources.NetStandard;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandLine;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using static CommandLine.Parser;

namespace MyBlueprint.ResxConverter
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args).Build();
            var parser = Default.ParseArguments<ActionInputs>(() => new(), args);
            parser.WithNotParsed(
                errors =>
                {
                    host.Services
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("MyBlueprint.ResxConverter.Program")
                        .LogError("{Errors}", string.Join(
                            Environment.NewLine, errors.Select(error => error.ToString())));

                    Environment.Exit(2);
                });

            await parser.WithParsedAsync(ConvertToResX);

            await host.RunAsync();
        }
        private static string MakeValidFileName(string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_").Replace(".", "_");
        }

        private static async Task ConvertToResX(ActionInputs options)
        {
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddIncludePatterns(options.Input);

            var result = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(Environment.CurrentDirectory)));
            if (!Directory.Exists(options.OutputDirectory))
            {
                Directory.CreateDirectory(options.OutputDirectory);
            }

            foreach (var file in result.Files.Where(f => f.Path.EndsWith(".json")))
            {
                await using var fs = File.OpenRead(file.Path);
                // Set -> Key -> Resource
                var resources = await JsonSerializer.DeserializeAsync<Dictionary<string, Dictionary<string, string>>>(fs) ?? throw new InvalidOperationException("Could not deserialize file");

                foreach (var item in resources.Where(r => r.Key.Contains("Server")))
                {
                    var culture = Path.GetFileNameWithoutExtension(file.Path);
                    var outputFile = $"{MakeValidFileName(item.Key)}.{culture}";
                    var outputFullPath = Path.Combine(options.OutputDirectory, Path.ChangeExtension(outputFile, "resx"));
                    using var resx = new ResXResourceWriter(outputFullPath);
                    foreach (var (key, value) in item.Value)
                    {
                        resx.AddResource(key, value);
                    }
                }
            }

            Environment.Exit(0);
        }
    }
}