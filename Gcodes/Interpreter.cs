using Gcodes.Ast;
using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private FileMap map;

        /// <summary>
        /// Callback fired whenever a comment is encountered during the 
        /// tokenizing process.
        /// </summary>
        public event EventHandler<CommentEventArgs> CommentDetected;
        /// <summary>
        /// Callback fired immediately before the interpreter starts executing
        /// gcodes.
        /// </summary>
        /// <param name="codes"></param>
        public event EventHandler<List<Code>> BeforeRun;
        /// <summary>
        /// Callback fired before the token stream is parsed.
        /// </summary>
        /// <param name="tokens"></param>
        public event EventHandler<List<Token>> BeforeParse;

        /// <summary>
        /// <para>
        /// Tokenize, parse, then execute a gcode program. 
        /// </para>
        /// <para>
        /// This will also populate an internal <see cref="FileMap"/>, giving
        /// you access to line and span info.        
        /// </para>
        /// </summary>
        /// <param name="src"></param>
        public void Run(string src)
        {
            map = new FileMap(src);
            var lexer = new Lexer(src);
            lexer.CommentDetected += OnCommentDetected;

            var tokens = lexer.Tokenize().ToList();
            Run(tokens);
        }

        /// <summary>
        /// Parse a stream of <see cref="Token"/>s and execute them.
        /// </summary>
        /// <param name="tokens"></param>
        public void Run(List<Token> tokens)
        {
            OnBeforeParse(tokens);
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
            OnBeforeRun(codes);
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

        /// <summary>
        /// If interpreting from source, get the <see cref="SpanInfo"/> for a
        /// particular <see cref="Span"/>.
        /// </summary>
        /// <param name="span"></param>
        /// <returns>
        /// Span information, or <c>null</c> if not interpeting from source.
        /// </returns>
        protected SpanInfo SpanInfoFor(Span span)
        {
            return map?.SpanInfoFor(span);
        }

        /// <summary>
        /// If interpreting from source, get the <see cref="Location"/> for a
        /// particular byte index into the source text.
        /// </summary>
        /// <param name="byteIndex"></param>
        /// <returns>
        /// The corresponding <see cref="Location"/>, or <c>null</c> if not 
        /// interpreting from source.
        /// </returns>
        protected Location LocationFor(int byteIndex)
        {
            return map?.LocationFor(byteIndex);
        }

        public virtual void Visit(Gcode code) { }
        public virtual void Visit(Mcode code) { }
        public virtual void Visit(Tcode tcode) { }
        public virtual void Visit(Ocode code) { }

        private void OnBeforeParse(List<Token> tokens) {
            BeforeParse?.Invoke(this, tokens);
        }

        private void OnBeforeRun(List<Code> codes)
        {
            BeforeRun?.Invoke(this, codes);
        }

        private void OnCommentDetected(object sender, CommentEventArgs e)
        {
            CommentDetected?.Invoke(this, e);
        }
    }
}
