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
using _1toOne_Converter.src.util;

namespace _1toOne_Converter.src.conversion
{
    public class ItemClipAddConversion : Conversion
    {
        public GBXLBS Collection;
        public GBXLBS DefaultAuthor;

        [XmlElement]
        public GBXLBS ClipBlock;
        [XmlElement(IsNullable = false)]
        public GBXLBS SecondaryTerrainClipBlock;
        [XmlElement(IsNullable = false)]
        public ItemData ClipFiller;

        public FlagName GroundClipFlag;
        public MultiPylon GroundClipPylon;

        [XmlArray]
        public ClipData[] ClipItemInfos;

        public FlagName ItemCountStatistic;

        public override void Convert(GBXFile file)
        {
            int itemCount = 0;

            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

            var clipList = new List<(ClipData clipItemInfo, byte rot)>[GBXFile.MaxMapXSize, GBXFile.MaxMapYSize, GBXFile.MaxMapZSize];

            foreach(var clipItemInfo in ClipItemInfos)
            {
                var clips = file.GetClips(clipItemInfo.Clip);
                if(clips != null)
                {
                    foreach (var clip in clips)
                    {
                        var list = clipList[clip.X, clip.Y, clip.Z] ??= new List<(ClipData clipItemInfo, byte rot)>();
                        list.Add((clipItemInfo, clip.Rot));
                    }
                }
            }

            foreach (var block in blockChunk.Blocks)
            {
                bool isSecondaryTerrain;

                if(block.BlockName.Equals(ClipBlock))
                    isSecondaryTerrain = false;
                else if(SecondaryTerrainClipBlock != null && block.BlockName.Equals(SecondaryTerrainClipBlock))
                    isSecondaryTerrain = true;
                else
                    continue; //Block is not a Clip, check next block.

                bool isGround = (block.Flags.Value & 0x1000) != 0;

                //Block is Clip
                //Get all clips in this position
                var clips = clipList[block.Coords.X, block.Coords.Y, block.Coords.Z];
                if (clips != null)
                {
                    var placedClips = new bool[4];

                    foreach (var (clipItemInfo, rot) in clips)
                    {
                        uint flags = isGround switch
                        {
                            true => 0x1000,
                            false => 0
                        };

                        var clipItem = clipItemInfo.GetItemInfo(new Identifier(null, flags, isSecondaryTerrain, null));
                        clipItem.PlaceAt(file, block.Coords.Value, rot, itemChunk, Collection, DefaultAuthor.Content);

                        placedClips[rot] = true;

                        itemCount++;
                    }

                    if(isGround)
                    {
                        for (byte rot = 0; rot < 4; rot++)
                        {
                            if (!placedClips[rot])
                            {
                                var fillerItem = ClipFiller.GetItemInfo(new Identifier(null, 0x1000, isSecondaryTerrain, null));
                                block.Rot.Value = rot;
                                fillerItem.PlaceAt(file, block.Coords.Value, rot, itemChunk, Collection, DefaultAuthor.Content);
                            }
                        }

                        file.SetFlag(new Flag(GroundClipFlag, block.Coords.X, block.Coords.Z));
                        file.AddPylons(GroundClipPylon.GetPylons().AsEnumerable(), block.Coords.X, block.Coords.Y, block.Coords.Z, 0);
                    }
                }
            }

            if(ItemCountStatistic != null)
            {
                file.AddStatistic(ItemCountStatistic.Name, itemCount);
            }
        }
    }

    public class ClipData : ItemData
    {
        [XmlAttribute]
        public string Clip;
    }
}
