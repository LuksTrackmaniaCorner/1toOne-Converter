using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Util
{
    public static class RandomExtensions
    {
        public static ulong NextULong(this Random random)
        {
            var buf = new byte[8];
            random.NextBytes(buf);
            return BitConverter.ToUInt64(buf, 0);
        }
    }
}
