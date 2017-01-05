// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.JsonConfigurationSource
// Assembly: Microsoft.Framework.ConfigurationModel.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 29E3F8BD-4D3C-4C9D-8840-A11A97E69911
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Json.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  public class JsonConfigurationSource : ConfigurationSource
  {
    public bool Optional { get; private set; }

    public string Path { get; private set; }

    public JsonConfigurationSource(string path)
      : this(path, false)
    {
    }

    public JsonConfigurationSource(string path, bool optional)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(Microsoft.Framework.ConfigurationModel.Json.Resources.Error_InvalidFilePath, "path");
      this.Optional = optional;
      this.Path = JsonPathResolver.ResolveAppRelativePath(path);
    }

    public override void Load()
    {
      if (!File.Exists(this.Path))
      {
        if (!this.Optional)
          throw new FileNotFoundException(string.Format(Microsoft.Framework.ConfigurationModel.Json.Resources.Error_FileNotFound, (object) this.Path), this.Path);
        this.Data = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      else
      {
        using (FileStream fileStream = new FileStream(this.Path, FileMode.Open, FileAccess.Read))
          this.Load((Stream) fileStream);
      }
    }

    internal void Load(Stream stream)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (JsonTextReader jsonTextReader = new JsonTextReader((TextReader) new StreamReader(stream)))
      {
        int num = 0;
        jsonTextReader.DateParseHandling = DateParseHandling.None;
        jsonTextReader.Read();
        this.SkipComments((JsonReader) jsonTextReader);
        if (jsonTextReader.TokenType != JsonToken.StartObject)
          throw new FormatException(Microsoft.Framework.ConfigurationModel.Json.Resources.FormatError_RootMustBeAnObject((object) jsonTextReader.Path, (object) jsonTextReader.LineNumber, (object) jsonTextReader.LinePosition));
        do
        {
          this.SkipComments((JsonReader) jsonTextReader);
          switch (jsonTextReader.TokenType)
          {
            case JsonToken.None:
              throw new FormatException(Microsoft.Framework.ConfigurationModel.Json.Resources.FormatError_UnexpectedEnd((object) jsonTextReader.Path, (object) jsonTextReader.LineNumber, (object) jsonTextReader.LinePosition));
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
              string key = this.GetKey(jsonTextReader.Path);
              if (dictionary.ContainsKey(key))
                throw new FormatException(Microsoft.Framework.ConfigurationModel.Json.Resources.FormatError_KeyIsDuplicated((object) key));
              dictionary[key] = jsonTextReader.Value.ToString();
              goto case JsonToken.PropertyName;
            case JsonToken.EndObject:
              --num;
              goto case JsonToken.PropertyName;
            default:
              throw new FormatException(Microsoft.Framework.ConfigurationModel.Json.Resources.FormatError_UnsupportedJSONToken((object) jsonTextReader.TokenType, (object) jsonTextReader.Path, (object) jsonTextReader.LineNumber, (object) jsonTextReader.LinePosition));
          }
        }
        while (num > 0);
      }
      this.Data = (IDictionary<string, string>) dictionary;
    }

    private string GetKey(string jsonPath)
    {
      List<string> stringList = new List<string>();
      int num;
      for (int startIndex1 = 0; startIndex1 < jsonPath.Length; startIndex1 = num + 2)
      {
        int startIndex2 = jsonPath.IndexOf("['", startIndex1);
        if (startIndex2 < 0)
        {
          stringList.Add(jsonPath.Substring(startIndex1).Replace('.', ':'));
          break;
        }
        if (startIndex2 > startIndex1)
          stringList.Add(jsonPath.Substring(startIndex1, startIndex2 - startIndex1).Replace('.', ':'));
        num = jsonPath.IndexOf("']", startIndex2);
        stringList.Add(jsonPath.Substring(startIndex2 + 2, num - startIndex2 - 2));
      }
      return string.Join(string.Empty, (IEnumerable<string>) stringList);
    }

    private void SkipComments(JsonReader reader)
    {
      while (reader.TokenType == JsonToken.Comment)
        reader.Read();
    }
  }
}
