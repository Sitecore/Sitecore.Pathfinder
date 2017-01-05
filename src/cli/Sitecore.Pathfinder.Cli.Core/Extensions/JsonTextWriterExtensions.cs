﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using Newtonsoft.Json;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class JsonTextWriterExtensions
    {
        public static void WriteObjectString([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName, [NotNull] string propertyName2, [NotNull] string value)
        {
            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteStartObject();

            jsonTextWriter.WritePropertyString(propertyName2, value);

            jsonTextWriter.WriteEndObject();
        }

        public static void WritePropertyString([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName, [NotNull] string value)
        {
            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteValue(value);
        }

        public static void WritePropertyStringIf([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName, [NotNull] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteValue(value);
        }

        public static void WritePropertyStringIf([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName, int value, int defaultValue = 0)
        {
            if (value == defaultValue)
            {
                return;
            }

            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteValue(value);
        }

        public static void WritePropertyStringIf([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName, bool value, bool defaultValue = false)
        {
            if (value == defaultValue)
            {
                return;
            }

            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteValue(value);
        }

        public static void WritePropertyString([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName, bool value)
        {
            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteValue(value);
        }

        public static void WriteStartArray([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName)
        {
            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteStartArray();
        }

        public static void WriteStartObject([NotNull] this JsonTextWriter jsonTextWriter, [NotNull] string propertyName)
        {
            jsonTextWriter.WritePropertyName(propertyName);
            jsonTextWriter.WriteStartObject();
        }
    }
}
