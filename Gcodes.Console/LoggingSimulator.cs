using Gcodes.Ast;
using Gcodes.Runtime;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Console
{
    public class LoggingEmulator : Emulator
    {
        ILogger logger;

        public LoggingEmulator()
        {
            logger = Log.ForContext<LoggingEmulator>();
        }

        public override void Visit(Gcode code)
        {
            var info = SpanInfoFor(code.Span);
            if (info == null)
            {
                logger.Debug("Executing a gcode, {@Code}", code);
            }
            else
            {
                logger.Debug("Executing {@Code} at L{Line},C{Column} => {@Info}", code, info.Start.Line, info.Start.Column, info);
            }

            base.Visit(code);
        }

        protected override void CommentDetected(CommentEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Comment))
            {
                var info = SpanInfoFor(e.Span);
                if (info == null)
                {
                    logger.Debug("Comment: {Comment}", e.Comment);
                }
                else
                {
                    logger.Debug("Comment: \"{Comment}\" at L{Line},C{Column}", e.Comment, info.Start.Line, info.Start.Column, info);
                }
            }
        }
    }
}
