using CommandLine;
using System;
using System.Collections.Generic;
namespace MyBlueprint.ResxConverter
{
    public class ActionInputs
    {
        [Option('d', "dir",
            Required = true,
            HelpText = "Input Csv file Directory that you want to convert.")]
        public string InputDirectory { get; set; } = null!;

        [Option('d', "dir",
             Required = true,
             HelpText = "Output ResX file Directory.")]
        public string OutputDirectory { get; set; } = null!;
    }

}