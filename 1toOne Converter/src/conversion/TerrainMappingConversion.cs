using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.util;

namespace _1toOne_Converter.src.conversion
{
    public class TerrainMappingConversion : Conversion
    {
        [XmlElement(IsNullable = false, ElementName = "Mapping")]
        public Mapping[] Mappings;

        private readonly Dictionary<TerrainBlock, (int mappingNr, TerrainType type)> _dict;

        public TerrainMappingConversion()
        {
            _dict = new Dictionary<TerrainBlock, (int mappingNr, TerrainType type)>();
        }

        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);

            var terrains = new TerrainType[Mappings.Length][,];

            //Building terrain map.
            var terrainblock = new TerrainBlock();
            foreach (var block in blockChunk.Blocks)
            {
                terrainblock.BlockName = block.BlockName.Content;
                terrainblock.Variant = (byte)(block.Flags.Value & 0x3F);

                if(!_dict.ContainsKey(terrainblock))
                    continue; //Block not relevant for any mapping

                var (mappingNr, type) = _dict[terrainblock];
                terrains[mappingNr] ??= new TerrainType[GBXFile.MaxMapXSize, GBXFile.MaxMapZSize];
                terrains[mappingNr][block.Coords.X, block.Coords.Z] = type;
            }

            //Mapping every mapping
            for(int i = 0; i < Mappings.Length; i++)
            {
                if (terrains[i] == null)
                    continue; //No mapping needed

                CreateMap(file, terrains[i], Mappings[i].Flag);
            }
        }

        private void CreateMap(GBXFile file, TerrainType[,] terrain, FlagName activeFlag)
        {
            for (byte row = 0; row < terrain.GetLength(1); row++)
            {
                IState state = new PrimaryState();

                for (byte column = 0; column < terrain.GetLength(0); column++)
                {
                    if (state.IsActiveState(terrain[row, column], ref state))
                    {
                        file.SetFlag(new Flag(activeFlag, row, column));
                        //Console.WriteLine("Terrain at {row}, {column}");
                    }
                }

                Trace.Assert(state is PrimaryState, "Error reading terrain");
            }
        }

        internal override void Initialize()
        {
            for(int i = 0; i < Mappings.Length; i++)
            {
                var mapping = Mappings[i];

                foreach(var convexBlock in mapping.ConvexBlocks)
                {
                    _dict.Add(convexBlock, (i, TerrainType.Convex));
                }

                foreach (var wallBlock in mapping.WallBlocks)
                {
                    _dict.Add(wallBlock, (i, TerrainType.Wall));
                }

                foreach (var concaveBlock in mapping.ConcaveBlocks)
                {
                    _dict.Add(concaveBlock, (i, TerrainType.Concave));
                }
            }
        }
    }

    public class Mapping
    {
        public TerrainBlock[] ConvexBlocks;
        public TerrainBlock[] WallBlocks;
        public TerrainBlock[] ConcaveBlocks;
        public FlagName Flag;
    }

    internal interface IState
    {
        bool IsActiveState(TerrainType type, ref IState successorState);
    }

    internal class PrimaryState : IState
    {
        public bool IsActiveState(TerrainType type, ref IState successorState)
        {
            switch (type)
            {
                case TerrainType.Standard:
                    return false;
                case TerrainType.Convex:
                    successorState = new BorderState();
                    return false;
                case TerrainType.Wall:
                    successorState = new SecondaryState();
                    return false;
                case TerrainType.Concave:
                    throw new Exception("Illegal Terrain");
            }
            return false;
        }
    }

    internal class SecondaryState : IState
    {
        public bool IsActiveState(TerrainType type, ref IState successorState)
        {
            switch (type)
            {
                case TerrainType.Standard:
                    return true;
                case TerrainType.Convex:
                    throw new Exception("Illegal Terrain");
                case TerrainType.Wall:
                    successorState = new PrimaryState();
                    return false;
                case TerrainType.Concave:
                    successorState = new BorderState();
                    return false;
            }
            return false;
        }
    }

    internal class BorderState : IState
    {
        public bool IsActiveState(TerrainType type, ref IState successorState)
        {
            switch (type)
            {
                case TerrainType.Standard:
                    throw new Exception("Illegal Terrain");
                case TerrainType.Convex:
                    successorState = new PrimaryState();
                    return false;
                case TerrainType.Wall:
                    return false;
                case TerrainType.Concave:
                    successorState = new SecondaryState();
                    return false;
            }
            return false;
        }
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
}