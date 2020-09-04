using Digify.Micro.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digify.Micro
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public bool Equals(ValueObject other)
        {
            return Equals((object)other);
        }

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType()) return false;
            return EquatableHelper.PropertiesEquals(this, other);
        }

        public override int GetHashCode()
        {
            return EquatableHelper.PropertiesGetHashCode(this);
        }

        public static bool operator ==(ValueObject x, ValueObject y)
        {
            return EquatableHelper.PropertiesEquals(x, y);
        }

        public static bool operator !=(ValueObject x, ValueObject y)
        {
            return !(x == y);
        }
    }
}
