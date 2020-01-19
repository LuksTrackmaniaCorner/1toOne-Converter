using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxSkippingArray<T> : GbxArray<T> where T : GbxComponent
    {
        public delegate bool StopCondition(StreamReader reader, uint count);

        private readonly Predicate<T> _skipper;
        private readonly StopCondition _stopCondition;

        public GbxSkippingArray(Func<T> producer, Predicate<T> skipper, StopCondition stopCondition) : base(producer)
        {
            _skipper = skipper;
            _stopCondition = stopCondition;
        }

        public bool IsSkipped(int index) => IsSkipped(this[index]);

        public bool IsSkipped(T value)
        {
            return _skipper(value);
        }

        public int GetUnskippedCount()
        {
            int result = 0;

            foreach(var child in this)
            {
                if (!IsSkipped(child))
                    result++;
            }

            return result;
        }

        internal bool ShouldStopReading(StreamReader reader, uint count)
        {
            return _stopCondition(reader, count);
        }
    }
}
