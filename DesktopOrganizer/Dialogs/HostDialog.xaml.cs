using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace DesktopOrganizer.Dialogs
{
    public partial class HostDialog
    {
        private readonly TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();

        public Task<MessageDialogResult> Task
        {
            get { return tcs.Task; }
        }

        public HostDialog(bool show_cancel)
        {
            InitializeComponent();

            if (!show_cancel)
                cancel_button.Visibility = Visibility.Collapsed;
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
