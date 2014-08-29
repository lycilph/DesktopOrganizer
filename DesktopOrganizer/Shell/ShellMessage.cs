using Framework.Mvvm;

namespace DesktopOrganizer.Shell
{
    public class ShellMessage
    {
        public enum MessageKind { Back, Show, Exit }

        public MessageKind Kind { get; set; }
        public IViewModel ViewModel { get; set; }

        public ShellMessage(MessageKind kind, IViewModel view_model)
        {
            Kind = kind;
            ViewModel = view_model;
        }

        public static ShellMessage BackMessage()
        {
            return new ShellMessage(MessageKind.Back, null);
        }

        public static ShellMessage ShowMessage(IViewModel view_model)
        {
            return new ShellMessage(MessageKind.Show, view_model);
        }

        public static ShellMessage ExitMessage()
        {
            return new ShellMessage(MessageKind.Exit, null);
        }
    }
}
