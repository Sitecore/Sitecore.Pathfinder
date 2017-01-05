// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.XmlConfigurationSource
// Assembly: Microsoft.Framework.ConfigurationModel.Xml, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34572FA8-986C-4AAC-914F-69C1CDA5880E
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Xml.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.Framework.ConfigurationModel
{
  public class XmlConfigurationSource : ConfigurationSource
  {
    private const string NameAttributeKey = "Name";
    private readonly XmlDocumentDecryptor _xmlDocumentDecryptor;

    public bool Optional { get; private set; }

    public string Path { get; private set; }

    public XmlConfigurationSource(string path)
      : this(path, (XmlDocumentDecryptor) null, false)
    {
    }

    internal XmlConfigurationSource(string path, bool optional)
      : this(path, (XmlDocumentDecryptor) null, optional)
    {
    }

    internal XmlConfigurationSource(string path, XmlDocumentDecryptor xmlDocumentDecryptor)
      : this(path, xmlDocumentDecryptor, false)
    {
    }

    internal XmlConfigurationSource(string path, XmlDocumentDecryptor xmlDocumentDecryptor, bool optional)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(Microsoft.Framework.ConfigurationModel.Xml.Resources.Error_InvalidFilePath, "path");
      this.Optional = optional;
      this.Path = XmlPathResolver.ResolveAppRelativePath(path);
      this._xmlDocumentDecryptor = xmlDocumentDecryptor ?? XmlDocumentDecryptor.Instance;
    }

    public override void Load()
    {
      if (!File.Exists(this.Path))
      {
        if (!this.Optional)
          throw new FileNotFoundException(string.Format(Microsoft.Framework.ConfigurationModel.Xml.Resources.Error_FileNotFound, (object) this.Path), this.Path);
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
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        CloseInput = false,
        DtdProcessing = DtdProcessing.Prohibit,
        IgnoreComments = true,
        IgnoreWhitespace = true
      };
      using (XmlReader decryptingXmlReader = this._xmlDocumentDecryptor.CreateDecryptingXmlReader(stream, settings))
      {
        Stack<string> stringStack = new Stack<string>();
        this.SkipUntilRootElement(decryptingXmlReader);
        this.ProcessAttributes(decryptingXmlReader, stringStack, (IDictionary<string, string>) dictionary, new Action<XmlReader, Stack<string>, IDictionary<string, string>, XmlWriter>(XmlConfigurationSource.AddNamePrefix), (XmlWriter) null);
        this.ProcessAttributes(decryptingXmlReader, stringStack, (IDictionary<string, string>) dictionary, new Action<XmlReader, Stack<string>, IDictionary<string, string>, XmlWriter>(XmlConfigurationSource.AddAttributePair), (XmlWriter) null);
        XmlNodeType xmlNodeType = decryptingXmlReader.NodeType;
        while (decryptingXmlReader.Read())
        {
          switch (decryptingXmlReader.NodeType)
          {
            case XmlNodeType.Element:
              stringStack.Push(decryptingXmlReader.LocalName);
              this.ProcessAttributes(decryptingXmlReader, stringStack, (IDictionary<string, string>) dictionary, new Action<XmlReader, Stack<string>, IDictionary<string, string>, XmlWriter>(XmlConfigurationSource.AddNamePrefix), (XmlWriter) null);
              this.ProcessAttributes(decryptingXmlReader, stringStack, (IDictionary<string, string>) dictionary, new Action<XmlReader, Stack<string>, IDictionary<string, string>, XmlWriter>(XmlConfigurationSource.AddAttributePair), (XmlWriter) null);
              if (decryptingXmlReader.IsEmptyElement)
              {
                stringStack.Pop();
                goto case XmlNodeType.ProcessingInstruction;
              }
              else
                goto case XmlNodeType.ProcessingInstruction;
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
              string key = string.Join(Constants.KeyDelimiter, stringStack.Reverse<string>());
              if (dictionary.ContainsKey(key))
                throw new FormatException(Microsoft.Framework.ConfigurationModel.Xml.Resources.FormatError_KeyIsDuplicated((object) key, (object) XmlConfigurationSource.GetLineInfo(decryptingXmlReader)));
              dictionary[key] = decryptingXmlReader.Value;
              goto case XmlNodeType.ProcessingInstruction;
            case XmlNodeType.ProcessingInstruction:
            case XmlNodeType.Comment:
            case XmlNodeType.Whitespace:
            case XmlNodeType.XmlDeclaration:
              xmlNodeType = decryptingXmlReader.NodeType;
              if (xmlNodeType == XmlNodeType.Element && decryptingXmlReader.IsEmptyElement)
              {
                xmlNodeType = XmlNodeType.EndElement;
                continue;
              }
              continue;
            case XmlNodeType.EndElement:
              if (stringStack.Any<string>())
              {
                if (xmlNodeType == XmlNodeType.Element)
                {
                  string index = string.Join(Constants.KeyDelimiter, stringStack.Reverse<string>());
                  dictionary[index] = string.Empty;
                }
                stringStack.Pop();
                goto case XmlNodeType.ProcessingInstruction;
              }
              else
                goto case XmlNodeType.ProcessingInstruction;
            default:
              throw new FormatException(Microsoft.Framework.ConfigurationModel.Xml.Resources.FormatError_UnsupportedNodeType((object) decryptingXmlReader.NodeType, (object) XmlConfigurationSource.GetLineInfo(decryptingXmlReader)));
          }
        }
      }
      this.Data = (IDictionary<string, string>) dictionary;
    }

    private void SkipUntilRootElement(XmlReader reader)
    {
      do
        ;
      while (reader.Read() && (reader.NodeType == XmlNodeType.XmlDeclaration || reader.NodeType == XmlNodeType.ProcessingInstruction));
    }

    private static string GetLineInfo(XmlReader reader)
    {
      IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
      if (xmlLineInfo != null)
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.FormatMsg_LineInfo((object) xmlLineInfo.LineNumber, (object) xmlLineInfo.LinePosition);
      return string.Empty;
    }

    private void ProcessAttributes(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data, Action<XmlReader, Stack<string>, IDictionary<string, string>, XmlWriter> act, XmlWriter writer = null)
    {
      for (int i = 0; i < reader.AttributeCount; ++i)
      {
        reader.MoveToAttribute(i);
        if (!string.IsNullOrEmpty(reader.NamespaceURI))
          throw new FormatException(Microsoft.Framework.ConfigurationModel.Xml.Resources.FormatError_NamespaceIsNotSupported((object) XmlConfigurationSource.GetLineInfo(reader)));
        act(reader, prefixStack, data, writer);
      }
      reader.MoveToElement();
    }

    private static void AddNamePrefix(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data, XmlWriter writer)
    {
      if (!string.Equals(reader.LocalName, "Name", StringComparison.OrdinalIgnoreCase))
        return;
      if (prefixStack.Any<string>())
      {
        string str = prefixStack.Pop();
        prefixStack.Push(str + Constants.KeyDelimiter + reader.Value);
      }
      else
        prefixStack.Push(reader.Value);
    }

    private static void AddAttributePair(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data, XmlWriter writer)
    {
      if (string.Equals(reader.LocalName, "Name", StringComparison.OrdinalIgnoreCase))
        return;
      prefixStack.Push(reader.LocalName);
      string key = string.Join(Constants.KeyDelimiter, prefixStack.Reverse<string>());
      if (data.ContainsKey(key))
        throw new FormatException(Microsoft.Framework.ConfigurationModel.Xml.Resources.FormatError_KeyIsDuplicated((object) key, (object) XmlConfigurationSource.GetLineInfo(reader)));
      data[key] = reader.Value;
      prefixStack.Pop();
    }
  }
}
