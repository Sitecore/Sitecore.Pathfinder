// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.PathResolver
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  internal static class PathResolver
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
      return Path.Combine(PathResolver.ApplicationBaseDirectory, path);
    }
  }
}
