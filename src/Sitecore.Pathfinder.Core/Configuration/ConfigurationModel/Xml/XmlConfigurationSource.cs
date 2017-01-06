// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Xml
{
    public class XmlConfigurationSource : ConfigurationSource
    {
        private const string NameAttributeKey = "Name";

        [NotNull]
        private readonly XmlDocumentDecryptor _xmlDocumentDecryptor;

        public XmlConfigurationSource([NotNull] string path) : this(path, null, false)
        {
        }

        internal XmlConfigurationSource([NotNull] string path, bool optional) : this(path, null, optional)
        {
        }

        internal XmlConfigurationSource([NotNull] string path, [CanBeNull] XmlDocumentDecryptor xmlDocumentDecryptor) : this(path, xmlDocumentDecryptor, false)
        {
        }

        internal XmlConfigurationSource([NotNull] string path, [CanBeNull] XmlDocumentDecryptor xmlDocumentDecryptor, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Xml.Resources.Error_InvalidFilePath, nameof(path));
            }

            Optional = optional;
            Path = XmlPathResolver.ResolveAppRelativePath(path);
            _xmlDocumentDecryptor = xmlDocumentDecryptor ?? XmlDocumentDecryptor.Instance;
        }

        public bool Optional { get; }

        [NotNull]
        public string Path { get; }

        public override void Load()
        {
            if (!File.Exists(Path))
            {
                if (!Optional)
                {
                    throw new FileNotFoundException(string.Format(Xml.Resources.Error_FileNotFound, Path), Path);
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

        internal void Load([NotNull] Stream stream)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var settings = new XmlReaderSettings
            {
                CloseInput = false,
                DtdProcessing = DtdProcessing.Prohibit,
                IgnoreComments = true,
                IgnoreWhitespace = true
            };
            using (var decryptingXmlReader = _xmlDocumentDecryptor.CreateDecryptingXmlReader(stream, settings))
            {
                var stringStack = new Stack<string>();
                SkipUntilRootElement(decryptingXmlReader);
                ProcessAttributes(decryptingXmlReader, stringStack, dictionary, AddNamePrefix);
                ProcessAttributes(decryptingXmlReader, stringStack, dictionary, AddAttributePair);
                var xmlNodeType = decryptingXmlReader.NodeType;
                while (decryptingXmlReader.Read())
                {
                    switch (decryptingXmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            stringStack.Push(decryptingXmlReader.LocalName);
                            ProcessAttributes(decryptingXmlReader, stringStack, dictionary, AddNamePrefix);
                            ProcessAttributes(decryptingXmlReader, stringStack, dictionary, AddAttributePair);
                            if (decryptingXmlReader.IsEmptyElement)
                            {
                                stringStack.Pop();
                            }
                            goto case XmlNodeType.ProcessingInstruction;
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            var key = string.Join(Constants.KeyDelimiter, stringStack.Reverse());
                            if (dictionary.ContainsKey(key))
                            {
                                throw new FormatException(Xml.Resources.FormatError_KeyIsDuplicated(key, GetLineInfo(decryptingXmlReader)));
                            }
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
                            }
                            continue;
                        case XmlNodeType.EndElement:
                            if (stringStack.Any())
                            {
                                if (xmlNodeType == XmlNodeType.Element)
                                {
                                    var index = string.Join(Constants.KeyDelimiter, stringStack.Reverse());
                                    dictionary[index] = string.Empty;
                                }
                                stringStack.Pop();
                            }
                            goto case XmlNodeType.ProcessingInstruction;
                        default:
                            throw new FormatException(Xml.Resources.FormatError_UnsupportedNodeType(decryptingXmlReader.NodeType, GetLineInfo(decryptingXmlReader)));
                    }
                }
            }
            Data = dictionary;
        }

        private static void AddAttributePair(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data, XmlWriter writer)
        {
            if (string.Equals(reader.LocalName, "Name", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            prefixStack.Push(reader.LocalName);
            var key = string.Join(Constants.KeyDelimiter, prefixStack.Reverse());
            if (data.ContainsKey(key))
            {
                throw new FormatException(Xml.Resources.FormatError_KeyIsDuplicated(key, GetLineInfo(reader)));
            }
            data[key] = reader.Value;
            prefixStack.Pop();
        }

        private static void AddNamePrefix(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data, XmlWriter writer)
        {
            if (!string.Equals(reader.LocalName, "Name", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (prefixStack.Any())
            {
                var str = prefixStack.Pop();
                prefixStack.Push(str + Constants.KeyDelimiter + reader.Value);
            }
            else
            {
                prefixStack.Push(reader.Value);
            }
        }

        private static string GetLineInfo(XmlReader reader)
        {
            var xmlLineInfo = reader as IXmlLineInfo;
            if (xmlLineInfo != null)
            {
                return Xml.Resources.FormatMsg_LineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
            }
            return string.Empty;
        }

        private void ProcessAttributes(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data, Action<XmlReader, Stack<string>, IDictionary<string, string>, XmlWriter> act, XmlWriter writer = null)
        {
            for (var i = 0; i < reader.AttributeCount; ++i)
            {
                reader.MoveToAttribute(i);
                if (!string.IsNullOrEmpty(reader.NamespaceURI))
                {
                    throw new FormatException(Xml.Resources.FormatError_NamespaceIsNotSupported(GetLineInfo(reader)));
                }
                act(reader, prefixStack, data, writer);
            }
            reader.MoveToElement();
        }

        private void SkipUntilRootElement([NotNull] XmlReader reader)
        {
            do
            {
            }
            while (reader.Read() && (reader.NodeType == XmlNodeType.XmlDeclaration || reader.NodeType == XmlNodeType.ProcessingInstruction));
        }
    }
}
