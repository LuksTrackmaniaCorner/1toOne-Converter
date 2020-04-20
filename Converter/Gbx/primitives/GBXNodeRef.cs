using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.primitives
{

    public class GBXNodeRef : FileComponent
    {
        internal readonly Node node;
        private readonly GBXNodeRefList _list;

        public override GBXNodeRefList NodeRefList => _list ?? Parent.NodeRefList;

        private GBXNodeRef()
        {
            //TODO make xml deserialization work, even when node is set;
        }

        internal GBXNodeRef(GBXNodeRefList list, Node node)
        {
            _list = list;
            this.node = node;
        }

        public override LinkedList<string> Dump()
        {
            if(node == null)
            {
                var result = new LinkedList<string>();
                result.AddLast("Empty Node");
                return result;
            }
            else
            {
                return node.Dump();
            }
        }

        public override void WriteBack(Stream s)
        {
            NodeRefList.WriteGBXNodeRef(s, this);
        }

        public override FileComponent DeepClone()
        {
            return new GBXNodeRef(null, (Node)node?.DeepClone());
        }
    }

    public class GBXNodeRefList
    {
        public const uint emptyNode = 0xFFFFFFFF;

        private readonly List<Node> _storedNodesRead;
        private List<Node> _storedNodesWrite;

        public GBXNodeRefList()
        {
            _storedNodesRead = new List<Node>();
        }

        public GBXNodeRef ReadGBXNodeRef(Stream s, GBXLBSContext context)
        {
            uint index = s.ReadUInt();

            if(index == emptyNode) //Empty NodeRef
            {
                return new GBXNodeRef(this, null);
            }

            //NodeRef contains node:
            Node node;

            try
            {
                node = _storedNodesRead[(int)index - 1];
            }
            catch(ArgumentOutOfRangeException)
            {
                node = new Node(s, context, this);
                _storedNodesRead.Add(node);
            }

            return new GBXNodeRef(this, node);
        }

        public void ClearWriteList()
        {
            _storedNodesWrite = null;
        }

        internal void WriteGBXNodeRef(Stream s, GBXNodeRef nodeRef)
        {
            if(_storedNodesWrite == null)
            {
                _storedNodesWrite = new List<Node>();
            }

            if(nodeRef.node == null)
            {
                s.WriteUInt(emptyNode);
                return;
            }

            var index = _storedNodesWrite.IndexOf(nodeRef.node);

            if(index < 0) //not stored, node needs to be stored
            {
                index = _storedNodesWrite.Count;
                _storedNodesWrite.Add(nodeRef.node);
                s.WriteUInt((uint) index + 1);
                nodeRef.node.WriteBack(s);
            }
            else //node already stored
            {
                s.WriteUInt((uint) index + 1);
            }
        }
    }
}
