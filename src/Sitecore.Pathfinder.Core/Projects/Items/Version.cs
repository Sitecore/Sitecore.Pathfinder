// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    [DebuggerDisplay("Version: {Number}")]
    public class Version
    {
        [NotNull]
        public static Version Latest = new Version(0);

        [NotNull]
        public static Version Undefined = new Version(int.MinValue);

        public Version(int number)
        {
            Number = number;
        }

        public int Number { get; }

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

            return Equals((Version)obj);
        }

        public override int GetHashCode()
        {
            return Number;
        }

        public static bool operator ==([CanBeNull] Version left, [CanBeNull] Version right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] Version left, [CanBeNull] Version right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Number.ToString();
        }

        public static bool TryParse([NotNull] string value, [NotNull] out Version version)
        {
            int number;
            if (!int.TryParse(value, out number))
            {
                version = Undefined;
                return false;
            }

            version = new Version(number);
            return true;
        }

        protected bool Equals([NotNull] Version other)
        {
            return Number == other.Number;
        }
    }
}
