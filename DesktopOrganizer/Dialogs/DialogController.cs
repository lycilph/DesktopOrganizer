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
        public static Task<MessageDialogResult> ShowAsync(IReactiveObject view_model, bool show_cancel = true)
        {
            var window = Application.Current.MainWindow as MetroWindow;
            if (window == null)
                throw new InvalidOperationException("Main window must be a MetroWindow");

            var dialog = new HostDialog(show_cancel) {DataContext = view_model};

            return window.ShowMetroDialogAsync(dialog)
                         .ContinueWith(async _ =>
                             {
                                 var result = await dialog.Task;
                                 await window.HideMetroDialogAsync(dialog);
                                 return result;
                             }, TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();
        }
    }
}
