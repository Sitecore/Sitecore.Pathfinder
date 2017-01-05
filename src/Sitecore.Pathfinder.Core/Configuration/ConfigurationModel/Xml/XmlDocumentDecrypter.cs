// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Xml;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Xml
{
    internal class XmlDocumentDecryptor
    {
        /// <summary>Accesses the singleton decryptor instance.</summary>
        public static readonly XmlDocumentDecryptor Instance = new XmlDocumentDecryptor();

        protected XmlDocumentDecryptor()
        {
        }

        /// <summary>
        /// Returns an XmlReader that decrypts data transparently.
        /// </summary>
        public XmlReader CreateDecryptingXmlReader(Stream input, XmlReaderSettings settings)
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

        protected virtual XmlReader DecryptDocumentAndCreateXmlReader(XmlDocument document)
        {
            throw new PlatformNotSupportedException(Xml.Resources.Error_EncryptedXmlNotSupported);
        }

        private static bool ContainsEncryptedData(XmlDocument document)
        {
            var nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
            return document.SelectSingleNode("//enc:EncryptedData", nsmgr) != null;
        }
    }
}
