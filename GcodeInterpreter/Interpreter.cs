using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gcodes;
using Gcodes.Ast;
using Serilog;

namespace GcodeInterpreter
{
    internal class Interpreter
    {
        string src;

        public Interpreter(string src)
        {
            this.src = src;
        }

        public void Run()
        {
            var lexer = new Lexer(src);
            Log.Debug("Tokenizing input data ({Length} chars)", src.Length);
            var tokens = lexer.Tokenize().ToList();
            Log.Debug("Got {NumTokens} tokens", tokens.Count);

            Log.Debug("Parsing");
            var parser = new Gcodes.Parser(tokens);
            var gcodes = parser.Parse().ToList();
            Log.Debug("Found {NumGcodes} instructions", gcodes.Count);

            foreach (var code in gcodes)
            {
                Execute(code);
            }
        }

        private void Execute(Gcode code)
        {
            var line = code.Span.LineNumber(src);
            var col = code.Span.ColumnNumber(src);

            Log.Debug("Executing G{Number} at (L{Line}, C{Column})", code.Number, line, col);
        }
    }
}