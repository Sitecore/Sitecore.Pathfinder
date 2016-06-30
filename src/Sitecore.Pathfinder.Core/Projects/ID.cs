// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public class ID
    {
        private readonly Guid _guid;

        public ID(Guid guid)
        {
            _guid = guid;
        }

        public ID([NotNull] string id)
        {
            Guid.TryParse(id, out _guid);
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ID)obj);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        public static bool operator ==([CanBeNull] ID left, [CanBeNull] ID right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] ID left, [CanBeNull] ID right)
        {
            return !Equals(left, right);
        }

        public Guid ToGuid()
        {
            return _guid;
        }

        public override string ToString()
        {
            return _guid.ToString("B").ToUpperInvariant();
        }

        protected bool Equals([NotNull] ID other)
        {
            return _guid.Equals(other._guid);
        }
    }
}
