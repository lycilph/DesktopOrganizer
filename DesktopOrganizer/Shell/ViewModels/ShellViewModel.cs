using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using System.ComponentModel.Composition;
using Core;
using DesktopOrganizer.About;
using DesktopOrganizer.Data;
using DesktopOrganizer.Dialogs;
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
        private readonly SettingsViewModel settings_view_model;
        private readonly Stack<ViewModelBase> items;

        private ReactiveList<IWindowCommand> _ShellCommands = new ReactiveList<IWindowCommand>();
        public ReactiveList<IWindowCommand> ShellCommands
        {
            get { return _ShellCommands; }
            private set { this.RaiseAndSetIfChanged(ref _ShellCommands, value); }
        }

        public bool Exiting { get; private set; }

        [ImportingConstructor]
        public ShellViewModel()
        {
            settings_view_model = IoC.Get<SettingsViewModel>();
            items = new Stack<ViewModelBase>();

            ShellCommands.Add(new WindowCommand("Settings", () => Show(settings_view_model)));
            ShellCommands.Add(new WindowCommand("About", ShowAbout));
            ShellCommands.Add(new WindowCommand("Quit", Exit));

            //Show(new MainViewModel(this, application_settings));
        }

        private static async void ShowAbout()
        {
            await DialogController.ShowAsync(new AboutViewModel(), false);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "Desktop Organizer" + (Advapi32.IsRunningAsAdministrator(Process.GetCurrentProcess()) ? " (Administrator)" : string.Empty);
        }

        public void Exit()
        {
            Exiting = true;
            TryClose();
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
