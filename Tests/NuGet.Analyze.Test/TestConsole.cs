using System;
using System.Globalization;
using System.IO;
using System.Security;
using NuGet.Common;

namespace NuGet.Analyze.Test
{
    public class TestConsole : IConsole
    {
        public int CursorLeft
        {
            get
            {
                int result;
                try
                {
                    result = System.Console.CursorLeft;
                }
                catch (IOException)
                {
                    result = 0;
                }
                return result;
            }
            set
            {
                System.Console.CursorLeft = value;
            }
        }
        public int WindowWidth
        {
            get
            {
                int result;
                try
                {
                    result = System.Console.WindowWidth;
                }
                catch (IOException)
                {
                    result = 60;
                }
                return result;
            }
            set
            {
                System.Console.WindowWidth = value;
            }
        }

        public Verbosity Verbosity { get; set; }
        public bool IsNonInteractive { get; set; }

        public TextWriter ErrorWriter
        {
            get
            {
                return System.Console.Error;
            }
        }
        public void Write(object value)
        {
            System.Console.Write(value);
        }
        public void Write(string value)
        {
            System.Console.Write(value);
        }
        public void Write(string format, params object[] args)
        {
            System.Console.Write(format, args);
        }
        public void WriteLine()
        {
            System.Console.WriteLine();
        }
        public void WriteLine(object value)
        {
            System.Console.WriteLine(value);
        }
        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
        public void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        public void WriteLine(ConsoleColor color, string value, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void WriteError(object value)
        {
            this.WriteError(value.ToString());
        }
        public void WriteError(string value)
        {
            this.WriteError(value, new object[0]);
        }
        public void WriteError(string format, params object[] args)
        {
            TestConsole.WriteColor(System.Console.Error, System.ConsoleColor.Red, format, args);
        }
        public void WriteWarning(string value)
        {
            bool prependWarningText = true;
            object[] args = new object[0];
            this.WriteWarning(prependWarningText, value, args);
        }
        public void WriteWarning(bool prependWarningText, string value)
        {
            this.WriteWarning(prependWarningText, value, new object[0]);
        }
        public void WriteWarning(string value, params object[] args)
        {
            bool prependWarningText = true;
            this.WriteWarning(prependWarningText, value, args);
        }
        public void WriteWarning(bool prependWarningText, string value, params object[] args)
        {
            string value2;
            if (prependWarningText)
            {
                value2 = string.Format(CultureInfo.CurrentCulture, "WARNING: {0}", new object[]
				{
					value
				});
            }
            else
            {
                value2 = value;
            }
            TestConsole.WriteColor(System.Console.Out, System.ConsoleColor.Yellow, value2, args);
        }
        private static void WriteColor(TextWriter writer, System.ConsoleColor color, string value, params object[] args)
        {
            System.ConsoleColor foregroundColor = System.Console.ForegroundColor;
            try
            {
                foregroundColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = color;
                writer.WriteLine(value, args);
            }
            finally
            {
                System.Console.ForegroundColor = foregroundColor;
            }
        }

        public void ReadSecureString(SecureString secureString)
        {
            throw new NotImplementedException();
        }

        public void PrintJustified(int startIndex, string text)
        {
            this.PrintJustified(startIndex, text, this.WindowWidth);
        }
        public void PrintJustified(int startIndex, string text, int maxWidth)
        {
            if (maxWidth > startIndex)
            {
                maxWidth = maxWidth - startIndex - 1;
            }
            while (text.Length > 0)
            {
                text = text.TrimStart(new char[0]);
                int num = Math.Min(text.Length, maxWidth);
                string text2 = text.Substring(0, num);
                int totalWidth = startIndex + num - this.CursorLeft;
                System.Console.WriteLine(text2.PadLeft(totalWidth));
                text = text.Substring(text2.Length);
            }
        }
        public bool Confirm(string description)
        {
            ConsoleColor foregroundColor = ConsoleColor.Gray;
            bool result;
            try
            {
                foregroundColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write(string.Format(CultureInfo.CurrentCulture, "{0} (y/N) ", new object[]
				{
					description
				}));
                string text = System.Console.ReadLine();
                result = text.StartsWith("y", StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                System.Console.ForegroundColor = foregroundColor;
            }
            return result;
        }

        public ConsoleKeyInfo ReadKey()
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case MessageLevel.Info:
                    {
                        this.WriteLine(message, args);
                        return;
                    }
                case MessageLevel.Warning:
                    {
                        this.WriteWarning(message, args);
                        return;
                    }
                case MessageLevel.Debug:
                    {
                        TestConsole.WriteColor(System.Console.Out, ConsoleColor.Gray, message, args);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public FileConflictResolution ResolveFileConflict(string message)
        {
            throw new NotImplementedException();
        }
    }
}