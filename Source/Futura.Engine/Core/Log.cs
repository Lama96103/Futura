using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Futura.Engine.Core
{
    public enum LogLevel
    {
        Debug, Trace, Info, Warning, Error
    }

    public sealed class Logger : SubSystem
    {
        private const string FilePath = "Futura.log";

        private readonly object ThreadLock = new object();
        private List<LogMessage> logMessages = new List<LogMessage>();
        private double timeSinceLastWrite = 0.0;
        private bool IsWritingToFile = false;



        public Logger()
        {
            Log.Init(this);
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }

        internal override void Tick(double deltaTime)
        {
            timeSinceLastWrite += deltaTime;

            if(timeSinceLastWrite >= 1000 && IsWritingToFile == false)
            {
                timeSinceLastWrite = 0.0;
                ThreadController.CreateTask(() => WriteLogMessagesToFile(), "WriteLogFile");
            }
        }


        internal void AddMessage(LogMessage message)
        {
            lock (ThreadLock)
            {
                logMessages.Add(message);
            }
        }

        private void WriteLogMessagesToFile()
        {
            IsWritingToFile = true;
            LogMessage[] messages;
            lock (ThreadLock)
            {
                messages = logMessages.ToArray();
                logMessages.Clear();
            }

            StringBuilder builder = new StringBuilder();
            foreach (LogMessage m in messages)
            {
                builder.AppendLine($"{m.Timestamp.ToString("HH:mm:ss:fff")} {m.File} {m.Method} {m.Line} {m.Level.ToString()}>{m.Message}");
                if (m.Exception != null)
                    builder.AppendLine(m.Exception.ToString());       
            }

            File.AppendAllText(FilePath, builder.ToString(), Encoding.UTF8);
            IsWritingToFile = false;
        }

        internal class LogMessage
        {
            public string Message { get; set; }
            public string File { get; set; }
            public string Method { get; set; }
            public int Line { get; set; }
            public LogLevel Level { get; set; }
            public DateTime Timestamp { get; set; }
            public Exception Exception { get; set; }
        }
    }

    public static class Log
    {
        private static Logger logger;
        public static LogLevel Level { get; set; } = LogLevel.Debug;

        internal static void Init(Logger logger)
        {
            Log.logger = logger;
        }

        public static void Debug(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Debug)
                logger.AddMessage(new Logger.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Debug,
                    Timestamp = DateTime.Now
                });
        }

        public static void Trace(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Trace)
                logger.AddMessage(new Logger.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Trace,
                    Timestamp = DateTime.Now
                });
        }

        public static void Info(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Info)
                logger.AddMessage(new Logger.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Info,
                    Timestamp = DateTime.Now
                });
        }

        public static void Warn(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Warning)
                logger.AddMessage(new Logger.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Warning,
                    Timestamp = DateTime.Now
                });
        }

        public static void Error(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Error)
                logger.AddMessage(new Logger.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Error,
                    Timestamp = DateTime.Now
                });
        }

        public static void Error(object message, Exception e, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Error)
                logger.AddMessage(new Logger.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Error,
                    Timestamp = DateTime.Now,
                    Exception = e
                });
        }
    }
}
