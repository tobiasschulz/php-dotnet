using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PHP.Library.Internal
{
    public interface IConsole
    {
        IConsoleOutput Out { get; }
        IConsoleOutput Err { get; }
        IConsoleInput In { get; }
    }

    public interface IConsoleOutput
    {
        void Flush ();
        void Write (string value);
        void WriteLine (string value);
    }

    public interface IConsoleInput
    {
        string ReadLine ();
    }

    public sealed class StandardConsole : IConsole
    {
        public IConsoleOutput Out { get; set; }
        public IConsoleOutput Err { get; set; }
        public IConsoleInput In { get; set; }

        public StandardConsole ()
        {
            Out = new StandardConsoleOutput (Console.Out);
            Err = new StandardConsoleOutput (Console.Error);
            In = new StandardConsoleInput (Console.In);
        }
    }

    public sealed class StandardConsoleOutput : IConsoleOutput
    {
        private readonly TextWriter _writer;

        public StandardConsoleOutput (TextWriter writer)
        {
            _writer = writer;
        }

        void IConsoleOutput.Flush ()
        {
            _writer.Flush ();
        }

        void IConsoleOutput.Write (string value)
        {
            _writer.Write (value);
        }

        void IConsoleOutput.WriteLine (string value)
        {
            _writer.WriteLine (value);
        }
    }

    public sealed class StandardConsoleInput : IConsoleInput
    {
        private readonly TextReader _reader;

        public StandardConsoleInput (TextReader reader)
        {
            _reader = reader;
        }

        string IConsoleInput.ReadLine ()
        {
            return _reader.ReadLine ();
        }

    }
}
