using CommandLine;
using JetBrains.Annotations;
namespace MyBlueprint.ResxConverter
{
    public class ActionInputs
    {
        [Option('i', "input",
            Required = true,
            HelpText = "Input glob of the JSON files to convert"), PublicAPI]
        public string[] Input { get; set; } = [];

        [Option('o', "output",
             Required = true,
             HelpText = "Directory where the output resource files should be saved."), PublicAPI]
        public string OutputDirectory { get; set; } = string.Empty;
    }
}