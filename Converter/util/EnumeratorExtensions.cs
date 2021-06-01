using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Util
{
    public static class EnumeratorExtensions
    {
        public static IEnumerable<T> AsEnumerable<T> (this T t)
        {
            yield return t;
        }

        public static IEnumerable<T> AsEnumerable<T> (this IEnumerator<T> t) => new EnumerableEnumerator<T>(t);
    }

    public class EnumerableEnumerator<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public EnumerableEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;

        public IEnumerator<T> GetEnumerator() => _enumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
