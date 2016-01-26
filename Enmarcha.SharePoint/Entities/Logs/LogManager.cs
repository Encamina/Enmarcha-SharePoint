using Enmarcha.SharePoint.Abstract.Interfaces;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;

namespace Enmarcha.SharePoint.Class.Logs
{
    public class LogManager
    {
        #region Properties

        #region Private Static

        private LoggingService _loggingService;

        #endregion

        #endregion

        public ILog GetLogger(System.Diagnostics.StackFrame currentStackFrame)
        {
            _loggingService = new LoggingService(currentStackFrame);
            return _loggingService;
        }
    }
}
