using NLog;
using NLog.Targets;

namespace DTBWebServer
{
    public static class NLog4Init
    {
        public static void LoadConfig()
        {
            var config = new NLog.Config.LoggingConfiguration();

            FileTarget logfile = new NLog.Targets.FileTarget("logfile") { FileName = AppContext.BaseDirectory + "Temp/NLogfile.log" };
            logfile.ArchiveAboveSize = 1 * 1024 * 1024;
            logfile.ArchiveNumbering = ArchiveNumberingMode.Rolling;
            logfile.ConcurrentWrites = true;
            logfile.MaxArchiveFiles = 1;
            logfile.KeepFileOpen = true;
            logfile.DeleteOldFileOnStartup = true;  
            
            config.AddTarget(logfile);
#if DEBUG
            ConsoleTarget logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);
            config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, logfile);
#else
            config.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, logfile);
#endif
            LogManager.Configuration = config;

            var logger = NLog.LogManager.Setup().LoadConfiguration(config).GetCurrentClassLogger();
            logger.Info("NLog4 Init");
        }
    }
}
