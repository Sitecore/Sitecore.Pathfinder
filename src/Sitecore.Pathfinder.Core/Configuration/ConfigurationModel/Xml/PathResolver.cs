// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.PathResolver
// Assembly: Microsoft.Framework.ConfigurationModel.Xml, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34572FA8-986C-4AAC-914F-69C1CDA5880E
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Xml.dll

using System;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  internal static class XmlPathResolver
  {
    private static string ApplicationBaseDirectory
    {
      get
      {
        return AppDomain.CurrentDomain.BaseDirectory;
      }
    }

    public static string ResolveAppRelativePath(string path)
    {
      return Path.Combine(XmlPathResolver.ApplicationBaseDirectory, path);
    }
  }
}
