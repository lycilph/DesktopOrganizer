using System;
using System.Linq;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell.ViewModels;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _ExcludedProcesses;
        public string ExcludedProcesses
        {
            get { return _ExcludedProcesses; }
            set { this.RaiseAndSetIfChanged(ref _ExcludedProcesses, value); }
        }

        public SettingsViewModel(IShell shell, ApplicationSettings application_settings) : base(shell, application_settings) { }

        private void Initialize()
        {
            ExcludedProcesses = string.Join(", ", application_settings.ExcludedProcesses);            
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
        }

        public void Back()
        {
            shell.Back();
        }

        public void Reset()
        {
            application_settings.Reset();
            Initialize();
        }
    }
}
