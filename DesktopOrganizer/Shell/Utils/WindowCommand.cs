using System;
using ReactiveUI;

namespace DesktopOrganizer.Shell.Utils
{
    public class WindowCommand : ReactiveObject, IWindowCommand
    {
        private readonly Action action;

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        public WindowCommand(string name, Action action)
        {
            this.action = action;
            DisplayName = name;
        }

        public virtual void Execute()
        {
            action();
        }
    }
}
