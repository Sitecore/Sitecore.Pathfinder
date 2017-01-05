// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.IConfigurationSourceRoot
// Assembly: Microsoft.Framework.ConfigurationModel.Interfaces, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4573E8-9F09-49F3-B2EF-6350D9165C25
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Interfaces.dll

using System.Collections.Generic;

namespace Microsoft.Framework.ConfigurationModel
{
  public interface IConfigurationSourceRoot : IConfiguration
  {
    IEnumerable<IConfigurationSource> Sources { get; }

    IConfigurationSourceRoot Add(IConfigurationSource configurationSource);

    void Reload();
  }
}
