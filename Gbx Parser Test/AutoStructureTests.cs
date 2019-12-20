using Gbx.Parser.Core;
using Gbx.Parser.Primitive;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx_Test
{
    public class AutoStructureTests : GbxStructure
    {
        [Test]
        public static void AutoStructureTest()
        {
            var testee = new AutoStructureTests();

            var enumerator = testee.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(testee.One, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(testee.Two, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(testee.Three, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(testee.Four, enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
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
