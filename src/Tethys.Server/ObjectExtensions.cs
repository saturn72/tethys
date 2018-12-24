using System.Collections.Generic;
using System.Linq;

namespace Tethys.Server
{
    public static class ObjectExtensions
    {
        public static bool HasValue(this string source)
        {
            return source != null && source.Trim().Length > 0;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }
    }
}