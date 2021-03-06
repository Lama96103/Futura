using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Futura.Engine.Core
{
    /// <summary>
    /// The level of the log message
    /// </summary>
    public enum LogLevel
    {
        Debug, Info, Warning, Error
    }

    /// <summary>
    /// Log system is for archiving and logging of log messages
    /// Every second the system will write the logs to the futura.log file
    /// </summary>
    public sealed class LogSystem : SubSystem
    {
        internal static event EventHandler<LogEventArgs> OnLogReceived;

        private const string FilePath = "Futura.log";

        private readonly object ThreadLock = new object();
        private List<LogMessage> logMessages = new List<LogMessage>();
        private double timeSinceLastWrite = 0.0;
        private bool IsWritingToFile = false;



        public LogSystem()
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
            OnLogReceived?.Invoke(this, new LogEventArgs(message));
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
}

namespace Futura
{
    public static class Log
    {
        private static LogSystem logger = null;
        public static LogLevel Level { get; set; } = LogLevel.Debug;



        internal static void Init(LogSystem logger)
        {
            if(Log.logger != null)
            {
                throw new Exception("Logger not Initilized");
            }
            Log.logger = logger;
        }

        /// <summary>
        /// Log a debug message
        /// You only need to add a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="path"></param>
        /// <param name="lineNumber"></param>
        /// <param name="memberName"></param>
        public static void Debug(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Debug)
            {
                logger.AddMessage(new LogSystem.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Debug,
                    Timestamp = DateTime.Now
                });
            }

        }

        /// <summary>
        /// Log a info message
        /// You only need to add a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="path"></param>
        /// <param name="lineNumber"></param>
        /// <param name="memberName"></param>
        public static void Info(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Info)
                logger.AddMessage(new LogSystem.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Info,
                    Timestamp = DateTime.Now
                });
        }

        /// <summary>
        /// Log a warning message
        /// You only need to add a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="path"></param>
        /// <param name="lineNumber"></param>
        /// <param name="memberName"></param>
        public static void Warn(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Warning)
                logger.AddMessage(new LogSystem.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Warning,
                    Timestamp = DateTime.Now
                });
        }

        /// <summary>
        /// Log an error message
        /// You only need to add a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="path"></param>
        /// <param name="lineNumber"></param>
        /// <param name="memberName"></param>
        public static void Error(object message, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Error)
                logger.AddMessage(new LogSystem.LogMessage
                {
                    Message = message.ToString(),
                    File = path,
                    Line = lineNumber,
                    Method = memberName,
                    Level = LogLevel.Error,
                    Timestamp = DateTime.Now
                });
        }

        /// <summary>
        /// Log an error message
        /// You only need to add a message and the exception you want to log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="path"></param>
        /// <param name="lineNumber"></param>
        /// <param name="memberName"></param>
        public static void Error(object message, Exception e, [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (Level <= LogLevel.Error)
                logger.AddMessage(new LogSystem.LogMessage
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


    internal class LogEventArgs : EventArgs
    {
        public LogSystem.LogMessage LogMsg { get; set; }

        internal LogEventArgs(LogSystem.LogMessage message)
        {
            LogMsg = message;
        }
    }
}
