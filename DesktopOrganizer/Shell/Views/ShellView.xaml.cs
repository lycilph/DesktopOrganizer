using System;
using System.ComponentModel;
using System.Windows;
using DesktopOrganizer.Shell.ViewModels;

namespace DesktopOrganizer.Shell.Views
{
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //WindowState = WindowState.Minimized;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                TaskbarIcon.Visibility = Visibility.Visible;
            }
            else
            {
                ShowInTaskbar = true;
                TaskbarIcon.Visibility = Visibility.Hidden;
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
        }
    }
}
