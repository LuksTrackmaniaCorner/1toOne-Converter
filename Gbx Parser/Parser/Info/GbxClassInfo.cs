using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gbx.Parser.Info
{
    public sealed class GbxClassInfo : IEquatable<GbxClassInfo>
    {
        public const uint NodID = 0x01001000;

        public static readonly GbxClassInfo Nod = new GbxClassInfo(NodID, "Root", Nod);

        //todo add file extension
        public uint ClassID { get; }
        public string Description { get; }
        public GbxClassInfo Parent { get; }
        private readonly Dictionary<uint, GbxChunkInfo> _chunkDict;

        public GbxClassInfo(uint classID, string description) : this(classID, description, Nod)
        {
        }

        public GbxClassInfo(uint classID, string description, GbxClassInfo parent)
        {
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
        internal GbxClassInfo(uint classID, GbxClassInfo other)
        {
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

        /// <summary>
        /// Tests if this Class can contain the chunk.
        /// The result depends on wether the chunkInfo has been added to this ClassInfo object
        /// or one of its parent.
        /// </summary>
        /// <param name="chunkInfo"></param>
        /// <returns></returns>
        public bool CanContain(GbxChunkInfo chunkInfo)
        {
            var chunkID = chunkInfo.ChunkID;

            if (_chunkDict.ContainsKey(chunkID))
                return object.ReferenceEquals(chunkInfo, _chunkDict[chunkID]);

            if (this != Nod) //To avoid infinte loops
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
