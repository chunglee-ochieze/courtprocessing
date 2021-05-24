using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CourtProcessing.Data
{
    public static class LogHandler
    {
        public static void WriteLog(string mssg, LogEventLevel logType, IConfiguration _config, [CallerMemberName] string cmn = null)
        {
            try
            {
                var path = Path.Combine(_config["Logging:Path"], $"Log_{logType}\\{logType.ToString().ToUpper()}-{DateTime.Now:yy-MM-dd}.txt");

                var logMssg = $"\r\nMethod: {cmn}\r\nMessage: {mssg}\r\n_______________________________________";

                using var log = new LoggerConfiguration()
                    .WriteTo.File(path, shared: true, rollOnFileSizeLimit: true, fileSizeLimitBytes: _config.GetValue<int>("Logging:SizeMb") * 1048576)
                    .CreateLogger();

                switch (logType)
                {
                    case var n when n.Equals(LogEventLevel.Information):
                        log.Information(logMssg);
                        break;
                    case var n when n.Equals(LogEventLevel.Warning):
                        log.Warning(logMssg);
                        break;
                    case var n when n.Equals(LogEventLevel.Error):
                        log.Error(logMssg);
                        break;
                    case var n when n.Equals(LogEventLevel.Fatal):
                        log.Fatal(logMssg);
                        break;
                    case var n when n.Equals(LogEventLevel.Verbose):
                        log.Verbose(logMssg);
                        break;
                    case var n when n.Equals(LogEventLevel.Debug):
                        log.Debug(logMssg);
                        break;
                    default:
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            var eventLog = new EventLog()
                            {
                                Source = "CourtProcessing"
                            };
                            eventLog.WriteEntry(logMssg, EventLogEntryType.Information);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var eventLog = new EventLog()
                    {
                        Source = "CourtProcessing"
                    };
                    eventLog.WriteEntry("Error in: WriteLog.");
                    eventLog.WriteEntry($"\r\nMethod: {cmn}\r\nMessage: {mssg}\r\n_______________________________________\r\n");
                    eventLog.WriteEntry($"Exception encountered in {cmn}. Error: {ex.Message}\r\nDetail: {ex.StackTrace}", EventLogEntryType.Error);
                }
            }
        }
    }
}
