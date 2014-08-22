using System;
using System.Diagnostics;
using System.Windows.Input;

namespace DesktopOrganizer.Utils
{
    public class RelayCommand : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> can_execute;

        public RelayCommand(Action<object> execute) : this(execute, null) {}
        public RelayCommand(Action<object> execute, Predicate<object> can_execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.can_execute = can_execute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return can_execute == null || can_execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
