using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using gbx.parser.visitor;

namespace gbx.parser.core
{
    /// <summary>
    /// This class serves as a wrapper around the leaf elements of the GbxComponentTree.
    /// It provides methods for converting this primitive from and to strings and streams.
    /// It also provides a ValueChecker mechanism, to avoid setting the GbxPrimitive to wrong values.
    /// </summary>
    /// <typeparam name="T">The primitive Type. Ideally a value type or immutable</typeparam>
    public abstract class GbxPrimitive<T> : GbxSerializable where T : IEquatable<T>
    {
        private readonly ControlledVar<T> _var;

        public T Value
        {
            get => _var.GetValue();
            set => _var.SetValue(value, NotifyChange);
        }

        protected GbxPrimitive(T initialValue, Func<T, bool>? constraint = null)
        {
            _var = new ControlledVar<T>(initialValue, constraint);
        }

        public bool TrySetValue(T value)
        {
            return _var.TrySetValue(value, NotifyChange);
        }

        public void MakeConst()
        {
            _var.MakeConst();
        }

        public bool IsConst()
        {
            return _var.IsConst();
        }

        public void MakeConstAfterNextSet()
        {
            OnChange += MakeConstOnChange;

            void MakeConstOnChange(IGbxComponent dummy)
            {
                MakeConst();
                //Remove eventhandler from event so that it only executes once.
                OnChange -= MakeConstOnChange;
            }
        }

        public override string ToString() => _var.ToString();

        public override abstract void FromString(string value);

        public override abstract void ToStream(BinaryWriter writer);

        public override abstract void FromStream(BinaryReader reader);

        public override TOut Accept<TIn, TOut>(InOutVisitor<TIn, TOut> visitor, TIn arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}
