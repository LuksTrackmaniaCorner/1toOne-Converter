using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;

namespace _1toOne_Converter.src.conversion
{
    public class EnviConversion : Conversion
    {
        [XmlElement(ElementName = "Deco")]
        public Decoration[] Decos;

        public GBXNat3 MapSize;

        public override void Convert(GBXFile file)
        {
            var commonChunk = (ChallengeCommon)file.GetChunk(Chunk.challengeCommonKey);
            var chunk0304301F = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);

            Decoration newDeco = null;
            var oldDeco = chunk0304301F.DecorationMeta.ID.Content;
            foreach (var deco in Decos)
            {
                if (deco.Name == oldDeco)
                {
                    newDeco = deco;
                    break;
                }
            }
            if (newDeco == null)
                throw new UnsupportedMapBaseException();

            //Change data in common chunk
            commonChunk.TrackMeta.Collection = (GBXLBS) newDeco.Deco.Collection.DeepClone();
            commonChunk.DecorationMeta = (Meta) newDeco.Deco.DeepClone(); ;
            
            //Change data in map chunk
            chunk0304301F.TrackMeta.Collection = (GBXLBS) newDeco.Deco.Collection.DeepClone();
            chunk0304301F.DecorationMeta = (Meta) newDeco.Deco.DeepClone();

            //Adjust block Position
            if(newDeco.GridOffset != null)
            {
                file.Move(newDeco.GridOffset.X, newDeco.GridOffset.Y, newDeco.GridOffset.Z);
            }

            //Adjust map size
            chunk0304301F.MapSize = (GBXNat3) MapSize.DeepClone();
        }
    }

    public class Decoration
    {
        [XmlAttribute]
        public string Name;

        [XmlElement(ElementName = "NewDeco")]
        public Meta Deco;

        [XmlElement(IsNullable = false)]
        public Vector GridOffset;
    }

    public class Vector
    {
        [XmlAttribute]
        public sbyte X;

        [XmlAttribute]
        public sbyte Y;

        [XmlAttribute]
        public sbyte Z;
    }
}
