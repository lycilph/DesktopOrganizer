using Core.Data;
using DesktopOrganizer.Utils;

namespace DesktopOrganizer.Main
{
    public class ProgramLayoutViewModel : ItemViewModelBase<Layout<Program>>
    {
        public string LayoutName { get { return AssociatedObject.Name; } }
        public string Shortcut { get { return AssociatedObject.Shortcut.ToString(); } }

        public ProgramLayoutViewModel(Layout<Program> layout) : base(layout) { }
    }
}
