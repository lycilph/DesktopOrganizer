using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Core;
using DesktopOrganizer.About;
using DesktopOrganizer.Dialogs;
using DesktopOrganizer.Main;
using DesktopOrganizer.Settings;
using DesktopOrganizer.Shell.Utils;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.Shell
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<ViewModelBase>, IShell, IHandle<ShellMessage>
    {
        private readonly Stack<ViewModelBase> items;

        private ReactiveList<IWindowCommand> _ShellCommands = new ReactiveList<IWindowCommand>();
        public ReactiveList<IWindowCommand> ShellCommands
        {
            get { return _ShellCommands; }
            private set { this.RaiseAndSetIfChanged(ref _ShellCommands, value); }
        }

        public bool Exiting { get; private set; }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator event_aggregator)
        {
            event_aggregator.Subscribe(this);

            ShellCommands.Add(new WindowCommand("Settings", () => Show(IoC.Get<SettingsViewModel>())));
            ShellCommands.Add(new WindowCommand("About", async () => await DialogController.ShowAsync(new AboutViewModel(), DialogButtonOptions.Ok)));
            ShellCommands.Add(new WindowCommand("Quit", Exit));

            items = new Stack<ViewModelBase>();
            Show(IoC.Get<MainViewModel>());
        }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "Desktop Organizer" + (Advapi32.IsRunningAsAdministrator(Process.GetCurrentProcess()) ? " (Administrator)" : string.Empty);
        }

        private void Back()
        {
            items.Pop();
            ActivateItem(items.Peek());
        }

        private void Show(ViewModelBase view_model)
        {
            items.Push(view_model);
            ActivateItem(view_model);
        }

        public void Exit()
        {
            Exiting = true;
            TryClose();
        }

        public void Handle(ShellMessage message)
        {
            switch (message.Kind)
            {
                case ShellMessage.MessageKind.Back:
                    Back();
                    break;
                case ShellMessage.MessageKind.Show:
                    Show(message.ViewModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
