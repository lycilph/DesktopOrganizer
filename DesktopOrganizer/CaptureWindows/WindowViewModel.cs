using DesktopOrganizer.Data;
using DesktopOrganizer.Utils;

namespace DesktopOrganizer.CaptureWindows
{
    public class WindowViewModel : ItemViewModelBase<Window>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public WindowViewModel(Window window) : base(window) { }
    }
}