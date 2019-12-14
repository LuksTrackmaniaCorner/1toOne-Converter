using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;

namespace _1toOne_Converter.src.conversion
{
    public class EnviConversion : Conversion
    {
        public Meta DecorationMeta; //TODO deal with different times of day
        public GBXNat3 MapSize;

        public override void Convert(GBXFile file)
        {
            var commonChunk = (ChallengeCommon)file.GetChunk(Chunk.challengeCommonKey);
            commonChunk.TrackMeta.Collection = (GBXLBS) DecorationMeta.Collection.DeepClone();

            commonChunk.DecorationMeta = (Meta) DecorationMeta.DeepClone(); ;

            var chunk0304301F = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            chunk0304301F.TrackMeta.Collection = (GBXLBS) DecorationMeta.Collection.DeepClone();

            chunk0304301F.DecorationMeta = (Meta) DecorationMeta.DeepClone();

            chunk0304301F.MapSize = (GBXNat3) MapSize.DeepClone();
        }
    }
}
