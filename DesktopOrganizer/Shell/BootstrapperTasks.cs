using System.ComponentModel.Composition;
using Caliburn.Micro;
using DesktopOrganizer.Data;
using Framework.Core;
using NLog;
using LogManager = NLog.LogManager;

namespace DesktopOrganizer.Shell
{
    public class BootstrapperTasks
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void LoadSettings()
        {
            logger.Trace("Loading settings");

            var settings = IoC.Get<ApplicationSettings>();
            settings.Load();
        }

        [Export(ApplicationBootstrapper.SHUTDOWN_TASK_NAME, typeof(BootstrapperTask))]
        public void SaveSettings()
        {
            logger.Trace("Saving settings");

            var settings = IoC.Get<ApplicationSettings>();
            settings.Save();
        }

    }
}
