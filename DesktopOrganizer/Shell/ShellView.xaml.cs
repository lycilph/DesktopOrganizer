using System;
using System.ComponentModel;
using System.Windows;

namespace DesktopOrganizer.Shell
{
    public partial class ShellView
    {
        private readonly WindowStyle default_window_style;

        public ShellView()
        {
            InitializeComponent();

            default_window_style = WindowStyle;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
#if !DEBUG
            WindowState = WindowState.Minimized;
#endif
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                TaskbarIcon.Visibility = Visibility.Visible;
                WindowStyle = WindowStyle.ToolWindow;
            }
            else
            {
                ShowInTaskbar = true;
                TaskbarIcon.Visibility = Visibility.Hidden;
                WindowStyle = default_window_style;
            }
        }

        private void OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;            
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
#if !DEBUG
            var vm = DataContext as IShell;
            if (vm == null) return;

            if (vm.Exiting)
            {
                TaskbarIcon.Visibility = Visibility.Hidden;
            }
            else
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
            }
#endif
        }
    }
}
