using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.util
{
    public static class Memoize
    {
        public static Func<TIn, TResult> MakeMemoized<TIn, TResult>(Func<TIn, TResult> function)
        {
            var MemoizedInputs = new Dictionary<TIn, TResult>();

            return MemoizedFunction;

            TResult MemoizedFunction(TIn input)
            {
                if (MemoizedInputs.ContainsKey(input))
                    return MemoizedInputs[input];
                else
                {
                    var result = function(input);
                    MemoizedInputs[input] = result;
                    return result;
                }
            }
        }

        public static Func<T1, T2, TResult> MakeMemoized<T1, T2, TResult>(Func<T1, T2, TResult> function)
        {
            var MemoizedInputs = new Dictionary<(T1, T2), TResult>();

            return MemoizedFunction;

            TResult MemoizedFunction(T1 input1, T2 input2)
            {
                var input = (input1, input2);

                if (MemoizedInputs.ContainsKey(input))
                    return MemoizedInputs[input];
                else
                {
                    var result = function(input1, input2);
                    MemoizedInputs[input] = result;
                    return result;
                }
            }
        }
    }
}
