using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResX_convertor
{
    public class ActionInputs
    {
        string _repositoryName = null!;
        string _branchName = null!;

        public ActionInputs()
        {
            if (Environment.GetEnvironmentVariable("GREETINGS") is { Length: > 0 } greetings)
            {
                Console.WriteLine(greetings);
            }
        }

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