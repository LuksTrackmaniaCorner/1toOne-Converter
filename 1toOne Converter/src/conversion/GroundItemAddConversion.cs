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
using _1toOne_Converter.src.util;

namespace _1toOne_Converter.src.conversion
{
    public class GroundItemAddConversion : Conversion
    {
        public GBXVec3 GridSize;
        public GBXVec3 GridOffset;

        public Marker MarkerTileIgnore;
        public Meta GroundItem;

        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

            var mapXSize = blockChunk.MapSize.X;
            var mapZSize = blockChunk.MapSize.Z;

            //Determine the tiles where no blocks should be placed
            var tileIgnore = new bool[mapXSize, mapZSize];
            foreach (var (x, y, z) in file.GetMarkers(MarkerTileIgnore.Name))
            {
                tileIgnore[(int) (x / GridSize.X), (int) (z / GridSize.Z)] = true;
            }

            //Placing the ground on all other tiles
            for(int x = 0; x < mapXSize; x++)
            {
                for(int z = 0; z < mapZSize; z++)
                {
                    if (!tileIgnore[x, z])
                    {
                        //ground item needs to be placed

                        //Creating the item chunk
                        var anchoredObject = new AnchoredObject03101002(
                            new GBXUInt(7),
                            (Meta)GroundItem.DeepClone(),
                            new GBXVec3(0 ,0 ,0),
                            new GBXByte3((byte) x, 0, (byte) z),
                            new GBXUInt(0xFFFFFFFF),
                            new GBXVec3(x * GridSize.X + GridOffset.X, GridOffset.Y, z * GridSize.Z + GridOffset.Z),
                            new GBXUInt(0xFFFFFFFF),
                            new GBXUShort(1),
                            new GBXVec3(0, 0, 0),
                            new GBXFloat(1)
                        );

                        //Wrapping the item chunk in a node
                        var itemNode = new Node(0x03101000);
                        itemNode.AddChunk(Chunk.anchoredObject03101002Key, anchoredObject);

                        //Adding the item node
                        itemChunk.Items.Add(itemNode);
                    }
                }
            }
        }
    }
}
