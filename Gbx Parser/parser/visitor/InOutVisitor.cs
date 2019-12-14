using gbx.parser.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace gbx.parser.visitor
{
    public abstract class InOutVisitor<TIn, TOut>
    {
        protected TOut Dispatch(IGbxComponent component, TIn arg)
        {
            return component.Accept(this, arg);
        }

        protected internal abstract TOut Visit(GbxComponent component, TIn arg);

        protected internal virtual TOut Visit(GbxLeaf leaf, TIn arg) => Visit((GbxComponent)leaf, arg);

        protected internal virtual TOut Visit(GbxSerializable serializable, TIn arg) => Visit((GbxLeaf)serializable, arg);

        protected internal virtual TOut Visit<T>(GbxPrimitive<T> primitive, TIn arg) where T : IEquatable<T> => Visit((GbxSerializable)primitive, arg);

        protected internal virtual TOut Visit(GbxLookBackString lookBackString, TIn arg) => Visit((GbxLeaf)lookBackString, arg);

        protected internal virtual TOut Visit<T>(GbxComposite<T> composite, TIn arg) where T : IGbxComponent => Visit((GbxComponent)composite, arg);

        protected internal virtual TOut Visit(GbxNode node, TIn arg) => Visit((GbxComposite<GbxChunk>)node, arg);

        protected internal virtual TOut Visit<T>(GbxArray<T> array, TIn arg) where T : IGbxComponent => Visit((GbxComposite<T>)array, arg);

        protected internal virtual TOut Visit<T>(GbxMeta meta, TIn arg) where T : IEquatable<T> => Visit((GbxComposite<GbxLookBackString>)meta, arg);

        protected internal virtual TOut Visit<T>(GbxPrimitive2<T> primitive2, TIn arg) where T : IEquatable<T> => Visit((GbxComposite<GbxPrimitive<T>>)primitive2, arg);

        protected internal virtual TOut Visit<T>(GbxPrimitive3<T> primitive3, TIn arg) where T : IEquatable<T> => Visit((GbxComposite<GbxPrimitive<T>>)primitive3, arg);

        protected internal virtual TOut Visit(GbxStructure structure, TIn arg) => Visit((GbxComposite<IGbxComponent>)structure, arg);

        protected internal virtual TOut Visit(GbxChunk chunk, TIn arg) => Visit((GbxStructure)chunk, arg);
    }
}
