// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    [DebuggerDisplay("Language: {{LanguageName}}")]
    public class Language
    {
        [NotNull]
        public static Language Empty = new Language(string.Empty);

        [NotNull]
        public static Language Undefined = new Language("[Undefined]");

        public Language([NotNull] string languageName)
        {
            LanguageName = languageName;
        }

        [NotNull]
        public string LanguageName { get; }

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

            return Equals((Language)obj);
        }

        public override int GetHashCode()
        {
            return LanguageName.GetHashCode();
        }

        public static bool operator ==([CanBeNull] Language left, [CanBeNull] Language right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] Language left, [CanBeNull] Language right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return LanguageName;
        }

        protected bool Equals([NotNull] Language other)
        {
            return string.Equals(LanguageName, other.LanguageName);
        }
    }
}
