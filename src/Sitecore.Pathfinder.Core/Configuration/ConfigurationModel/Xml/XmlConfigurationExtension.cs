// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.XmlConfigurationExtension
// Assembly: Microsoft.Framework.ConfigurationModel.Xml, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34572FA8-986C-4AAC-914F-69C1CDA5880E
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Xml.dll

using System;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  public static class XmlConfigurationExtension
  {
    public static IConfigurationSourceRoot AddXmlFile(this IConfigurationSourceRoot configuration, string path)
    {
      return configuration.AddXmlFile(path, false);
    }

    public static IConfigurationSourceRoot AddXmlFile(this IConfigurationSourceRoot configuration, string path, bool optional)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(Microsoft.Framework.ConfigurationModel.Xml.Resources.Error_InvalidFilePath, "path");
      string str = XmlPathResolver.ResolveAppRelativePath(path);
      if (!optional && !File.Exists(str))
        throw new FileNotFoundException(Microsoft.Framework.ConfigurationModel.Xml.Resources.Error_FileNotFound, str);
      configuration.Add((IConfigurationSource) new XmlConfigurationSource(path, optional));
      return configuration;
    }
  }
}
