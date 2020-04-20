using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.Gbx.core.primitives
{
    public class GBXLBS : FileComponent , IEquatable<GBXLBS>
    {
        public const uint unassigned = 0xFFFFFFFF;

        [XmlAttribute]
        public uint CollectionID;
        [XmlAttribute]
        public string Content {
            get => _content?.Value;
            set
            {
                if (value == null)
                    _content = null;
                else if (_content == null)
                    _content = new GBXString(value);
                else
                    _content.Value = value;
            }
        }

        internal GBXString _content;

        internal GBXLBSContext _context;

        private GBXLBS()
        {

        }

        public GBXLBS(string s) => _content = new GBXString(s);

        internal GBXLBS(GBXLBSContext context)
        {
            _context = context;
        }

        public bool ShouldSerializeCollectionID() => CollectionID != 0;

        //public bool ShouldSerializeContent() => _content != null;

        public override LinkedList<string> Dump()
        {
            if(_content == null)
            {
                //TODO do display the name of the collection here instead.
                var result = new LinkedList<string>();
                if (CollectionID == unassigned)
                {
                    result.AddLast("Unassigned");
                }
                else
                {
                    result.AddLast(CollectionID.ToString());
                }
                return result;
            }
            else
            {
                return _content.Dump();
            }
        }

        public override FileComponent DeepClone()
        {
            return new GBXLBS
            {
                CollectionID = this.CollectionID,
                Content = this.Content
            };
        }

        public override void WriteBack(Stream s)
        {
            if (_context == null)
                _context = LBSContext;
            _context.WriteLookBackString(s, this);
        }

        public bool Equals(GBXLBS other)
        {
            if(this._content != null && other._content != null)
                return this._content.Equals(other._content);

            if (this._content == null && other._content == null)
                return this.CollectionID == other.CollectionID;

            return false;
        }
    }

    public class GBXLBSContext
    {
        private const uint LBSFlag = 1 << 30;
        private const uint LBSVersion = 3;
        private bool _versionHasBeenRead;
        private readonly List<GBXString> _storedStringsRead;
        private List<GBXString> _storedStringsWrite;

        public GBXLBSContext()
        {
            _storedStringsRead = new List<GBXString>();
        }

        public GBXLBS ReadLookBackString(Stream s)
        {
            if (!_versionHasBeenRead)
            {
                var version = s.ReadUInt();
                Trace.Assert(version == LBSVersion, "Unsupported Version of the LookBackString: " + version.ToString("X"));
                _versionHasBeenRead = true;
            }

            var index = new GBXUInt(s);

            uint id = index.Value;
            var lbs = new GBXLBS(this);

            if (id == GBXLBS.unassigned)
            {
                return lbs;
            }

            switch (id >> 30)
            {
                case 0b00:
                    lbs.CollectionID = id;
                    return lbs;
                case 0b01: //no difference found
                    goto case 0b10;
                case 0b10:
                    if ((id & 0x3FFF) == 0)
                    {
                        var newString = new GBXString(s);
                        _storedStringsRead.Add(newString);
                        lbs._content = newString;
                    }
                    else
                    {
                        lbs._content = _storedStringsRead[(int)(id & 0x3FFF) - 1];
                    }
                    return lbs;
                case 0b11: throw new Exception("Could not read LookBackString. Unknown Flags." + id);
                default: throw new Exception("Universe is broken, have you tried turning it off and on again?");
            }
        }

        public void ClearWriteList()
        {
            _storedStringsWrite = null;
        }

        internal void WriteLookBackString(Stream s, GBXLBS lookbackstring)
        {
            if(_storedStringsWrite == null) //First Lookbackstring encountered, need to write version
            {
                _storedStringsWrite = new List<GBXString>();
                s.WriteUInt(LBSVersion);
            }

            if(lookbackstring._content == null)
            {
                s.WriteUInt(lookbackstring.CollectionID);
            }
            else
            {
                var index = _storedStringsWrite.IndexOf(lookbackstring._content);

                if (index < 0) // not stored, add string to stringlist, write new string
                {
                    _storedStringsWrite.Add(lookbackstring._content);
                    s.WriteUInt(LBSFlag);
                    lookbackstring._content.WriteBack(s);
                }
                else //string stored, write reference
                {
                    s.WriteUInt(((uint)index | LBSFlag) + 1);
                }
            }
        }

        internal void SkipVersion()
        {
            if (_storedStringsWrite == null) //First Lookbackstring encountered, need to write version
            {
                _storedStringsWrite = new List<GBXString>();
                _versionHasBeenRead = true;
            }
        }
    }
}
