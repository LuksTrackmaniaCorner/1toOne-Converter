using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using _1toOne_Converter.src.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.conversion
{
    public abstract class BlockToItem
    {
        [XmlAttribute]
        public string ItemName { get; set; }
        [XmlAttribute]
        public string ItemAuthor { get; set; }
        [XmlAttribute]
        public sbyte YOffset { get; set; }
        [XmlAttribute]
        public byte RotOffset { get; set; }
        [XmlAttribute]
        public sbyte XOffset { get; set; }
        [XmlAttribute]
        public sbyte ZOffset { get; set; }
        [XmlAttribute]
        public float SmallYOffset { get; set; }

        private BlockToItem[] _children;

        //TODO do same thing with complexconversion
        [XmlElement(ElementName = "BlockData", Type = typeof(BlockData))]
        [XmlElement(ElementName = "BlockVariantData", Type = typeof(BlockVariantData))]
        [XmlElement(ElementName = "BlockTypeData", Type = typeof(BlockTypeData))]
        [XmlElement(ElementName = "BlockRandomData", Type = typeof(BlockRandomData))]
        [XmlElement(ElementName = "BlockSkinData", Type = typeof(BlockSkinData))]
        public BlockToItem[] Children
        {
            get => _children ??= Array.Empty<BlockToItem>();
            set => _children = value;
        }
        [XmlElement(ElementName = "Flag")]
        public Flag[] Flags { get; set; }
        [XmlElement(ElementName = "Clip")]
        public Clip[] Clips { get; set; }
        [XmlElement(ElementName = "Pylon")]
        public MultiPylon[] MultiPylons { get; set; }

        /// <summary>
        /// Tests if this BlockToItem object fits could know which item the given block represents.
        /// </summary>
        /// <param name="blockname"></param>
        /// <param name="flags"></param>
        /// <param name="isSecondaryTerrain"></param>
        /// <returns>True, if the block could correspond to this BlockToItem object or one of its children.</returns>
        internal abstract bool TestBlock(Identifier identifier);

        internal virtual ItemInfo GetItemInfo(Identifier identifier)
        {
            if (!TestBlock(identifier))
                return null;

            ItemInfo result = null;
            if (Children.Length == 0)
                result = new ItemInfo(ItemName, ItemAuthor, RotOffset, (XOffset, YOffset, ZOffset), SmallYOffset);
            else
            {
                foreach (var child in Children)
                {
                    result = child.GetItemInfo(identifier);
                    if (result != null)
                    {
                        result.ItemAuthor ??= ItemAuthor;
                        result.RotOffset += RotOffset;
                        result.Offset.x += XOffset;
                        result.Offset.y += YOffset;
                        result.Offset.z += ZOffset;
                        result.SmallYOffset += SmallYOffset;
                        break;
                    }
                }
            }

            if (result != null)
            {
                if (Flags != null)
                    result.Flags.AddRange(Flags);
                if (Clips != null)
                    result.Clips.AddRange(Clips);
                if (MultiPylons != null)
                {
                    foreach (var multipylon in MultiPylons)
                    {
                        result.Pylons.AddRange(multipylon.GetPylons().AsEnumerable());
                    }
                }
            }

            return result;
        }

        public bool ShouldSerializeYOffset() => YOffset != 0;
        public bool ShouldSerializeRotOffset() => RotOffset != 0;
        public bool ShouldSerializeXOffset() => XOffset != 0;
        public bool ShouldSerializeZOffset() => ZOffset != 0;

        public IEnumerator<BlockToItem> GetEnumerator()
        {
            //Returns all elements of the BlockToItem tree.
            yield return this;
            foreach (var child in Children)
            {
                foreach (var childEnum in child)
                {
                    yield return childEnum;
                }
            }
        }
    }

    public class ItemData : BlockToItem
    {
        internal override bool TestBlock(Identifier identifier) => true;
    }

    public class BlockData : BlockToItem
    {
        [XmlAttribute]
        public string BlockName { get; set; }

        [XmlAttribute]
        public byte BlockXSize { get; set; }
        [XmlAttribute]
        public byte BlockZSize { get; set; }

        [XmlElement(ElementName = "AltName")]
        public AlternativeName[] AltNames;

        internal void Initialize()
        {
            if (BlockXSize <= 0)
                BlockXSize = 1;
            if (BlockZSize <= 0)
                BlockZSize = 1;

            if (AltNames == null)
                AltNames = Array.Empty<AlternativeName>();
        }

        internal override bool TestBlock(Identifier identifier)
        {
            if (BlockName != null && identifier.blockName == BlockName)
                return true;

            foreach(var altname in AltNames)
            {
                if(identifier.blockName == altname.BlockName)
                {
                    if (altname.PriSecTerrain is bool terrain)
                        identifier.isSecondaryTerrain = terrain;

                    return true;
                }
            }

            return false;
        }

        internal override ItemInfo GetItemInfo(Identifier identifier)
        {
            var result = base.GetItemInfo(identifier);
            result?.SetBlockSize(BlockXSize, BlockZSize);
            return result;
        }

        public (byte x, byte y, byte z) ApplyBlockOffset(Block block)
        {
            return ApplyBlockOffset(block.Rot.Value, block.Coords.Value);
        }

        internal (byte x, byte y, byte z) ApplyBlockOffset(byte rot, (byte x, byte y, byte z) coords)
        {
            int xOffset;
            int zOffset;

            switch (rot)
            {
                case 0:
                    xOffset = 0;
                    zOffset = 0;
                    break;
                case 1:
                    xOffset = BlockZSize - 1;
                    zOffset = 0;
                    break;
                case 2:
                    xOffset = BlockXSize - 1;
                    zOffset = BlockZSize - 1;
                    break;
                case 3:
                    xOffset = 0;
                    zOffset = BlockZSize - 1;
                    break;
                default: throw new InternalException();
            }

            return ((byte)(coords.x + xOffset), (byte)(coords.y), (byte)(coords.z + zOffset));
        }

        public bool ShouldSerializeBlockXSize() => BlockXSize > 1;
        public bool ShouldSerializeBlockZSize() => BlockZSize > 1;
    }

    public class BlockVariantData : BlockToItem
    {
        internal byte? _variant;

        [XmlAttribute]
        public byte Variant { get => _variant.Value; set => _variant = value; }

        internal override bool TestBlock(Identifier identifier)
        {
            if (_variant == null)
                return true;
            return (identifier.flags & 0x3F) == Variant;
        }

        public bool ShouldSerializeVariant() => _variant != null;
    }

    public class BlockTypeData : BlockToItem
    {
        internal Type? _type;

        [XmlAttribute(AttributeName = "Type")]
        public Type TypeOfBlock { get => _type.Value; set => _type = value; }

        internal override bool TestBlock(Identifier identifier)
        {
            if (_type == null)
                return true;

            bool isGround = (identifier.flags & 0x1000) != 0;

            return TypeOfBlock switch
            {
                Type.Air => isGround == false,
                Type.Ground => isGround == true,
                Type.GroundPrimary => isGround == true && identifier.isSecondaryTerrain == false,
                Type.GroundSecondary => isGround == true && identifier.isSecondaryTerrain == true,
                _ => throw new InternalException(),
            };
        }

        public bool ShouldSerializeTypeOfBlock() => _type != null;
    }

    public class BlockRandomData : BlockToItem
    {
        internal byte? _variant;

        [XmlAttribute]
        public byte Variant { get => _variant.Value; set => _variant = value; }

        internal override bool TestBlock(Identifier identifier)
        {
            if (_variant == null)
                return true;
            return ((identifier.flags >> 6) & 0x3F) == Variant;
        }
    }

    public class BlockSkinData : BlockToItem
    {
        [XmlAttribute]
        public string SkinRegex { get; set; }

        internal override bool TestBlock(Identifier identifier)
        {
            if (SkinRegex == null)
                return true;

            if (identifier.skinLocator == null)
                return false;

            var regex = new Regex(SkinRegex, RegexOptions.IgnoreCase);
            return regex.IsMatch(identifier.skinLocator);
        }
    }

    internal class Identifier : IEquatable<Identifier>
    {
        internal string blockName;
        internal uint flags;
        internal bool isSecondaryTerrain;
        internal string skinLocator;

        public Identifier(string blockName, uint flags, bool isSecondaryTerrain, string skinLocator)
        {
            this.blockName = blockName;
            this.flags = flags;
            this.isSecondaryTerrain = isSecondaryTerrain;
            this.skinLocator = skinLocator;
        }

        internal Identifier(Block block, bool isSecondaryTerrain)
        {
            blockName = block.BlockName.Content;
            flags = block.Flags.Value;
            this.isSecondaryTerrain = isSecondaryTerrain;
            skinLocator = ((BlockSkin03059002)block.Skin?.node.Get(Chunk.blockSkin03059002Key))?.packDesc.filePath.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Identifier);
        }

        public bool Equals(Identifier other)
        {
            return other != null &&
                   blockName == other.blockName &&
                   flags == other.flags &&
                   isSecondaryTerrain == other.isSecondaryTerrain &&
                   skinLocator == other.skinLocator;
        }

        public override int GetHashCode()
        {
            int hashCode = 344487553;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(blockName);
            hashCode = hashCode * -1521134295 + flags.GetHashCode();
            hashCode = hashCode * -1521134295 + isSecondaryTerrain.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(skinLocator);
            return hashCode;
        }
    }

    internal class ItemInfo
    {
        internal string ItemName;
        internal string ItemAuthor;
        internal byte RotOffset;
        internal (sbyte x, sbyte y, sbyte z) Offset;
        internal float SmallYOffset;
        internal (byte x, byte z) BlockSize;

        internal List<Flag> Flags;
        internal List<Clip> Clips;
        internal List<Pylon> Pylons;

        public ItemInfo(string itemName, string itemAuthor, byte rotOffset, (sbyte x, sbyte y, sbyte z) offset, float smallY)
        {
            ItemName = itemName;
            ItemAuthor = itemAuthor;
            RotOffset = rotOffset;
            Offset = offset;
            SmallYOffset = smallY;
            BlockSize = (1, 1);

            Flags = new List<Flag>();
            Clips = new List<Clip>();
            Pylons = new List<Pylon>();
        }

        internal void SetBlockSize(byte blockXSize, byte blockZSize)
        {
            BlockSize.x = blockXSize;
            BlockSize.z = blockZSize;
        }

        internal void PlaceRelToBlock(GBXFile file, Block reference, Challenge03043040 itemChunk, GBXLBS collection, string defaultAuthor)
        {
            var blockRot = reference.Rot.Value;
            var adjustedCoords = ApplyBlockOffset(blockRot, reference.Coords.Value);
            PlaceWithOffset(file, adjustedCoords, blockRot, itemChunk, collection, defaultAuthor);
        }

        internal void PlaceWithOffset(GBXFile file, (byte x, byte y, byte z) coords, byte rot, Challenge03043040 itemChunk, GBXLBS collection, string defaultAuthor)
        {
            var adjustedCoords = ApplyOffset(rot, coords);
            PlaceWithRotOffset(file, adjustedCoords, rot, itemChunk, collection, defaultAuthor);
        }

        internal void PlaceWithRotOffset(GBXFile file, (byte x, byte y, byte z) coords, byte rot, Challenge03043040 itemChunk, GBXLBS collection, string defaultAuthor)
        {
            var adjustedRot = (byte)((rot + RotOffset) % 4);
            PlaceAt(file, coords, adjustedRot, itemChunk, collection, defaultAuthor);
        }

        internal void PlaceAt(GBXFile file, (byte x, byte y, byte z) coords, byte rot, Challenge03043040 itemChunk, GBXLBS collection, string defaultAuthor)
        {
            var itemCoords = file.ConvertCoords(coords, SmallYOffset);
            var author = ItemAuthor ?? defaultAuthor;

            //Creating Item
            itemChunk?.AddItem(
                new Meta(new GBXLBS(ItemName), (GBXLBS)collection.DeepClone(), new GBXLBS(author)),
                GBXFile.ConvertRot(rot),
                new GBXByte3(coords),
                itemCoords
            );

            file.SetFlags(Flags, coords.x, coords.z, rot);
            file.AddClips(Clips, coords.x, coords.y, coords.z, rot);
            file.AddPylons(Pylons, coords.x, coords.y, coords.z, rot);
        }

        internal (byte x, byte y, byte z) ApplyBlockOffset(byte rot, (byte x, byte y, byte z) coords)
        {
            int xOffset;
            int zOffset;

            switch (rot)
            {
                case 0:
                    xOffset = 0;
                    zOffset = 0;
                    break;
                case 1:
                    xOffset = BlockSize.z - 1;
                    zOffset = 0;
                    break;
                case 2:
                    xOffset = BlockSize.x - 1;
                    zOffset = BlockSize.z - 1;
                    break;
                case 3:
                    xOffset = 0;
                    zOffset = BlockSize.x - 1;
                    break;
                default: throw new InternalException();
            }

            return ((byte)(coords.x + xOffset), (byte)(coords.y), (byte)(coords.z + zOffset));
        }

        internal (byte x, byte y, byte z) ApplyOffset(byte rot, (byte x, byte y, byte z) coords)
        {
            int xOffset;
            int zOffset;

            switch (rot)
            {
                case 0:
                    xOffset = Offset.x;
                    zOffset = Offset.z;
                    break;
                case 1:
                    xOffset = -Offset.z;
                    zOffset = Offset.x;
                    break;
                case 2:
                    xOffset = -Offset.x;
                    zOffset = -Offset.z;
                    break;
                case 3:
                    xOffset = Offset.z;
                    zOffset = -Offset.x;
                    break;
                default: throw new InternalException();
            }

            return ((byte)(coords.x + xOffset), (byte)(coords.y + Offset.y), (byte)(coords.z + zOffset));
        }
    }

    public enum Type
    {
        [XmlEnum(Name = "Air")]
        Air,
        [XmlEnum(Name = "Gnd")]
        Ground,
        [XmlEnum(Name = "Pri")]
        GroundPrimary,
        [XmlEnum(Name = "Sec")]
        GroundSecondary
    }

    public class AlternativeName
    {
        [XmlAttribute]
        public string BlockName;

        [XmlIgnore]
        public bool? PriSecTerrain;

        [XmlAttribute]
        public bool SecondaryTerrain
        {
            set => PriSecTerrain = value;
            get => (bool)PriSecTerrain;
        }
    }
}