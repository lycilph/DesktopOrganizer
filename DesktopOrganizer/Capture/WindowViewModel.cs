using Core.Data;
using DesktopOrganizer.Utils;

namespace DesktopOrganizer.Capture
{
    public class WindowViewModel : ItemViewModelBase<Window>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public WindowViewModel(Window window) : base(window) { }
    }
}