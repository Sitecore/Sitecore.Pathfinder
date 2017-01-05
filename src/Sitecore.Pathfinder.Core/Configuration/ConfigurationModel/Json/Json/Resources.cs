// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.Json.Resources
// Assembly: Microsoft.Framework.ConfigurationModel.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 29E3F8BD-4D3C-4C9D-8840-A11A97E69911
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Json.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Framework.ConfigurationModel.Json
{
  internal static class Resources
  {
    private static readonly ResourceManager _resourceManager = new ResourceManager("Microsoft.Framework.ConfigurationModel.Json.Resources", ((Type) IntrospectionExtensions.GetTypeInfo(typeof (Microsoft.Framework.ConfigurationModel.Json.Resources))).Assembly);

    /// <summary>
    /// Unable to commit because the following keys are missing from the configuration file: {0}.
    /// </summary>
    internal static string Error_CommitWhenKeyMissing
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_CommitWhenKeyMissing");
      }
    }

    /// <summary>
    /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
    /// </summary>
    internal static string Error_CommitWhenNewKeyFound
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_CommitWhenNewKeyFound");
      }
    }

    /// <summary>
    /// The configuration file '{0}' was not found and is not optional.
    /// </summary>
    internal static string Error_FileNotFound
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_FileNotFound");
      }
    }

    /// <summary>File path must be a non-empty string.</summary>
    internal static string Error_InvalidFilePath
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_InvalidFilePath");
      }
    }

    /// <summary>A duplicate key '{0}' was found.</summary>
    internal static string Error_KeyIsDuplicated
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_KeyIsDuplicated");
      }
    }

    /// <summary>
    /// Only an object can be the root. Path '{0}', line {1} position {2}.
    /// </summary>
    internal static string Error_RootMustBeAnObject
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_RootMustBeAnObject");
      }
    }

    /// <summary>
    /// Unexpected end when parsing JSON. Path '{0}', line {1} position {2}.
    /// </summary>
    internal static string Error_UnexpectedEnd
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_UnexpectedEnd");
      }
    }

    /// <summary>
    /// Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.
    /// </summary>
    internal static string Error_UnsupportedJSONToken
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_UnsupportedJSONToken");
      }
    }

    /// <summary>
    /// Unable to commit because the following keys are missing from the configuration file: {0}.
    /// </summary>
    internal static string FormatError_CommitWhenKeyMissing(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_CommitWhenKeyMissing"), new object[1]
      {
        p0
      });
    }

    /// <summary>
    /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
    /// </summary>
    internal static string FormatError_CommitWhenNewKeyFound(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_CommitWhenNewKeyFound"), new object[1]
      {
        p0
      });
    }

    /// <summary>
    /// The configuration file '{0}' was not found and is not optional.
    /// </summary>
    internal static string FormatError_FileNotFound(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_FileNotFound"), new object[1]
      {
        p0
      });
    }

    /// <summary>File path must be a non-empty string.</summary>
    internal static string FormatError_InvalidFilePath()
    {
      return Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_InvalidFilePath");
    }

    /// <summary>A duplicate key '{0}' was found.</summary>
    internal static string FormatError_KeyIsDuplicated(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_KeyIsDuplicated"), new object[1]
      {
        p0
      });
    }

    /// <summary>
    /// Only an object can be the root. Path '{0}', line {1} position {2}.
    /// </summary>
    internal static string FormatError_RootMustBeAnObject(object p0, object p1, object p2)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_RootMustBeAnObject"), p0, p1, p2);
    }

    /// <summary>
    /// Unexpected end when parsing JSON. Path '{0}', line {1} position {2}.
    /// </summary>
    internal static string FormatError_UnexpectedEnd(object p0, object p1, object p2)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_UnexpectedEnd"), p0, p1, p2);
    }

    /// <summary>
    /// Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.
    /// </summary>
    internal static string FormatError_UnsupportedJSONToken(object p0, object p1, object p2, object p3)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Json.Resources.GetString("Error_UnsupportedJSONToken"), p0, p1, p2, p3);
    }

    private static string GetString(string name, params string[] formatterNames)
    {
      string str = Microsoft.Framework.ConfigurationModel.Json.Resources._resourceManager.GetString(name);
      if (formatterNames != null)
      {
        for (int index = 0; index < formatterNames.Length; ++index)
          str = str.Replace("{" + formatterNames[index] + "}", "{" + (object) index + "}");
      }
      return str;
    }
  }
}
