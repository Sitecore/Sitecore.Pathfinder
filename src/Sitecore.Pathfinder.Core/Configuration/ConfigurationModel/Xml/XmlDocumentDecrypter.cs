// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.XmlDocumentDecryptor
// Assembly: Microsoft.Framework.ConfigurationModel.Xml, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34572FA8-986C-4AAC-914F-69C1CDA5880E
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Xml.dll

using System;
using System.IO;
using System.Xml;

namespace Microsoft.Framework.ConfigurationModel
{
    internal class XmlDocumentDecryptor
    {
        /// <summary>Accesses the singleton decryptor instance.</summary>
        public static readonly XmlDocumentDecryptor Instance = (XmlDocumentDecryptor)new XmlDocumentDecryptor();

        protected XmlDocumentDecryptor()
        {
        }

        private static bool ContainsEncryptedData(XmlDocument document)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
            return document.SelectSingleNode("//enc:EncryptedData", nsmgr) != null;
        }

        /// <summary>
        /// Returns an XmlReader that decrypts data transparently.
        /// </summary>
        public XmlReader CreateDecryptingXmlReader(Stream input, XmlReaderSettings settings)
        {
            MemoryStream memoryStream = new MemoryStream();
            input.CopyTo((Stream)memoryStream);
            memoryStream.Position = 0L;
            XmlDocument document = new XmlDocument();
            using (XmlReader reader = XmlReader.Create((Stream)memoryStream, settings))
                document.Load(reader);
            memoryStream.Position = 0L;
            // if (XmlDocumentDecryptor.ContainsEncryptedData(document))
            //     return this.DecryptDocumentAndCreateXmlReader(document);
            return XmlReader.Create((Stream)memoryStream, settings);
        }

        protected virtual XmlReader DecryptDocumentAndCreateXmlReader(XmlDocument document)
        {
            throw new PlatformNotSupportedException(Microsoft.Framework.ConfigurationModel.Xml.Resources.Error_EncryptedXmlNotSupported);
        }
    }
}
