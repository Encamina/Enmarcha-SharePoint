using System;
using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Microsoft.SharePoint.Administration;

namespace Enmarcha.SharePoint.Class.Logs
{
    internal sealed class LoggingService : SPDiagnosticsServiceBase, ILog
    {
        #region Level enum

        // Priority:
        // DEBUG < INFO < WARN < ERROR
        private enum LogLevels
        {
            debug,
            info,
            warn,
            error
        }

        #endregion

        #region Properties

        #region Private

        /// <summary>
        /// Local StackFrame with method info
        /// </summary>
        private readonly System.Diagnostics.StackFrame _stackFrame;

        private string _levelFileLog;

        private string LevelFileLog
        {
            get
            {
                return string.IsNullOrEmpty(_levelFileLog)
                    ? ApplicationDirectory.Logging.DefaultLevelFileLog
                    : _levelFileLog;
            }
            set { _levelFileLog = value; }
        }

        #endregion

        #region Static

        private static LoggingService _current;

        public static LoggingService Current
        {
            get
            {
                if
                    (_current == null)
                {
                    _current = new LoggingService();
                    
                }

                return _current;
            }
        }

        #endregion

        #endregion

        #region Ctors

        private LoggingService()
            : base("Sanquest Logging Service", SPFarm.Local)
        {
        }

        public LoggingService(System.Diagnostics.StackFrame stackFrame)
        {
            _stackFrame = stackFrame;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            var areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(ApplicationDirectory.Logging.DiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory(ApplicationDirectory.Logging.LogDebug, TraceSeverity.VerboseEx,
                        EventSeverity.Verbose),
                    new SPDiagnosticsCategory(ApplicationDirectory.Logging.LogInfo, TraceSeverity.Verbose,
                        EventSeverity.Information),
                    new SPDiagnosticsCategory(ApplicationDirectory.Logging.LogWarn, TraceSeverity.Unexpected,
                        EventSeverity.Warning),
                    new SPDiagnosticsCategory(ApplicationDirectory.Logging.LogError, TraceSeverity.High,
                        EventSeverity.Error)
                })
            };

            return areas;
        }

        #endregion

        #region ILog Interface implementation

        public void Debug(string message)
        {
            var debug =
                Current.Areas[ApplicationDirectory.Logging.DiagnosticAreaName].Categories[
                    ApplicationDirectory.Logging.LogDebug];

            DoWriteTrace(debug, message);
        }

        public void Debug(string message, params object[] args)
        {
            Debug(string.Format(message, args));
        }

        public void Info(string message)
        {
            var info =
                Current.Areas[ApplicationDirectory.Logging.DiagnosticAreaName].Categories[
                    ApplicationDirectory.Logging.LogInfo];

            DoWriteTrace(info, message);
        }

        public void Info(string message, params object[] args)
        {
            Info(string.Format(message, args));
        }

        public void Warn(string message)
        {
            var warn =
                Current.Areas[ApplicationDirectory.Logging.DiagnosticAreaName].Categories[
                    ApplicationDirectory.Logging.LogWarn];

            DoWriteTrace(warn, message);
        }

        public void Warn(string message, params object[] args)
        {
            Warn(string.Format(message, args));
        }

        public void Error(string message)
        {
            Error(message, null, string.Empty);
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(string message, Exception exception)
        {
            var error =
                Current.Areas[ApplicationDirectory.Logging.DiagnosticAreaName].Categories[
                    ApplicationDirectory.Logging.LogError];

            DoWriteTrace(error, message, exception);
        }

        public void Error(string message, Exception exception, params object[] args)
        {
            Error(string.Format(message, args), exception);
        }

        #endregion

        #region Methods Private

       

        private void DoWriteTrace(SPDiagnosticsCategory category, string message)
        {
            DoWriteTrace(category, message, null);
        }

        private void DoWriteTrace(SPDiagnosticsCategory category, string message, Exception exception)
        {
            // Si el level log que vamos a escribir es mayor o igual que el configurado en el web.config, escribe en el destino

            if (LevelFileLog.ToLower().Equals(ApplicationDirectory.Logging.Disabled))
            {
                return;
            }
            if (GetLoggingLevel(GetLoggingLevelByCategory(category.Name)) >= GetLoggingLevel(LevelFileLog))
                Current.WriteTrace(0, category, category.TraceSeverity, ComposeLogMessage(message, exception), null);
        }

        /// <summary>
        /// Composes log message text
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private string ComposeLogMessage(string message, Exception exception)
        {
            var msg = message ?? string.Empty;

            var methodCaller = _stackFrame.GetMethod();

            if (methodCaller.DeclaringType != null)
                msg = string.Format("[{0}.{1}()]::{2}{3}", methodCaller.DeclaringType.FullName,
                    methodCaller.Name,
                    msg,
                    exception != null ? "::Exception=" + exception.Message : string.Empty);

            return msg;
        }

        /// <summary>
        /// Returns log level value for given log type
        /// </summary>
        /// <param name="loggingType"></param>
        /// <returns></returns>
        private static int GetLoggingLevel(string loggingType)
        {
            return (int)Enum.Parse(typeof(LogLevels), loggingType);
        }

        /// <summary>
        /// Returns LogLevel by category title
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private static string GetLoggingLevelByCategory(string category)
        {
            return category.Replace("[", string.Empty).Replace("]", string.Empty).ToLower();
        }

        #endregion
     
    }
}
