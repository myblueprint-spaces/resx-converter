using CommandLine;
using System;
using System.Collections.Generic;
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
    }

}