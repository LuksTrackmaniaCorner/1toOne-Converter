using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Converter.Gbx;
using Converter.Gbx.chunks;
using Converter.Gbx.core;
using Converter.Gbx.core.chunks;
using Converter.Gbx.core.primitives;
using Converter.util;

namespace Converter.Converion
{
    public class TerrainMappingConversion : Conversion
    {
        public FlagName HeightFlag;
        public FlagName SecondaryTerrainFlag;
        public GBXByte BaseHeight;

        [XmlElement(IsNullable = false, ElementName = "Terrain")]
        public Terrain[] Terrains;

        private readonly Dictionary<TerrainBlock, (Terrain terrain, TerrainType type)> _dict;

        public TerrainMappingConversion()
        {
            _dict = new Dictionary<TerrainBlock, (Terrain terrain, TerrainType type)>();
        }

        public override void Convert(GBXFile file)
        {
            var mapper = new Mapper(file, this);
            mapper.CalculateHeightMap(this);
        }

        internal override void Initialize()
        {
            if (Terrains == null)
                Terrains = new Terrain[0];

            foreach(var terrain in Terrains)
            {
                foreach(var convexBlock in terrain.ConvexBlocks)
                {
                    _dict.Add(convexBlock, (terrain, TerrainType.Convex));
                }

                foreach (var wallBlock in terrain.WallBlocks)
                {
                    _dict.Add(wallBlock, (terrain, TerrainType.Wall));
                }

                foreach (var concaveBlock in terrain.ConcaveBlocks)
                {
                    _dict.Add(concaveBlock, (terrain, TerrainType.Concave));
                }
            }
        }

        internal class Mapper
        {
            private readonly GBXFile _file;
            private readonly ((Terrain terrain, byte height) block, TerrainType type)[,] _map;

            internal Mapper(GBXFile file, TerrainMappingConversion conversion)
            {
                _file = file;
                _map = new ((Terrain, byte), TerrainType)[GBXFile.MaxMapXSize, GBXFile.MaxMapZSize];

                var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
                var dict = conversion._dict;

                var terrainblock = new TerrainBlock();
                foreach (var block in blockChunk.Blocks)
                {
                    terrainblock.BlockName = block.BlockName.Content;
                    terrainblock.Variant = (byte)(block.Flags.Value & 0x3F);

                    if (!dict.ContainsKey(terrainblock))
                        continue; //Block not relevant for any mapping

                    var temp = dict[terrainblock];
                    _map[block.Coords.X, block.Coords.Z] = ((temp.terrain, block.Coords.Y), temp.type);
                }
            }

            internal void CalculateHeightMap(TerrainMappingConversion conversion)
            {
                var baseHeight = conversion.BaseHeight.Value;

                for (byte row = 0; row < _map.GetLength(1); row++)
                {
                    for (byte column = 0; column < _map.GetLength(0); column++)
                    {
                        while (_map[row, column].block.terrain != null)
                            CalculateHeightMap(conversion, baseHeight, row, ref column);

                        _file.SetFlag(new Flag(conversion.HeightFlag.Name, row, (byte) baseHeight, column));

                        Trace.Assert(column < _map.GetLength(0));
                    }
                }
            }

            private void CalculateHeightMap(TerrainMappingConversion conversion, int oldHeight, byte row, ref byte column)
            {
                IState state = PrimaryState.Instance;
                var (currentBlock, type) = _map[row, column];
                //The type of terrain handled in this call of the function
                var block = currentBlock;
                var terrain = block.terrain;
                var secondaryTerrain = terrain.SecondaryTerrain;
                var newHeight = oldHeight + terrain.Height;
                var includeEdges = terrain.Height < 0;

                while (true)
                {
                    //Field belongs to the terrain.
                    //Console.WriteLine($"{height} at {row}, {column}");
                    //Set Secondary Terrain
                    if (secondaryTerrain)
                        _file.SetFlag(new Flag(conversion.SecondaryTerrainFlag, row, column));
                    //Set Height
                    int thisHeight;
                    if (includeEdges || state.IsActive == true)
                        thisHeight = newHeight;
                    else
                        thisHeight = oldHeight;
                    
                    _file.SetFlag(new Flag(conversion.HeightFlag.Name, row, (byte) thisHeight, column));

                    //Move to next field
                    column++;
                    state = state.GetNext(type);

                    if (state.IsActive == false)
                        break;
                    
                    //Terrain not over
                    (currentBlock, type) = _map[row, column];

                    //Handle Terrain placed over this terrain, recursively
                    while (state.IsActive == true && currentBlock.terrain != null && currentBlock != block)
                    {
                        CalculateHeightMap(conversion, newHeight, row, ref column);
                        (currentBlock, type) = _map[row, column];
                    }
                }
            }
        }
    }

    public class Terrain
    {
        public TerrainBlock[] ConvexBlocks;
        public TerrainBlock[] WallBlocks;
        public TerrainBlock[] ConcaveBlocks;
        [XmlAttribute]
        public int Height;
        [XmlAttribute]
        public bool SecondaryTerrain; //TODO
    }

    public class TerrainBlock : IEquatable<TerrainBlock>
    {
        [XmlAttribute]
        public string BlockName;
        [XmlAttribute]
        public byte Variant;

        public override bool Equals(object obj)
        {
            return Equals(obj as TerrainBlock);
        }

        public bool Equals(TerrainBlock other)
        {
            return other != null &&
                   BlockName == other.BlockName &&
                   Variant == other.Variant;
        }

        public override int GetHashCode()
        {
            var hashCode = 883540855;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BlockName);
            hashCode = hashCode * -1521134295 + Variant.GetHashCode();
            return hashCode;
        }
    }

    internal enum TerrainType : byte
    {
        Standard,
        Convex,
        Wall,
        Concave
    }

    internal interface IState
    {
        IState GetNext(TerrainType terrainType);
        bool? IsActive { get; }
    }

    internal class PrimaryState : IState
    {
        public static PrimaryState Instance { get; } = new PrimaryState();

        public bool? IsActive => false;

        private PrimaryState() { }

        public IState GetNext(TerrainType terrainType)
        {
            return terrainType switch
            {
                TerrainType.Standard => PrimaryState.Instance,
                TerrainType.Convex => BorderState.Instance,
                TerrainType.Wall => SecondaryState.Instance,
                _ => throw new Exception("Illegal Terrain"),
            };
        }
    }

    internal class SecondaryState : IState
    {
        public static SecondaryState Instance { get; } = new SecondaryState();

        public bool? IsActive => true;

        private SecondaryState() { }

        public IState GetNext(TerrainType type)
        {
            return type switch
            {
                TerrainType.Standard => SecondaryState.Instance,
                TerrainType.Wall => PrimaryState.Instance,
                TerrainType.Concave => BorderState.Instance,
                _ => throw new Exception("Illegal Terrain"),
            };
        }
    }

    internal class BorderState : IState
    {
        public static BorderState Instance { get; } = new BorderState();

        public bool? IsActive => null;

        private BorderState() { }

        public IState GetNext(TerrainType type)
        {
            return type switch
            {
                TerrainType.Convex => PrimaryState.Instance,
                TerrainType.Wall => BorderState.Instance,
                TerrainType.Concave => SecondaryState.Instance,
                _ => throw new Exception("Illegal Terrain"),
            };
        }
    }
}