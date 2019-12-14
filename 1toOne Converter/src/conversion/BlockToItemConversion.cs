using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using _1toOne_Converter.src.util;

namespace _1toOne_Converter.src.conversion
{
    public class BlockToItemConversion : Conversion
    {
        private const float Yaw0 = 0;
        private const float Yaw1 = -1.57079632679489f;
        private const float Yaw2 = -3.14159265358979f;
        private const float Yaw3 = 1.57079632679489f;

        public GBXVec3 GridSize;
        public GBXVec3 GridOffset;

        public GBXLBS Collection;
        public GBXLBS DefaultAuthor; //TODO allow individual items to override the author.

        public BlockToItem[] BlockToItems;

        public override void Convert(GBXFile file)
        {

            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

            if (itemChunk == null) {
                itemChunk = new Challenge03043040(false);
                file.AddBodyChunk(Chunk.challenge03043040Key, itemChunk);
            }

            foreach (var block in blockChunk.Blocks)
            {
                foreach (var blockToItem in BlockToItems)
                {
                    if(block.BlockName.Equals(blockToItem.BlockName))
                    {
                        //Block which needs to be replaced with an item has been detected
                        //Searching for the item for the current block variant.
                        var coords = ConvertCoords(block, blockToItem.YOffset);
                        bool blockConverted = false;
                        uint blockFlags = block.Flags.Value;

                        foreach (var blockVariant in blockToItem.BlockVariants)
                        {
                            if((blockFlags & 0x3F) == blockVariant.Variant && (((blockFlags & 0x1000) != 0) == blockVariant.IsGroundVariant))
                            {
                                bool blockVariantConverted = false;
                                foreach (var randomVariant in blockVariant.RandomVariants)
                                {
                                    if(((blockFlags >> 6) & 0x3F) == randomVariant.Variant)
                                    {
                                        //Create and add item
                                        var anchoredObject = new AnchoredObject03101002(
                                            new GBXUInt(7),
                                            GenerateItemMeta(randomVariant),
                                            ConvertRot(block, blockVariant.RotOffset),
                                            (GBXByte3)block.Coords.DeepClone(),
                                            new GBXUInt(0xFFFFFFFF),
                                            coords,
                                            new GBXUInt(0xFFFFFFFF),
                                            new GBXUShort(1),
                                            GenerateUnknownVec(),
                                            new GBXFloat(1)
                                        );

                                        var itemNode = new Node(0x03101000);
                                        itemNode.AddChunk(Chunk.anchoredObject03101002Key, anchoredObject);

                                        itemChunk.Items.Add(itemNode);

                                        blockVariantConverted = blockConverted = true;

                                        break;
                                    }
                                }

                                //Add markers for block variant
                                if (blockVariantConverted)
                                    AddMarkers(file, blockVariant.Markers, coords);

                                break;
                            }
                        }

                        //Add markers for block
                        if (blockConverted)
                            AddMarkers(file, blockToItem.Markers, coords);

                        //No need to check this block with more blockToItem rules;
                        break;
                    }
                }
            }
        }

        private Meta GenerateItemMeta(RandomVariant randomVariant)
        {
            return new Meta(
               new GBXLBS(randomVariant.ItemName),
               (GBXLBS)Collection.DeepClone(),
               (GBXLBS)DefaultAuthor.DeepClone()
            );
        }

        private void AddMarkers(GBXFile file, Marker[] markers, GBXVec3 coords)
        {
            if (markers == null)
                return;

            foreach(var marker in markers)
            {
                file.AddMarker(marker.Name, coords);
            }
        }

        private GBXVec3 ConvertRot(Block block, byte rotOffset)
        {
            switch((block.Rot.Value + rotOffset) % 4)
            {
                case 0:
                    return new GBXVec3(Yaw0, 0, 0); //TODO
                case 1:
                    return new GBXVec3(Yaw1, 0, 0);
                case 2:
                    return new GBXVec3(Yaw2, 0, 0);
                case 3:
                    return new GBXVec3(Yaw3, 0, 0);
                default: throw new Exception();
            }
        }

        private GBXVec3 ConvertCoords(Block block, float yOffset)
        {
            return new GBXVec3(
                block.Coords.X * GridSize.X + GridOffset.X,
                block.Coords.Y * GridSize.Y + GridOffset.Y + yOffset,
                block.Coords.Z * GridSize.Z + GridOffset.Z
            );
        }

        private GBXVec3 GenerateUnknownVec()
        {
            return new GBXVec3(0, 0, 0);
        }
    }

    public class BlockToItem
    {
        [XmlAttribute]
        public float YOffset;
        public GBXLBS BlockName;

        public BlockVariant[] BlockVariants;
        [XmlElement(IsNullable = false)]
        public Marker[] Markers;
    }

    public class BlockVariant
    {
        [XmlAttribute]
        public byte Variant;
        [XmlAttribute]
        public bool IsGroundVariant;
        [XmlAttribute]
        public byte RotOffset;

        public RandomVariant[] RandomVariants;
        [XmlElement(IsNullable = false)]
        public Marker[] Markers;
    }

    public class RandomVariant
    {
        [XmlAttribute]
        public byte Variant;

        [XmlAttribute]
        public string ItemName;
    }
}
