// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    // ReSharper disable once InconsistentNaming
    public class ID
    {
        private readonly Guid _guid;

        public ID(Guid guid) => _guid = guid;

        public ID([NotNull] string id) => Guid.TryParse(id, out _guid);

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

        public override int GetHashCode() => _guid.GetHashCode();

        public static bool operator ==([CanBeNull] ID left, [CanBeNull] ID right) => Equals(left, right);

        public static bool operator !=([CanBeNull] ID left, [CanBeNull] ID right) => !Equals(left, right);

        public Guid ToGuid() => _guid;

        public override string ToString() => _guid.ToString("B").ToUpperInvariant();

        protected bool Equals([NotNull] ID other) => _guid.Equals(other._guid);
    }
}
