using System;
using System.ComponentModel.Composition;
using System.Linq;
using DesktopOrganizer.Data;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.Settings
{
    [Export(typeof(SettingsViewModel))]
    public class SettingsViewModel : ViewModelBase
    {
        private string _ExcludedProcesses;
        public string ExcludedProcesses
        {
            get { return _ExcludedProcesses; }
            set { this.RaiseAndSetIfChanged(ref _ExcludedProcesses, value); }
        }

        private bool _LaunchOnWindowsStart;
        public bool LaunchOnWindowsStart
        {
            get { return _LaunchOnWindowsStart; }
            set { this.RaiseAndSetIfChanged(ref _LaunchOnWindowsStart, value); }
        }

        [ImportingConstructor]
        public SettingsViewModel(ApplicationSettings application_settings) : base(application_settings) { }

        private void Initialize()
        {
            ExcludedProcesses = string.Join(", ", application_settings.ExcludedProcesses);
            LaunchOnWindowsStart = application_settings.LaunchOnWindowsStart;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Initialize();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            application_settings.ExcludedProcesses = ExcludedProcesses.Split(new[] { "," }, StringSplitOptions.None).Select(s => s.Trim()).ToList();
            application_settings.LaunchOnWindowsStart = LaunchOnWindowsStart;
        }

        public void Back()
        {
            //shell.Back();
        }

        public void Reset()
        {
            application_settings.Reset();
            Initialize();
        }
    }
}
