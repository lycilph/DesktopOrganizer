using ReactiveUI;

namespace DesktopOrganizer.Utils
{
    public class ItemViewModelBase<T> : ReactiveObject
    {
        public T AssociatedObject { get; private set; }

        public ItemViewModelBase(T obj)
        {
            AssociatedObject = obj;
        }
    }
}
