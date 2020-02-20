using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.conversion
{
    public class MultiBlockAddConversion : Conversion
    {
        public Block NewBlock;
        public Range XRange;
        public Range YRange;
        public Range ZRange;

        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);

            blockChunk.Version.Value = 6; //Adding Support for Custom Blocks

            var block = (Block)NewBlock.DeepClone();

            foreach(var x in XRange.GetValues())
            {
                foreach(var y in YRange.GetValues())
                {
                    foreach(var z in ZRange.GetValues())
                    {
                        block.Coords.X = x;
                        block.Coords.Y = y;
                        block.Coords.Z = z;
                        blockChunk.Blocks.Add((Block)block.DeepClone());
                    }
                }
            }
        }
    }

    public class Range
    {
        [XmlAttribute]
        public byte start;
        [XmlAttribute]
        public byte end;
        [XmlAttribute]
        public byte step = 1;

        public IEnumerable<byte> GetValues()
        {
            byte current = start;

            do
            {
                yield return current;
                current += step;
            }
            while (current <= end);
        }
    }
}
