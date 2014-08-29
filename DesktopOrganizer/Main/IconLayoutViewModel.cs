using Core.Data;
using Framework.Mvvm;

namespace DesktopOrganizer.Main
{
    public class IconLayoutViewModel : ItemViewModelBase<Layout<Icon>>, ILayoutViewModel
    {
        public string LayoutName { get { return AssociatedObject.Name; } }
        public string Shortcut { get { return AssociatedObject.Shortcut.ToString(); } }

        public IconLayoutViewModel(Layout<Icon> layout) : base(layout) { }
    }
}
