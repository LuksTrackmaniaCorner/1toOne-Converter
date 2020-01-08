using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Info
{
    public static class GbxInfo
    {
        internal const uint ClassMask = 0xFFFFF000;
        internal const uint ChunkMask = ~ClassMask;

        private static readonly Dictionary<uint, GbxClassInfo> _classDict;

        static GbxInfo()
        {
            _classDict = new Dictionary<uint, GbxClassInfo>();
        }

        public static GbxChunk CreateChunk(uint chunkID)
        {
            return GetChunkInfo(chunkID).CreateChunk();
        }

        public static GbxChunkInfo GetChunkInfo(uint chunkID)
        {
            return GetClassInfo(chunkID).GetChunkInfo(chunkID);
        }

        public static GbxClassInfo GetClassInfo(uint classID)
        {
            var key = classID & ClassMask;
            return _classDict[key];
        }

        public static void Add(GbxClassInfo classInfo)
        {
            //TODO add exception
            _classDict.Add(classInfo.ClassID, classInfo);
        }

        public static void AddAlias(GbxClassInfo classInfo, uint newAlias)
        {
            var alias = new GbxClassInfo(newAlias, classInfo);

            Add(alias);
        }
    }
}
