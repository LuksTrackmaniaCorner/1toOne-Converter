using Gbx.Parser.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Visit
{
    public abstract class Visitor
    {
        protected void Dispatch(GbxComponent component)
        {
            component.Accept(this);
        }

        protected internal abstract void Visit(GbxComponent component);

        protected internal virtual void Visit(GbxLeaf leaf) => Visit((GbxComponent)leaf);

        protected internal virtual void Visit<T>(GbxPrimitive<T> primitive) where T : IEquatable<T> => Visit((GbxLeaf)primitive);

        protected internal virtual void Visit<T>(GbxComposite<T> composite) where T : GbxComponent => Visit((GbxComponent)composite);

        protected internal virtual void Visit(GbxNode node) => Visit((GbxComposite<GbxChunk>)node);

        protected internal virtual void Visit(GbxNodeReference nodeReference) => Visit((GbxComposite<GbxNode>)nodeReference);

        protected internal virtual void Visit(GbxLookBackString lookBackString) => Visit((GbxComposite<GbxLeaf>)lookBackString);

        protected internal virtual void Visit<T>(GbxArray<T> array) where T : GbxComponent => Visit((GbxComposite<T>)array);

        //protected internal virtual void Visit<T>(GbxMeta meta) where T : IEquatable<T> => Visit((GbxComposite<GbxLookBackString>)meta);

        protected internal virtual void Visit<T>(GbxPrimitive2<T> primitive2) where T : IEquatable<T> => Visit((GbxComposite<GbxPrimitive<T>>)primitive2);

        protected internal virtual void Visit<T>(GbxPrimitive3<T> primitive3) where T : IEquatable<T> => Visit((GbxComposite<GbxPrimitive<T>>)primitive3);

        protected internal virtual void Visit(GbxStructure structure) => Visit((GbxComposite<GbxComponent>)structure);

        protected internal virtual void Visit(GbxChunk chunk) => Visit((GbxStructure)chunk);
    }
}
