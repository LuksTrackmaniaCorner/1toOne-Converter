using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace gbx.parser.info
{
    public sealed class GbxClassInfo : IEquatable<GbxClassInfo>
    {
        public uint ClassID { get; }
        public string Description { get; }
        public GbxClassInfo? Parent { get; }
        private readonly Dictionary<uint, GbxChunkInfo> _chunkDict;

        public GbxClassInfo(uint classID, string description, GbxClassInfo? parent = null)
        {
            //TODO masking
            _chunkDict = new Dictionary<uint, GbxChunkInfo>();
            Description = description;
            ClassID = classID & GbxInfo.ClassMask;
            Parent = parent;
        }

        /// <summary>
        /// This constructor is used to create an alias for another GbxClassInfo object
        /// </summary>
        /// <param name="classID">The new class ID for the other GbxClassInfo</param>
        /// <param name="other">The GbxClassInfo for which an alias should be created</param>
        public GbxClassInfo(uint classID, GbxClassInfo other)
        {
            //TODO masking
            //Shallow copy, an alias
            ClassID = classID & GbxInfo.ClassMask;
            this._chunkDict = other._chunkDict;
            this.Description = other.Description;
            this.Parent = other.Parent;
        }

        public GbxChunkInfo GetChunkInfo(uint chunkID)
        {
            var key = chunkID & GbxInfo.ChunkMask;
            if (_chunkDict.ContainsKey(chunkID))
                return _chunkDict[key];
            else if (Parent != null)
                return Parent.GetChunkInfo(chunkID);
            else
                //TODO exception
                throw new Exception();
        }

        public void Add(GbxChunkInfo chunkInfo)
        {
            //TODO Exception
            _chunkDict.Add(chunkInfo.ChunkID, chunkInfo);
        }

        public bool CanContain(GbxChunkInfo chunkInfo)
        {
            var chunkID = chunkInfo.ChunkID;

            if (_chunkDict.ContainsKey(chunkID))
                return object.ReferenceEquals(chunkInfo, _chunkDict[chunkID]);

            if (Parent != null)
                return Parent.CanContain(chunkInfo);

            //chunkid not found in this classinfo or its parents.
            return false;
        }

        public bool Equals([AllowNull] GbxClassInfo other)
        {
            return object.ReferenceEquals(this._chunkDict, other._chunkDict);
        }
    }
}
