using System;
using System.ComponentModel;
using System.Windows;
using NLog;
using LogManager = NLog.LogManager;

namespace DesktopOrganizer.Shell
{
    public partial class ShellView : IShellView
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly WindowStyle default_window_style;

        public bool IsExiting { get; set; }

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
            logger.Trace("State changed: " + WindowState);

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
            var vm = DataContext as Framework.Core.IShell;
            if (vm == null) return;

            if (IsExiting)
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
