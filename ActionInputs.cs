using CommandLine;
namespace MyBlueprint.ResxConverter
{
    public class ActionInputs
    {
        [Option('i', "input-dir",
            Required = true,
            HelpText = "Directory path containing the CSV files you want to convert.")]
        public string InputDirectory { get; set; } = null!;

        [Option('o', "output-dir",
             Required = true,
             HelpText = "Directory where the output ResX files should be saved.")]
        public string OutputDirectory { get; set; } = null!;

        [Option('f', "input-fileName",
            Required = true,
            HelpText = "The name of the CSV file to convert.")]
        public string InputFileName { get; set; } = null!;

        [Option('v', "output-fileName",
           Required = true,
           HelpText = "The name of the ResX file to create.")]
        public string OutputFileName { get; set; } = null!;
    }

}