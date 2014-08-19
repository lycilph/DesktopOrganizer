using System.Collections.Generic;
using ReactiveUI;

namespace DesktopOrganizer.Utils
{
    public static class EnumerableExtensions
    {
        public static ReactiveList<T> ToReactiveList<T>(this IEnumerable<T> source)
        {
            return new ReactiveList<T>(source);
        }
    }
}
