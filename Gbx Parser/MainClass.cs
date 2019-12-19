using gbx.parser.core;
using gbx.parser.primitive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace gbx
{
    public class MainClass : GbxStructure
    {
        public static void Main(string[] args)
        {
            var testee = new MainClass();

            var stopwatch = Stopwatch.StartNew();

            for(int i = 0; i < 1000000; i++)
            {
                var enumerator = testee.GetEnumerator();

                enumerator.MoveNext();
                if (!object.ReferenceEquals(enumerator.Current, testee.One))
                    throw new Exception();

                enumerator.MoveNext();
                if (!object.ReferenceEquals(enumerator.Current, testee.Two))
                    throw new Exception();

                enumerator.MoveNext();
                if (!object.ReferenceEquals(enumerator.Current, testee.Three))
                    throw new Exception();

                enumerator.MoveNext();
                if (!object.ReferenceEquals(enumerator.Current, testee.Four))
                    throw new Exception();

            }

            stopwatch.Stop();
            Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds);
        }

        [AutoStructure]
        public GbxUInt One { get; } = new GbxUInt();
        [AutoStructure]
        public GbxUInt Two { get; } = new GbxUInt();
        [AutoStructure]
        public GbxUInt Three { get; } = new GbxUInt();
        [AutoStructure]
        public GbxUInt Four { get; } = new GbxUInt();

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            return AutoGetNamedChildren();
        }
    }
}
