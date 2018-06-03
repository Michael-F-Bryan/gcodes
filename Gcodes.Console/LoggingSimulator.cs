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
    public class LoggingEmulator: Emulator
    {
        ILogger logger;

        public LoggingEmulator()
        {
            logger = Log.ForContext<LoggingEmulator>();
        }

        public override void Visit(Gcode code)
        {
            logger.Debug("Executing a gcode, {@Code}", code);
            base.Visit(code);
        }

        protected override void CommentDetected(CommentEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Comment))
            {
                logger.Debug("Comment: {Comment}", e.Comment);
            }
        }
    }
}
