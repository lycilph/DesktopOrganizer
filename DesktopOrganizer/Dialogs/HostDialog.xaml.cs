using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace DesktopOrganizer.Dialogs
{
    public partial class HostDialog
    {
        private readonly TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();

        public Task<MessageDialogResult> Task { get { return tcs.Task; } }

        public Action CloseCallback { get { return () => tcs.SetResult(MessageDialogResult.Affirmative); } }

        public HostDialog(DialogButtonOptions options)
        {
            InitializeComponent();

            switch (options)
            {
                case DialogButtonOptions.None:
                    button_panel.Visibility = Visibility.Collapsed;
                    break;
                case DialogButtonOptions.Ok:
                    cancel_button.Visibility = Visibility.Collapsed;
                    break;
                case DialogButtonOptions.OkAndCancel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("options");
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            tcs.SetResult(MessageDialogResult.Affirmative);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tcs.SetResult(MessageDialogResult.Negative);
        }
    }
}
