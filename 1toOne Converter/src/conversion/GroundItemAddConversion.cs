﻿using System;
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
using _1toOne_Converter.src.util;

namespace _1toOne_Converter.src.conversion
{
    public class GroundItemAddConversion : Conversion
    {
        public GBXLBS Collection;
        public GBXLBS DefaultAuthor;
        public GBXByte Height;

        [XmlElement(ElementName = "BlockIgnoreFlag")]
        public FlagName[] BlockIgnoreFlags;

        [XmlElement]
        public FlagName HeightFlag;

        public ItemData GroundItem;

        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

            var mapXSize = blockChunk.MapSize.X;
            var mapZSize = blockChunk.MapSize.Z;

            var ignoredTiles = new bool[GBXFile.MaxMapXSize, GBXFile.MaxMapZSize];

            //Initializing ignoredTiles
            foreach(var blockIgnoreFlag in BlockIgnoreFlags)
            {
                for(int x = 0; x < GBXFile.MaxMapXSize; x++)
                {
                    for(int z = 0; z < GBXFile.MaxMapZSize; z++)
                    {
                        if(file.TestFlag(blockIgnoreFlag.Name, x, z))
                        {
                            ignoredTiles[x, z] = true;
                        }
                    }
                }
            }

            //Placing the ground on all other tiles
            for(byte x = 0; x < mapXSize; x++)
            {
                for(byte z = 0; z < mapZSize; z++)
                {
                    if (ignoredTiles[x, z])
                        continue;

                    //Tile not ignored
                    //ground item needs to be placed
                    byte height = 0;
                    if (HeightFlag != null)
                    {
                        height = file.GetFlag(HeightFlag.Name, x, z);
                    }

                    //Creating the item chunk
                    var itemInfo = GroundItem.GetItemInfo(new Identifier(null, 0, false, null));
                    itemInfo.PlaceAt(file, (x, (byte)(Height.Value + height), z), 0, itemChunk, Collection, DefaultAuthor.Content);
                }
            }
        }
    }
}
