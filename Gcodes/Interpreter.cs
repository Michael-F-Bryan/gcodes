using Gcodes.Ast;
using Gcodes.Runtime;
using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes
{
    /// <summary>
    /// <para>
    /// A gcode interpreter which handles the tokenizing and parsing of a gcode
    /// program and will then visit each parsed gcode.
    /// </para> 
    /// <para>
    /// Callbacks are fired at each step of the process, allowing subclasses 
    /// to hook into the interpreting process.
    /// </para>
    /// </summary>
    public class Interpreter : IGcodeVisitor
    {
        private bool running = false;

        /// <summary>
        /// Tokenize, parse, then execute a gcode program.
        /// </summary>
        /// <param name="src"></param>
        public void Run(string src)
        {
            var lexer = new Lexer(src);
            lexer.CommentDetected += (s, e) => CommentDetected(e);

            var tokens = lexer.Tokenize().ToList();
            Run(tokens);
        }

        /// <summary>
        /// Parse a stream of <see cref="Token"/>s and execute them.
        /// </summary>
        /// <param name="tokens"></param>
        public void Run(List<Token> tokens)
        {
            BeforeParse(tokens);
            var parser = new Parser(tokens);
            var codes = parser.Parse().ToList();
            Run(codes);
        }

        /// <summary>
        /// Start executing a stream of pre-parsed gcodes.
        /// </summary>
        /// <param name="codes"></param>
        public void Run(List<Code> codes)
        {
            BeforeRun(codes);
            running = true;

            try
            {
                foreach (var code in codes)
                {
                    if (!running) break;
                    code.Accept(this);
                }
            }
            finally
            {
                running = false;
            }
        }

        /// <summary>
        /// Tell the interpreter to stop executing gcodes.
        /// </summary>
        public void Halt()
        {
            running = false;
        }

        public virtual void Visit(Gcode code) { }
        public virtual void Visit(Mcode code) { }
        public virtual void Visit(Tcode tcode) { }
        public virtual void Visit(Ocode code) { }

        /// <summary>
        /// Callback fired before the token stream is parsed.
        /// </summary>
        /// <param name="tokens"></param>
        protected virtual void BeforeParse(List<Token> tokens) { }
        /// <summary>
        /// Callback fired immediately before the interpreter starts executing
        /// gcodes.
        /// </summary>
        /// <param name="codes"></param>
        protected virtual void BeforeRun(List<Code> codes) { }
        /// <summary>
        /// Callback fired whenever a comment is encountered during the 
        /// tokenizing process.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void CommentDetected(CommentEventArgs e) { }
    }
}
