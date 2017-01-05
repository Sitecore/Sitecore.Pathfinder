// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Json
{
    public class JsonConfigurationSource : ConfigurationSource
    {
        public JsonConfigurationSource(string path) : this(path, false)
        {
        }

        public JsonConfigurationSource(string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Json.Resources.Error_InvalidFilePath, "path");
            }
            Optional = optional;
            Path = JsonPathResolver.ResolveAppRelativePath(path);
        }

        public bool Optional { get; private set; }

        public string Path { get; private set; }

        public override void Load()
        {
            if (!File.Exists(Path))
            {
                if (!Optional)
                {
                    throw new FileNotFoundException(string.Format(Json.Resources.Error_FileNotFound, Path), Path);
                }
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                using (var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read))
                {
                    Load(fileStream);
                }
            }
        }

        internal void Load(Stream stream)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var jsonTextReader = new JsonTextReader(new StreamReader(stream)))
            {
                var num = 0;
                jsonTextReader.DateParseHandling = DateParseHandling.None;
                jsonTextReader.Read();
                SkipComments(jsonTextReader);
                if (jsonTextReader.TokenType != JsonToken.StartObject)
                {
                    throw new FormatException(Json.Resources.FormatError_RootMustBeAnObject(jsonTextReader.Path, jsonTextReader.LineNumber, jsonTextReader.LinePosition));
                }
                do
                {
                    SkipComments(jsonTextReader);
                    switch (jsonTextReader.TokenType)
                    {
                        case JsonToken.None:
                            throw new FormatException(Json.Resources.FormatError_UnexpectedEnd(jsonTextReader.Path, jsonTextReader.LineNumber, jsonTextReader.LinePosition));
                        case JsonToken.StartObject:
                            ++num;
                            goto case JsonToken.PropertyName;
                        case JsonToken.PropertyName:
                            jsonTextReader.Read();
                            continue;
                        case JsonToken.Raw:
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Null:
                        case JsonToken.Bytes:
                            var key = GetKey(jsonTextReader.Path);
                            if (dictionary.ContainsKey(key))
                            {
                                throw new FormatException(Json.Resources.FormatError_KeyIsDuplicated(key));
                            }
                            dictionary[key] = jsonTextReader.Value.ToString();
                            goto case JsonToken.PropertyName;
                        case JsonToken.EndObject:
                            --num;
                            goto case JsonToken.PropertyName;
                        default:
                            throw new FormatException(Json.Resources.FormatError_UnsupportedJSONToken(jsonTextReader.TokenType, jsonTextReader.Path, jsonTextReader.LineNumber, jsonTextReader.LinePosition));
                    }
                }
                while (num > 0);
            }
            Data = dictionary;
        }

        private string GetKey(string jsonPath)
        {
            var stringList = new List<string>();
            int num;
            for (var startIndex1 = 0; startIndex1 < jsonPath.Length; startIndex1 = num + 2)
            {
                var startIndex2 = jsonPath.IndexOf("['", startIndex1);
                if (startIndex2 < 0)
                {
                    stringList.Add(jsonPath.Substring(startIndex1).Replace('.', ':'));
                    break;
                }
                if (startIndex2 > startIndex1)
                {
                    stringList.Add(jsonPath.Substring(startIndex1, startIndex2 - startIndex1).Replace('.', ':'));
                }
                num = jsonPath.IndexOf("']", startIndex2);
                stringList.Add(jsonPath.Substring(startIndex2 + 2, num - startIndex2 - 2));
            }
            return string.Join(string.Empty, stringList);
        }

        private void SkipComments(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }
        }
    }
}
