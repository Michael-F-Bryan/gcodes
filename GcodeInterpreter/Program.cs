using CommandLine;
using CommandLine.Text;
using Gcodes;
using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcodeInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
                .MapResult(opts => Run(opts), _ => 1);
        }

        private static int Run(Options opts)
        {
            try
            {
                var src = File.ReadAllText(opts.InputFile);

                var lexer = new Lexer(src);
                var tokens = lexer.Tokenize().ToList();

                var parser = new Gcodes.Parser(tokens);
                var gcodes = parser.Parse().ToList();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return 1;
            }

            return 0;
        }
    }

    class Options
    {
        [Value(0, MetaName = "input file", HelpText = "The gcode file to interpret", Required = true)]
        public string InputFile { get; set; }
    }
}
