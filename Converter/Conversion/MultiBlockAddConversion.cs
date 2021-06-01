using Converter.Gbx;
using Converter.Gbx.Chunks.Challenge;
using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Converter.Conversion
{
    public class MultiBlockAddConversion : Conversion
    {
        public Block NewBlock;
        public GBXByte XStep;
        public GBXByte YValue;
        public GBXByte ZStep;

        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var chunk0304301F = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var mapSize = chunk0304301F.MapSize;

            blockChunk.Version.Value = 6; //Adding Support for Custom Blocks

            var block = (Block)NewBlock.DeepClone();

            for(byte x = 1; x <= mapSize.X; x += XStep.Value)
            {
                for(byte z = 1; z <= mapSize.Z; z += ZStep.Value)
                {
                    block.Coords.X = x;
                    block.Coords.Y = YValue.Value;
                    block.Coords.Z = z;
                    blockChunk.Blocks.Add((Block)block.DeepClone());
                }
            }
        }
    }
}
