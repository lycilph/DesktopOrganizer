using Core.Data;
using Framework.Mvvm;

namespace DesktopOrganizer.Capture
{
    public class WindowViewModel : ItemViewModelBase<Window>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public WindowViewModel(Window window) : base(window) { }
    }
}