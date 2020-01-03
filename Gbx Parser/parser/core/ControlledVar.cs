﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Parser.Core
{
    internal sealed class ControlledVar<T> where T : IEquatable<T>
    {
        public static bool ConstConstraint(T dummy) => false;

        private Predicate<T>? _constraint;

        private T _value;

        public ControlledVar(T initialValue, Predicate<T>? constraint = null)
        {
            if (initialValue == null)
                throw new ArgumentNullException(nameof(initialValue));

            if (constraint != null && !constraint(initialValue))
                throw new Exception();

            _constraint = constraint;
            _value = initialValue;
        }

        public T GetValue()
        {
            return _value;
        }

        public void SetValue(T value, Action changeCallback)
        {
            if (!TrySetValue(value, changeCallback))
                throw new Exception("Setting the var to the desired value was not possible");
        }

        public bool TrySetValue(T value, Action changeCallback)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (_value.Equals(value))
                return true;

            if (_constraint == null || _constraint(value))
            {
                _value = value;
                changeCallback();
                return true;
            }

            return false;
        }

        public void MakeConst()
        {
            //No Check for overriding, because the new Constraint will be stricter than whatever was assigned before.
            _constraint = ConstConstraint;
        }

        public bool IsConst()
        {
            if (_constraint == null)
                return false;
            else
                return _constraint == ConstConstraint;
        }

        public override string ToString() => _value.ToString()!;
    }
}