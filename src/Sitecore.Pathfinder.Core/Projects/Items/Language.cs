// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    [DebuggerDisplay("Language: {LanguageName}"), TypeConverter(typeof(StringToLanguageConverter))]
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

        private class StringToLanguageConverter : TypeConverter
        {
            public override bool CanConvertFrom([NotNull] ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return value != null ? new Language(value.ToString()) : base.ConvertFrom(context, culture, value);
            }
        }
    }
}
