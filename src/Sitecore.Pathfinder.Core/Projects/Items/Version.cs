// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    [DebuggerDisplay("Version: {Number}")]
    public class Version
    {
        [NotNull]
        public static Version Latest = new Version(int.MaxValue);

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

        public override int GetHashCode() => Number;

        public static bool operator ==([CanBeNull] Version left, [CanBeNull] Version right) => Equals(left, right);

        public static bool operator !=([CanBeNull] Version left, [CanBeNull] Version right) => !Equals(left, right);

        public override string ToString() => Number.ToString();

        public static bool TryParse([NotNull] string value, [NotNull] out Version version)
        {
            if (!int.TryParse(value, out int number))
            {
                version = Undefined;
                return false;
            }

            version = new Version(number);
            return true;
        }

        protected bool Equals([NotNull] Version other) => Number == other.Number;
    }
}
