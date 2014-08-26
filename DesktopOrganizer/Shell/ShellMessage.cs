using DesktopOrganizer.Utils;

namespace DesktopOrganizer.Shell
{
    public class ShellMessage
    {
        public enum MessageKind { Back, Show }

        public MessageKind Kind { get; set; }
        public ViewModelBase ViewModel { get; set; }

        public ShellMessage(MessageKind kind, ViewModelBase view_model)
        {
            Kind = kind;
            ViewModel = view_model;
        }

        public static ShellMessage BackMessage()
        {
            return new ShellMessage(MessageKind.Back, null);
        }

        public static ShellMessage ShowMessage(ViewModelBase view_model)
        {
            return new ShellMessage(MessageKind.Show, view_model);
        }
    }
}
