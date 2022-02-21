using System.Collections.Generic;
using System.Linq;

namespace QboProductUpdater.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TSource>> BatchesOf<TSource>(
            this IEnumerable<TSource> source, int batchSize)
        {
            var items = new TSource[batchSize];
            var count = 0;
            foreach (var item in source)
            {
                items[count++] = item;
                if (count == batchSize)
                {
                    yield return items;
                    items = new TSource[batchSize];
                    count = 0;
                }
            }
            if (count > 0)
                yield return items.Take(count);
        }
    }
}