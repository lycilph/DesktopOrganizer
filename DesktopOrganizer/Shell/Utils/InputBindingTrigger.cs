using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DesktopOrganizer.Shell.Utils
{
    public class InputBindingTrigger : TriggerBase<FrameworkElement>, ICommand
    {
        public static DependencyProperty InputBindingProperty =
            DependencyProperty.Register("InputBinding", typeof(InputBinding), typeof(InputBindingTrigger), new UIPropertyMetadata());

        public InputBinding InputBinding
        {
            get { return (InputBinding)GetValue(InputBindingProperty); }
            set { SetValue(InputBindingProperty, value); }
        }

        protected override void OnAttached()
        {
            if (InputBinding != null)
            {
                InputBinding.Command = this;
                AssociatedObject.InputBindings.Add(InputBinding);
            }

            base.OnAttached();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            InvokeActions(parameter);
        }
    }
}
