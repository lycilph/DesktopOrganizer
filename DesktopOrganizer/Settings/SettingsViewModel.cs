using System.ComponentModel.Composition;
using System;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell;
using Framework.Window;
using MahApps.Metro.Controls;
using NLog;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace DesktopOrganizer.Settings
{
    [Export("Settings", typeof(IFlyout))]
    public class SettingsViewModel : FlyoutBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ApplicationSettings application_settings;
        private readonly IEventAggregator event_aggregator;

        private string _Processes;
        public string Processes
        {
            get { return _Processes; }
            set { this.RaiseAndSetIfChanged(ref _Processes, value); }
        }

        private bool _LaunchOnWindowsStart;
        public bool LaunchOnWindowsStart
        {
            get { return _LaunchOnWindowsStart; }
            set { this.RaiseAndSetIfChanged(ref _LaunchOnWindowsStart, value); }
        }

        [ImportingConstructor]
        public SettingsViewModel(IEventAggregator event_aggregator, ApplicationSettings application_settings) : base("Settings", Position.Right)
        {
            this.event_aggregator = event_aggregator;
            this.application_settings = application_settings;

            this.ObservableForProperty(x => x.IsOpen)
                .Where(x => x.Value)
                .Subscribe(x => ReadSettings());

            this.ObservableForProperty(x => x.IsOpen)
                .Where(x => !x.Value)
                .Subscribe(x => WriteSettings());
        }

        private void ReadSettings()
        {
            logger.Trace("Settings read");

            Processes = string.Join(", ", application_settings.ExcludedProcesses);
            LaunchOnWindowsStart = application_settings.LaunchOnWindowsStart;
        }

        private void WriteSettings()
        {
            logger.Trace("Settings written");

            application_settings.ExcludedProcesses = Processes.Split(new[] { "," }, StringSplitOptions.None).Select(s => s.Trim()).ToList();
            application_settings.LaunchOnWindowsStart = LaunchOnWindowsStart;
        }

        public void Reset()
        {
            application_settings.Reset();
            ReadSettings();
        }

        public void Exit()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ExitMessage());
        }
    }
}
