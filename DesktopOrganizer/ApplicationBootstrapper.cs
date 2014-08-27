using Caliburn.Micro;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell;
using DesktopOrganizer.Shell.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using NLog;
using LogManager = NLog.LogManager;

namespace DesktopOrganizer
{
    public class ApplicationBootstrapper : BootstrapperBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private CompositionContainer container;

        static ApplicationBootstrapper()
        {
            Caliburn.Micro.LogManager.GetLog = type => new DebugLog(type);
        }

        public ApplicationBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            logger.Trace("Configure");

            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(ApplicationSettings.Load());
            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract).ToList();

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            logger.Trace("Startup");

            var startup_tasks = GetAllInstances(typeof(StartupTask))
                                .Cast<ExportedDelegate>()
                                .Select(exportedDelegate => (StartupTask)exportedDelegate.CreateDelegate(typeof(StartupTask)));
            startup_tasks.Apply(s => s());

            DisplayRootViewFor<IShell>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            logger.Trace("Exit");

            base.OnExit(sender, e);

            var settings = container.GetExportedValue<ApplicationSettings>();
            settings.Save();
        }
    }
}
