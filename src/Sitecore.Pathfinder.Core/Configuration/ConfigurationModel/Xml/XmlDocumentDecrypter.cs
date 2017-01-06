// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Xml
{
    internal class XmlDocumentDecryptor
    {
        [NotNull]
        public static readonly XmlDocumentDecryptor Instance = new XmlDocumentDecryptor();

        protected XmlDocumentDecryptor()
        {
        }

        [NotNull]
        public XmlReader CreateDecryptingXmlReader([NotNull] Stream input, [NotNull] XmlReaderSettings settings)
        {
            var memoryStream = new MemoryStream();
            input.CopyTo(memoryStream);
            memoryStream.Position = 0L;

            var document = new XmlDocument();
            using (var reader = XmlReader.Create(memoryStream, settings))
            {
                document.Load(reader);
            }

            memoryStream.Position = 0L;

            // if (XmlDocumentDecryptor.ContainsEncryptedData(document))
            //     return this.DecryptDocumentAndCreateXmlReader(document);
            return XmlReader.Create(memoryStream, settings);
        }

        [NotNull]
        protected virtual XmlReader DecryptDocumentAndCreateXmlReader([NotNull] XmlDocument document)
        {
            throw new PlatformNotSupportedException(Xml.Resources.Error_EncryptedXmlNotSupported);
        }

        private static bool ContainsEncryptedData([NotNull] XmlDocument document)
        {
            var nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
            return document.SelectSingleNode("//enc:EncryptedData", nsmgr) != null;
        }
    }
}
