using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace DesktopOrganizer.Dialogs
{
    public static class DialogController
    {
        public static Task<MessageDialogResult> ShowAsync(IReactiveObject view_model, DialogButtonOptions button_options = DialogButtonOptions.OkAndCancel)
        {
            var window = Application.Current.MainWindow as MetroWindow;
            if (window == null)
                throw new InvalidOperationException("Main window must be a MetroWindow");

            var dialog = new HostDialog(button_options) {DataContext = view_model};

            return window.ShowMetroDialogAsync(dialog)
                         .ContinueWith(async _ =>
                             {
                                 var result = await dialog.Task;
                                 await window.HideMetroDialogAsync(dialog);
                                 return result;
                             }, TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();
        }

        public static Task<ProgressDialogController> ShowBusyDialog(string title, string message)
        {
            var window = Application.Current.MainWindow as MetroWindow;
            if (window == null)
                throw new InvalidOperationException("Main window must be a MetroWindow");

            return window.ShowProgressAsync(title, message, false, new MetroDialogSettings {AnimateShow = false, AnimateHide = true});
        }
    }
}
