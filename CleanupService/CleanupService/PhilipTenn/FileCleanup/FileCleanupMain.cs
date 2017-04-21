using System.ServiceProcess;
using log4net;

namespace PhilipTenn.FileCleanup
{
    /// <summary>
    /// This application is used to run the AF FileCleanupService as a Windows Service.  This class is merely
    /// used to instantiate and run the FileCleanupService.  

    /// </summary>
    static class FileCleanupMain
    {
        private static ILog logger = LogManager.GetLogger(typeof(FileCleanupMain));

        /// <summary>
        /// FileCleanup main application entry point.
        /// </summary>
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Info("Execute Main entry point for FileCleanup.  Will create Windows Service");

            // Instantiate the CleanupService and tell the Windows Service Controller to "run" it.
            // Depending on the startup type of this service (i.e., manual or automatic), the service may initially be stopped.
            CleanupService svcMgr = new CleanupService();
            ServiceBase.Run(svcMgr);
        }
    }
}