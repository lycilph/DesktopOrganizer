using System.Collections.Generic;
using Caliburn.Micro.ReactiveUI;
using System.ComponentModel.Composition;
using DesktopOrganizer.Data;
using DesktopOrganizer.Main;
using DesktopOrganizer.Settings;
using DesktopOrganizer.Shell.Utils;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<ViewModelBase>, IShell
    {
        private readonly ApplicationSettings application_settings;
        private readonly SettingsViewModel settings_view_model;
        private readonly Stack<ViewModelBase> items;

        private ReactiveList<IWindowCommand> _ShellCommands = new ReactiveList<IWindowCommand>();
        public ReactiveList<IWindowCommand> ShellCommands
        {
            get { return _ShellCommands; }
            private set { this.RaiseAndSetIfChanged(ref _ShellCommands, value); }
        }

        public ShellViewModel()
        {
            application_settings = ApplicationSettings.Load();
            settings_view_model = new SettingsViewModel(this, application_settings);
            items = new Stack<ViewModelBase>();

            ShellCommands.Add(new WindowCommand("Settings", () => Show(settings_view_model)));

            Show(new MainViewModel(this, application_settings));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "Desktop Organizer";
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                ApplicationSettings.Save(application_settings);
        }

        public void Back()
        {
            items.Pop();
            ActivateItem(items.Peek());
        }

        public void Show(ViewModelBase view_model)
        {
            items.Push(view_model);
            ActivateItem(view_model);
        }
    }
}
