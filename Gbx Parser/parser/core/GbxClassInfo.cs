using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.core
{
    public class GbxClassInfo
    {
        internal const uint ClassMask = 0xFFFFF000;
        internal const uint ChunkMask = 0x00000FFF;

        private static readonly Dictionary<uint, GbxClassInfo> _classDict;

        static GbxClassInfo()
        {
            _classDict = new Dictionary<uint, GbxClassInfo>();
        }

        public static GbxChunk GetChunk(uint chunkID)
        {
            return GetClassInfo(chunkID).GetChunkInfo(chunkID).Constructor(chunkID);
        }

        private static GbxClassInfo GetClassInfo(uint classID)
        {
            var key = classID & ClassMask;
            return _classDict[key];
        }

        public static void AddClass(GbxClassInfo classInfo, uint classPrefix)
        {
            //TODO add exception
            _classDict.Add(classPrefix & ClassMask, classInfo);
        }

        public GbxClassInfo? Parent { get; }
        private readonly Dictionary<uint, GbxChunkInfo> _chunkDict;

        public GbxClassInfo()
        {
            _chunkDict = new Dictionary<uint, GbxChunkInfo>();
        }

        private GbxChunkInfo GetChunkInfo(uint chunkID)
        {
            var key = chunkID & ChunkMask;
            if (_chunkDict.ContainsKey(chunkID))
                return _chunkDict[key];
            else if (Parent != null)
                return Parent.GetChunkInfo(chunkID);
            else
                //TODO exception
                throw new Exception();
        }
        
        public void AddChunkInfo(GbxChunkInfo chunkInfo, uint chunkSuffix)
        {
            _chunkDict.Add(chunkSuffix & ChunkMask, chunkInfo);
        }
    }

    public class GbxChunkInfo
    {
        public readonly bool IsHeaderChunk;
        public readonly bool IsSkippable;
        public readonly GbxChunkConstructor Constructor;

        public GbxChunkInfo(bool isHeaderChunk, bool isSkippable, GbxChunkConstructor constructor)
        {
            IsHeaderChunk = isHeaderChunk;
            IsSkippable = isSkippable;
            Constructor = constructor;
        }
    }

    public delegate GbxChunk GbxChunkConstructor(uint chunkID);
}
