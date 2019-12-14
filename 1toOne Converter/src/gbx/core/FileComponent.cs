using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using _1toOne_Converter.src.gbx.core.primitives;
using System.Xml.Serialization;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.chunks;

namespace _1toOne_Converter.src.gbx.core
{

    [XmlInclude(typeof(AnchoredObject03101002))]
    [XmlInclude(typeof(SkippedChunk))]
    public abstract class FileComponent : IDumpable
    {
        private FileComponent _parent;

        [XmlIgnore]
        public FileComponent Parent //Should be set whenever the element is added to the parent.
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    throw new Exception();
                _parent = value;
            }
        }

        public virtual GBXLBSContext LBSContext
        {
            get{ return Parent.LBSContext; }
        }

        public virtual GBXNodeRefList NodeRefList
        {
            get { return Parent.NodeRefList; }
        }

        public virtual FileComponent DeepClone()
        {
            throw new NotImplementedException("Operation not supported for type " + this.GetType());
        }

        //Writes the data which has been read by the constructor and eventually modified back to a file.
        public abstract void WriteBack(Stream s);

        public abstract LinkedList<string> Dump();
    }
}
