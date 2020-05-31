using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TeamsAppLib.Log
{
    public static class TraceManager
    {
        private static readonly string _traceSourceName = "TraceSourceTeamsApp";
        private static TraceSource _traceSource;

        public static void Init(SourceLevels filterLevel, string listenerKey, string filePath = null, string fileName = null)
        {
            _traceSource = new TraceSource(_traceSourceName);
            // If custom listener not configured in App.config, create a new one.
            if (_traceSource.Listeners[listenerKey] == null
                && !string.IsNullOrWhiteSpace(filePath)
                && !string.IsNullOrWhiteSpace(fileName))
            {
                var fileStream = new System.IO.FileStream(filePath + fileName, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                _traceSource.Listeners.Add(new TextWriterTraceListener(fileStream, listenerKey));
                _traceSource.Switch.Level = SourceLevels.All;
            }
            _traceSource.Listeners[listenerKey].Filter = new EventTypeFilter(filterLevel);
        }

        public static CodeSection GetCodeSection<T>(this T pthis, TraceEventType traceEventType = TraceEventType.Verbose, SourceLevels sourceLevel = SourceLevels.Verbose, object payload = null, string name = null, string category = null, string source = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new CodeSection(typeof(T), name, payload, _traceSource, traceEventType, sourceLevel, category, source, memberName, sourceFilePath, sourceLineNumber);
        }
        public static CodeSection GetCodeSection(Type t, TraceEventType traceEventType = TraceEventType.Verbose, SourceLevels sourceLevel = SourceLevels.Verbose, object payload = null, string name = null, string category = null, string source = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new CodeSection(t, name, payload, _traceSource, traceEventType, sourceLevel, category, source, memberName, sourceFilePath, sourceLineNumber);
        }
        public static CodeSection GetCodeSection<T>(TraceEventType traceEventType = TraceEventType.Verbose, SourceLevels sourceLevel = SourceLevels.Verbose, object payload = null, string name = null, string category = null, string source = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new CodeSection(typeof(T), name, payload, _traceSource, traceEventType, sourceLevel, category, source, memberName, sourceFilePath, sourceLineNumber);
        }
    }

    // TRACE ENTRY
    public class CodeSectionInfo
    {
        public object Payload { get; set; }
        public string Name { get; set; }
        public string MemberName { get; set; }
        public string SourceFilePath { get; set; }
        public int SourceLineNumber { get; set; }
        public long CallStartMilliseconds { get; set; }
        public DateTimeOffset? CallStart { get; set; }
        public DateTimeOffset? CallEnd { get; set; }
        public int NestingLevel { get; set; }
        public Type T { get; set; }
    }

    public class RequestInfo
    {
        public string RequestId { get; set; }
        public string RequestDescription { get; set; }
        public DateTimeOffset? RequestStart { get; set; }
        public DateTimeOffset? RequestEnd { get; set; }
    }

    public class ProcessInfo
    {
        public string ProcessID { get; set; }
        public string ProcessName { get; set; }
        public int ThreadID { get; set; }
        public Assembly Assembly { get; set; }
        public Process Process { get; set; }
        public Thread Thread { get; set; }
    }

    public class SystemInfo
    {
        public string Server { get; set; }
    }

    public enum TraceEntryType
    {
        Message,
        Exception,
        StartCall,
        EndCall
    }

    public struct TraceEntry
    {
        public TraceEntryType TraceEntryType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public SourceLevels SourceLevel { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Exception Exception { get; set; }

        public CodeSection CodeSection { get; set; }
        public RequestInfo RequestInfo { get; set; }
        public ProcessInfo ProcessInfo { get; set; }
        public SystemInfo SystemInfo { get; set; }

        public const int PROCESSNAME_PADDING = 15;
        public const int SOURCE_PADDING = 10;

        public override string ToString()
        {
            string line = null;
            string source = (this.Source != null && this.Source.Length < SOURCE_PADDING)
                ? this.Source.PadRight(SOURCE_PADDING)
                : this.Source;

            var processName = CodeSection.processName + ".exe";
            processName = (processName != null && processName.Length < PROCESSNAME_PADDING)
                ? processName.PadRight(PROCESSNAME_PADDING)
                : processName;

            var tidpid = string.Format("{0} {1} {2}", CodeSection.processId, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState());
            switch (this.TraceEntryType)
            {
                case TraceEntryType.Message:
                    line = string.Format("[{0}] {1} {2} {3} - {4} - {5}\r\n", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), processName, source, tidpid, this.SourceLevel, Message);
                    break;
                case TraceEntryType.StartCall:
                    line = string.Format("[{0}] {1} {2} {3} - {4} - {5}\r\n", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), processName, source, tidpid, this.SourceLevel, Message);
                    break;
                case TraceEntryType.EndCall:
                    var milliseconds = CodeSection.stopwatch.ElapsedMilliseconds - this.CodeSection.CallStartMilliseconds;
                    var delta = ((float)milliseconds / 1000).ToString("###0.00").PadLeft(7);
                    line = string.Format("[{0}] {1} {2} {3} - {4} - {5} - {6}\r\n", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), processName, source, tidpid, this.SourceLevel, delta, Message);
                    break;
            }
            return line;
        }
    }

    public class CodeSection : IDisposable
    {
        public static string processName = null;
        public static int processId = -1;
        public static Stopwatch stopwatch = new Stopwatch();

        public object Payload { get; set; }
        public string Name { get; set; }
        public string MemberName { get; set; }
        public string SourceFilePath { get; set; }
        public int sourceLineNumber { get; set; }
        public Type T { get; set; }
        public TraceSource traceSource;
        public TraceEventType traceEventType;
        public SourceLevels SourceLevel { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public long CallStartMilliseconds { get; set; }

        #region .ctor
        static CodeSection()
        {
            var process = Process.GetCurrentProcess();
            processName = process.ProcessName;
            processId = process.Id;
            stopwatch.Start();
        }

        public CodeSection(object pthis,
            string name = null,
            object payload = null,
            TraceSource traceSource = null,
            TraceEventType traceEventType = TraceEventType.Verbose,
            SourceLevels sourceLevel = SourceLevels.Verbose,
            string category = null,
            string source = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
            : this(pthis.GetType(), name, payload, traceSource, traceEventType, sourceLevel, category, source, memberName, sourceFilePath, sourceLineNumber)
        { }

        public CodeSection(Type type,
            string name = null,
            object payload = null,
            TraceSource traceSource = null,
            TraceEventType traceEventType = TraceEventType.Verbose,
            SourceLevels sourceLevel = SourceLevels.Verbose,
            string category = null,
            string source = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            this.Name = name;
            this.Payload = payload;
            this.traceSource = traceSource;
            this.traceEventType = traceEventType;
            this.SourceLevel = sourceLevel;
            this.MemberName = memberName;
            this.SourceFilePath = sourceFilePath;
            this.sourceLineNumber = sourceLineNumber;
            this.T = type;
            this.Category = category;

            if (string.IsNullOrEmpty(source))
            {
                source = type.Assembly.GetName().Name;
            }
            this.Source = source;
            this.CallStartMilliseconds = stopwatch.ElapsedMilliseconds;
            string section = !string.IsNullOrEmpty(this.Name) ? this.Name : null;

            if (traceSource.Switch.ShouldTrace(traceEventType))
            {
                var message = string.Format("{0}.{1}{2}({3}) START", !string.IsNullOrWhiteSpace(T.Name) ? T.Name : string.Empty, this.MemberName, section, this.Payload);
                var entry = new TraceEntry() { TraceEntryType = TraceEntryType.StartCall, Message = message, Source = source, Category = category, SourceLevel = sourceLevel, CodeSection = this };
                foreach (TraceListener listener in traceSource.Listeners)
                {
                    listener.WriteLine(entry);
                    listener.Flush();
                }
            }
        }
        #endregion

        public void Debug(string message, string category = null, string source = null)
        {
            if (traceSource.Switch.ShouldTrace(TraceEventType.Information))
            {
                var entry = new TraceEntry()
                {
                    TraceEntryType = TraceEntryType.Message,
                    Message = message,
                    Source = source ?? this.Source,
                    Category = category,
                    SourceLevel = SourceLevels.Information,
                    CodeSection = this
                };
                foreach (TraceListener listener in traceSource.Listeners)
                {
                    listener.WriteLine(entry);
                    listener.Flush();
                }
            }
        }

        public void Information(string message, string category = null, string source = null)
        {
            if (traceSource.Switch.ShouldTrace(TraceEventType.Information))
            {
                var entry = new TraceEntry() { TraceEntryType = TraceEntryType.Message, Message = message, Source = source ?? this.Source, Category = category, SourceLevel = SourceLevels.Information, CodeSection = this };
                foreach (TraceListener listener in traceSource.Listeners)
                {
                    listener.WriteLine(entry);
                    listener.Flush();
                }
            }
        }

        public void Warning(string message, string category = null, string source = null)
        {
            if (traceSource.Switch.ShouldTrace(TraceEventType.Warning))
            {
                var entry = new TraceEntry() { TraceEntryType = TraceEntryType.Message, Message = message, Source = source ?? this.Source, Category = category, SourceLevel = SourceLevels.Warning, CodeSection = this };
                foreach (TraceListener listener in traceSource.Listeners)
                {
                    listener.WriteLine(entry);
                    listener.Flush();
                }
            }
        }

        public void Error(string message, string category = null, string source = null)
        {
            if (traceSource.Switch.ShouldTrace(TraceEventType.Error))
            {
                var entry = new TraceEntry() { TraceEntryType = TraceEntryType.Message, Message = message, Source = source ?? this.Source, Category = category, SourceLevel = SourceLevels.Error, CodeSection = this };
                foreach (TraceListener listener in traceSource.Listeners)
                {
                    listener.WriteLine(entry);
                    listener.Flush();
                }
            }
        }

        public void Exception(Exception exception, string category = null, string source = null)
        {
            if (traceSource.Switch.ShouldTrace(TraceEventType.Critical))
            {
                var entry = new TraceEntry() { TraceEntryType = TraceEntryType.Message, Message = $"Exception: {exception?.ToString()}\nInnerException: {exception?.InnerException?.Message}\nStackTrace: {exception?.StackTrace}", Source = source ?? this.Source, Category = category, SourceLevel = SourceLevels.Critical, CodeSection = this };
                foreach (TraceListener listener in traceSource.Listeners)
                {
                    listener.WriteLine(entry);
                    listener.Flush();
                }
            }
        }

        public void Dispose()
        {
            string section = !string.IsNullOrEmpty(this.Name) ? string.Format(".{0}", this.Name) : null;
            if (traceSource.Switch.ShouldTrace(traceEventType))
            {
                var message = string.Format("{0}.{1}{2}() END", !string.IsNullOrWhiteSpace(T.Name) ? T.Name : string.Empty, this.MemberName, section);
                var entry = new TraceEntry() { TraceEntryType = TraceEntryType.EndCall, Message = message, Source = this.Source, Category = this.Category, SourceLevel = this.SourceLevel, CodeSection = this };
                foreach (TraceListener listener in traceSource.Listeners)
                {
                    listener.WriteLine(entry);
                    listener.Flush();
                }
            }
        }
    }
}