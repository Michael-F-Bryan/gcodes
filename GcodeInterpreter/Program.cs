using CommandLine;
using CommandLine.Text;
using Gcodes;
using Gcodes.Runtime;
using Gcodes.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
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
        static int Main(string[] args)
        {
            var parsedArgs = CommandLine.Parser.Default.ParseArguments<Options>(args);

            return parsedArgs.MapResult(opts => Run(opts), _ => 1);
        }

        private static void Initializelogger(Options opts)
        {
            var minLevel = opts.Verbose ? LogEventLevel.Debug : LogEventLevel.Information;

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                    .MinimumLevel.Is(minLevel)
                  .WriteTo.Console()
                .CreateLogger();
        }

        private static int Run(Options opts)
        {
            Initializelogger(opts);

            try
            {
                Log.Debug("Reading {Filename}", opts.InputFile);
                var src = File.ReadAllText(opts.InputFile);

                var vm = new Emulator();
                vm.Run(src);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Debug(ex, "An error occurred");
                return 1;
            }

            return 0;
        }
    }

    class Options
    {
        [Value(0, MetaName = "input file", HelpText = "The gcode file to interpret", Required = true)]
        public string InputFile { get; internal set; }
        [Option(SetName = "verbose", Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; internal set; }
    }
}
