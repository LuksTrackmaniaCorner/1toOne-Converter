using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Converter.Gbx;
using Converter.Gbx.Chunks.Challenge;
using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using Converter.Util;

namespace Converter.Conversion
{
    public class EnviConversion : Conversion
    {
        [XmlElement(ElementName = "NewDeco")]
        public NewDecoration[] NewDecos;

        private Dictionary<string, (OldDecoration oldDecoration, NewDecoration newDecoration)> _oldDecoDict;

        public GBXNat3 MapSize;

        internal override void Initialize()
        {
            _oldDecoDict = new Dictionary<string, (OldDecoration oldDecoration, NewDecoration newDecoration)>();

            var decos = from newDeco in NewDecos
                        from oldDeco in newDeco.OldDeco
                        select (oldDeco, newDeco);

            foreach (var deco in decos)
                _oldDecoDict.Add(deco.oldDeco.Name, deco);
        }

        public override void Convert(GBXFile file)
        {
            var commonChunk = (ChallengeCommon)file.GetChunk(Chunk.challengeCommonKey);
            var chunk0304301F = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

            var mapDeco = chunk0304301F.DecorationMeta.ID.Content;

            if(!_oldDecoDict.ContainsKey(mapDeco))
                throw new UnsupportedMapBaseException();

#pragma warning disable IDE0042 // Deconstruct variable declaration
            var deco = _oldDecoDict[mapDeco];
#pragma warning restore IDE0042 // Deconstruct variable declaration
            var newDeco = deco.newDecoration;
            var gridOffset = deco.oldDecoration.GridOffset;
            var mapSize = deco.newDecoration.MapSize ?? this.MapSize;

            //Change data in common chunk
            commonChunk.TrackMeta.Collection = (GBXLBS) newDeco.Deco.Collection.DeepClone();
            commonChunk.DecorationMeta = (Meta) newDeco.Deco.DeepClone(); ;
            
            //Change data in map chunk
            chunk0304301F.TrackMeta.Collection = (GBXLBS) newDeco.Deco.Collection.DeepClone();
            chunk0304301F.DecorationMeta = (Meta) newDeco.Deco.DeepClone();

            //Adjust block Position
            if(gridOffset != null)
            {
                file.Move(gridOffset.X, gridOffset.Y, gridOffset.Z);
            }

            //Adjust map size
            chunk0304301F.MapSize = (GBXNat3) mapSize.DeepClone();

            //Place Warp items
            var warpItems = newDeco.WarpItems;
            if (warpItems != null)
            {
                if (itemChunk == null)
                {
                    itemChunk = new Challenge03043040(false);
                    file.AddBodyChunk(Chunk.challenge03043040Key, itemChunk);
                }

                foreach (var warpItem in warpItems)
                {
                    itemChunk.AddItem(warpItem);
                }
            }
                 
        }
    }

    public class OldDecoration
    {
        [XmlAttribute]
        public string Name;

        [XmlElement]
        public Vector GridOffset;
    }

    public class NewDecoration
    {
        [XmlElement]
        public OldDecoration[] OldDeco;

        [XmlElement]
        public Meta Deco;

        [XmlElement(IsNullable = false, ElementName ="WarpItem")]
        public MinimalItem[] WarpItems;

        [XmlElement]
        public GBXNat3 MapSize;
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
