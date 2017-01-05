// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.PathResolver
// Assembly: Microsoft.Framework.ConfigurationModel.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 29E3F8BD-4D3C-4C9D-8840-A11A97E69911
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Json.dll

using System;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  internal static class JsonPathResolver
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
      return Path.Combine(ApplicationBaseDirectory, path);
    }
  }
}
