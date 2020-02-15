using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using _1toOne_Converter.src.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        internal List<BlockToItem> childrenList;

        [XmlElement(ElementName = "BlockData", Type = typeof(BlockData))]
        [XmlElement(ElementName = "BlockVariantData", Type = typeof(BlockVariantData))]
        [XmlElement(ElementName = "BlockTypeData", Type = typeof(BlockTypeData))]
        [XmlElement(ElementName = "BlockRandomData", Type = typeof(BlockRandomData))]
        [XmlElement(ElementName = "BlockSkinData", Type = typeof(BlockSkinData))]
        public BlockToItem[] Children { get => childrenList.ToArray(); set { if (value != null) childrenList = new List<BlockToItem>(value); } }
        [XmlElement(ElementName = "Flag")]
        public Flag[] Flags { get; set; }
        [XmlElement(ElementName = "Clip")]
        public Clip[] Clips { get; set; }
        [XmlElement(ElementName = "Pylon")]
        public MultiPylon[] MultiPylons { get; set; }

        //TODO remove childrenList once you serialized the newer version.
        internal BlockToItem() => childrenList = new List<BlockToItem>();

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
            if (childrenList.Count == 0)
                result = new ItemInfo(ItemName, ItemAuthor, RotOffset, (XOffset, YOffset, ZOffset));
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

        internal void Flatten()
        {
            foreach (var child in Children)
            {
                child.Flatten();
            }

            foreach (var child in Children)
            {
                if (child.CanFlatten() && child.childrenList.Count != 0)
                {
                    childrenList.Remove(child);
                    //Flatten Non-Leaf Node
                    foreach (var grandChild in child.Children)
                    {
                        grandChild.ItemAuthor ??= child.ItemAuthor;
                        grandChild.YOffset += child.YOffset;
                        grandChild.RotOffset += child.RotOffset;
                        grandChild.XOffset += child.XOffset;
                        grandChild.ZOffset += child.ZOffset;
                        if (child.Flags != null)
                            grandChild.Flags = grandChild.Flags == null ? child.Flags : grandChild.Flags.Union(child.Flags).ToArray();
                        if (child.Clips != null)
                            grandChild.Clips = grandChild.Clips == null ? child.Clips : grandChild.Clips.Union(child.Clips).ToArray();
                        if (child.MultiPylons != null)
                            grandChild.MultiPylons = grandChild.MultiPylons == null ? child.MultiPylons : grandChild.MultiPylons.Union(child.MultiPylons).ToArray();
                        childrenList.Add(grandChild);
                    }
                }
            }
        }

        internal virtual bool CanFlatten() => false;

        public bool ShouldSerializeYOffset() => YOffset != 0;
        public bool ShouldSerializeRotOffset() => RotOffset != 0;
        public bool ShouldSerializeXOffset() => XOffset != 0;
        public bool ShouldSerializeZOffset() => ZOffset != 0;

        public IEnumerator<BlockToItem> GetEnumerator()
        {
            //Returns all elements of the BlockToItem tree.
            yield return this;
            foreach(var child in childrenList)
            {
                foreach(var childEnum in child)
                {
                    yield return childEnum;
                }
            }
        }

        internal IEnumerator<Stack<BlockToItem>> GetTreeEnumerator()
        {
            //TODO Replace stack with a immutable data structure.
            var stack = new Stack<BlockToItem>();
            stack.Push(this);
            return GetTreeEnumerator(new Stack<BlockToItem>(stack));
        }

        private IEnumerator<Stack<BlockToItem>> GetTreeEnumerator(Stack<BlockToItem> stack)
        {
            yield return stack;

            foreach(var child in stack.Peek().childrenList)
            {
                stack.Push(child);
                foreach(var childEnum in child.GetTreeEnumerator(stack).AsEnumerable())
                {
                    yield return childEnum;
                }
                stack.Pop();
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
        public string SecondaryTerrainName { get; set; }

        [XmlAttribute]
        public byte BlockXSize { get; set; }
        [XmlAttribute]
        public byte BlockZSize { get; set; }

        internal void Initialize()
        {
            if (BlockXSize <= 0)
                BlockXSize = 1;
            if (BlockZSize <= 0)
                BlockZSize = 1;
        }

        internal override bool TestBlock(Identifier identifier)
        {
            if (SecondaryTerrainName == null)
            {
                return identifier.blockName == BlockName;
            }

            if (identifier.blockName == BlockName)
            {
                identifier.isSecondaryTerrain = false;
                return true;
            }
            if (identifier.blockName == SecondaryTerrainName)
            {
                identifier.isSecondaryTerrain = true;
                return true;
            }
            return false;
        }

        internal override ItemInfo GetItemInfo(Identifier identifier)
        {
            var result = base.GetItemInfo(identifier);
            result?.SetBlockSize(BlockXSize, BlockZSize);
            return result;
        }

        internal void MergeVariants()
        {
            for (int i = 0; i < childrenList.Count; i++)
            {
                for (int j = i + 1; j < childrenList.Count; j++)
                {
                    if (childrenList[i] is BlockVariantData v1 && childrenList[j] is BlockVariantData v2 && v1._variant == v2._variant)
                    {
                        //Merge variants.
                        foreach (var child2 in v2.Children)
                        {
                            child2.ItemAuthor ??= v2.ItemAuthor;
                            child2.YOffset += v2.YOffset;
                            child2.RotOffset += v2.RotOffset;
                            child2.XOffset += v2.XOffset;
                            child2.ZOffset += v2.ZOffset;
                            if (v2.Flags != null)
                                child2.Flags = child2.Flags == null ? v2.Flags : child2.Flags.Union(v2.Flags).ToArray();
                            if (v2.Clips != null)
                                child2.Clips = child2.Clips == null ? v2.Clips : child2.Clips.Union(v2.Clips).ToArray();
                            v1.childrenList.Add(child2);
                        }

                        this.childrenList.Remove(v2);
                    }
                }
            }

            Flatten();
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

        internal override bool CanFlatten() => _variant == null;

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

        internal override bool CanFlatten() => _type == null;

        public bool ShouldSerializeTypeOfBlock() => _type != null;
    }

    public class BlockRandomData : BlockToItem
    {
        internal byte? _variant;

        [XmlAttribute]
        public byte Variant { get => _variant.Value; set => _variant = value; }

        internal override bool TestBlock(Identifier identifier)
        {
            return ((identifier.flags >> 6) & 0x3F) == Variant;
        }

        internal override bool CanFlatten() => _variant == null;
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

    internal class Identifier
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
    }

    internal class ItemInfo
    {
        internal string ItemName;
        internal string ItemAuthor;
        internal byte RotOffset;
        internal (sbyte x, sbyte y, sbyte z) Offset;
        internal (byte x, byte z) BlockSize;

        internal List<Flag> Flags;
        internal List<Clip> Clips;
        internal List<Pylon> Pylons;

        public ItemInfo(string itemName, string itemAuthor, byte rotOffset, (sbyte x, sbyte y, sbyte z) offset)
        {
            ItemName = itemName;
            ItemAuthor = itemAuthor;
            RotOffset = rotOffset;
            Offset = offset;
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
            var itemCoords = file.ConvertCoords(coords);
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
}
