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

        [XmlElement(ElementName = "ClipBlock")]
        public GBXLBS[] ClipBlocks;
        [XmlElement(IsNullable = false, ElementName = "SecondaryTerrainClipBlock")]
        public GBXLBS[] SecondaryTerrainClipBlocks;
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

            itemCount = PlaceClips(file, itemCount, blockChunk, itemChunk);
            //itemCount = PlaceClipsForced(file, itemCount, itemChunk);

            if (ItemCountStatistic != null)
            {
                file.AddStatistic(ItemCountStatistic.Name, itemCount);
            }
        }

        private int PlaceClips(GBXFile file, int itemCount, Challenge0304301F blockChunk, Challenge03043040 itemChunk)
        {
            var clipList = new List<(ClipData clipItemInfo, byte rot)>[GBXFile.MaxMapXSize, GBXFile.MaxMapYSize, GBXFile.MaxMapZSize];

            foreach (var clipItemInfo in ClipItemInfos)
            {
                var clips = file.GetClips(clipItemInfo.Clip);
                if (clips != null)
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
                //Test if block is clip and collect the needed metadata
                bool isSecondaryTerrain;

                if (ClipBlocks.Any(x => x.Equals(block.BlockName)))
                    isSecondaryTerrain = false;
                else if (SecondaryTerrainClipBlocks.Any(x => x.Equals(block.BlockName)))
                    isSecondaryTerrain = true;
                else
                    continue; //Block is not a Clip, check next block.

                uint groundFlag = block.Flags.Value & 0x1000;
                bool isGround = groundFlag != 0;

                //Block is Clip
                //Get all clips in this position
                var positionedClips = clipList[block.Coords.X, block.Coords.Y, block.Coords.Z];
                if (positionedClips != null)
                {
                    var placedClips = new bool[4];

                    //Get Clips by rotation
                    var rotatedClips = from clip in positionedClips
                                       group clip.clipItemInfo by clip.rot into g
                                       select g;

                    foreach (var rotation in rotatedClips)
                    {
                        var rot = rotation.Key;
                        var neighbourCoords = Clip.GetCoordsFacing(block.Coords.Value, rot);

                        var neighbourClips = clipList[neighbourCoords.x, neighbourCoords.y, neighbourCoords.z];

                        foreach (var clipItemInfo in rotation)
                        {
                            if (neighbourClips != null && neighbourClips.Exists(x => x.rot == (rot + 2) % 4 && x.clipItemInfo.Clip == clipItemInfo.Clip))
                            {
                                //clip is connected, try next clip
                                continue;
                            }

                            //clip is unconnected, items must be placed
                            var clipItem = clipItemInfo.GetItemInfo(new Identifier(null, groundFlag, isSecondaryTerrain, null));
                            clipItem.PlaceWithOffset(file, block.Coords.Value, rot, itemChunk, Collection, DefaultAuthor.Content);

                            placedClips[rot] = true;

                            itemCount++;
                        }
                    }

                    if (isGround)
                    {
                        //Place filler items
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

            return itemCount;
        }

        private int PlaceClipsForced(GBXFile file, int itemCount, Challenge03043040 itemChunk)
        {
            foreach (var clipItemInfo in ClipItemInfos)
            {
                var clips = file.GetClips(clipItemInfo.Clip);
                if (clips != null)
                {
                    foreach (var clip in clips)
                    {
                        //TODO support the different clip types
                        var clipItem = clipItemInfo.GetItemInfo(new Identifier(null, 0, false, null));
                        clipItem.PlaceWithOffset(file, ((byte)clip.X, (byte)clip.Y, (byte)clip.Z), clip.Rot, itemChunk, Collection, DefaultAuthor.Content);

                        itemCount++;
                    }
                }
            }

            return itemCount;
        }
    }

    public class ClipData : ItemData
    {
        [XmlAttribute]
        public string Clip;
    }
}
