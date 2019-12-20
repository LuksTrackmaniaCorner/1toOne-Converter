﻿using Gbx.Parser.info;
using Gbx.Parser.Visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Core
{
    public class GbxNodeReference : GbxComposite<GbxNode>
    {
        public GbxNode? Node { get; private set; }
        public GbxClassInfo? NodeClassInfo { get; }

        public GbxNodeReference(GbxClassInfo? nodeClassInfo = null)
        {
            NodeClassInfo = nodeClassInfo;
        }

        public void SetNode(GbxNode newNode)
        {
            if (NodeClassInfo != null && NodeClassInfo != newNode.ClassInfo)
                throw new ArgumentException("This node has an incorrect type.");

            //Set Node
            if (Node is GbxNode oldNode)
                oldNode.RemoveNodeReference(this);

            newNode.AddNodeReference(this);
            Node = newNode;
        }

        public void RemoveNode()
        {
            if (Node is GbxNode oldNode)
                oldNode.RemoveNodeReference(this);

            Node = null;
        }

        public override IEnumerable<GbxNode> GetChildren()
        {
            if (Node != null)
                yield return Node;
        }

        public override IEnumerable<(string, GbxNode)> GetNamedChildren()
        {
            if (Node != null)
                yield return (nameof(Node), Node);
        }

        internal override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
