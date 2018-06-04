using Gcodes.Runtime;
using Serilog;

namespace Gcodes.Console
{
    /// <summary>
    /// An emulator which hooks into the various events provided by an 
    /// <see cref="Emulator"/> and logs the event arguments.
    /// </summary>
    public class LoggingEmulator : Emulator
    {
        ILogger logger;

        public LoggingEmulator()
        {
            logger = Log.ForContext<LoggingEmulator>();

            OperationExecuted += LoggingEmulator_OperationExecuted;
            StateChanged += LoggingEmulator_StateChanged;
            CommentDetected += LoggingEmulator_CommentDetected;
        }

        private void LoggingEmulator_StateChanged(object sender, StateChangeEventArgs e)
        {
            logger.Debug("State changed to {@State} (t: {Time})", e.NewState, e.Time);
        }

        private void LoggingEmulator_OperationExecuted(object sender, OperationExecutedEventArgs e)
        {
            if (e.Operation is Noop nop && nop.Duration.TotalMilliseconds == 0.0)
            {
                logger.Debug("Ignoring {$Code}", e.Code);
            }
            else
            {
                logger.Debug("Executing {@Code} with {@Operation}", e.Code, e.Operation);
            }
        }

        private void LoggingEmulator_CommentDetected(object sender, CommentEventArgs e)
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
