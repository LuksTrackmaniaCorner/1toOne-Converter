using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Util
{
    public interface IDelEnumerable<out T> : IEnumerable<T>
    {
        IRemoveEnumerator<T> GetRemoveEnumerator();
    }

    public interface IRemoveEnumerator<out T> : IEnumerator<T>
    {
        void Remove();
    }
}
