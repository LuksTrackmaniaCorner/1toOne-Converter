using Gbx.Parser.Primitive;
using Gbx.Parser.Visit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxFileReference : GbxStructure
    {
        public const int ChecksumLength = 32;

        public GbxByte Version { get; } = new GbxByte();
        public GbxUnread Checksum { get; } = new GbxUnread(ChecksumLength);
        public GbxString FilePath { get; } = new GbxString();
        public GbxString LocatorUrl { get; } = new GbxString();

        public bool IsRelativePath => Version >= 2;

        public override IEnumerable<(string, GbxComponent)> GetNamedChildren()
        {
            yield return (nameof(Version), Version);

            if (Version >= 3)
                yield return (nameof(Checksum), Checksum);

            yield return (nameof(FilePath), FilePath);

            if (FilePath.Length > 0 && Version >= 1)
                yield return (nameof(FilePath), FilePath);
        }

        internal override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
