// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.Xml.Resources
// Assembly: Microsoft.Framework.ConfigurationModel.Xml, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34572FA8-986C-4AAC-914F-69C1CDA5880E
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Xml.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Framework.ConfigurationModel.Xml
{
  internal static class Resources
  {
    private static readonly ResourceManager _resourceManager = new ResourceManager("Microsoft.Framework.ConfigurationModel.Xml.Resources", ((Type) IntrospectionExtensions.GetTypeInfo(typeof (Microsoft.Framework.ConfigurationModel.Xml.Resources))).Assembly);

    /// <summary>
    /// Unable to commit because the following keys are missing from the configuration file: {0}.
    /// </summary>
    internal static string Error_CommitWhenKeyMissing
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_CommitWhenKeyMissing");
      }
    }

    /// <summary>
    /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
    /// </summary>
    internal static string Error_CommitWhenNewKeyFound
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_CommitWhenNewKeyFound");
      }
    }

    /// <summary>Encrypted XML is not supported on this platform.</summary>
    internal static string Error_EncryptedXmlNotSupported
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_EncryptedXmlNotSupported");
      }
    }

    /// <summary>
    /// The configuration file '{0}' was not found and is not optional.
    /// </summary>
    internal static string Error_FileNotFound
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_FileNotFound");
      }
    }

    /// <summary>File path must be a non-empty string.</summary>
    internal static string Error_InvalidFilePath
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_InvalidFilePath");
      }
    }

    /// <summary>A duplicate key '{0}' was found.{1}</summary>
    internal static string Error_KeyIsDuplicated
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_KeyIsDuplicated");
      }
    }

    /// <summary>XML namespaces are not supported.{0}</summary>
    internal static string Error_NamespaceIsNotSupported
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_NamespaceIsNotSupported");
      }
    }

    /// <summary>Unsupported node type '{0}' was found.{1}</summary>
    internal static string Error_UnsupportedNodeType
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_UnsupportedNodeType");
      }
    }

    /// <summary>Line {0}, position {1}.</summary>
    internal static string Msg_LineInfo
    {
      get
      {
        return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Msg_LineInfo");
      }
    }

    /// <summary>
    /// Unable to commit because the following keys are missing from the configuration file: {0}.
    /// </summary>
    internal static string FormatError_CommitWhenKeyMissing(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_CommitWhenKeyMissing"), new object[1]
      {
        p0
      });
    }

    /// <summary>
    /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
    /// </summary>
    internal static string FormatError_CommitWhenNewKeyFound(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_CommitWhenNewKeyFound"), new object[1]
      {
        p0
      });
    }

    /// <summary>Encrypted XML is not supported on this platform.</summary>
    internal static string FormatError_EncryptedXmlNotSupported()
    {
      return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_EncryptedXmlNotSupported");
    }

    /// <summary>
    /// The configuration file '{0}' was not found and is not optional.
    /// </summary>
    internal static string FormatError_FileNotFound(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_FileNotFound"), new object[1]
      {
        p0
      });
    }

    /// <summary>File path must be a non-empty string.</summary>
    internal static string FormatError_InvalidFilePath()
    {
      return Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_InvalidFilePath");
    }

    /// <summary>A duplicate key '{0}' was found.{1}</summary>
    internal static string FormatError_KeyIsDuplicated(object p0, object p1)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_KeyIsDuplicated"), new object[2]
      {
        p0,
        p1
      });
    }

    /// <summary>XML namespaces are not supported.{0}</summary>
    internal static string FormatError_NamespaceIsNotSupported(object p0)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_NamespaceIsNotSupported"), new object[1]
      {
        p0
      });
    }

    /// <summary>Unsupported node type '{0}' was found.{1}</summary>
    internal static string FormatError_UnsupportedNodeType(object p0, object p1)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Error_UnsupportedNodeType"), new object[2]
      {
        p0,
        p1
      });
    }

    /// <summary>Line {0}, position {1}.</summary>
    internal static string FormatMsg_LineInfo(object p0, object p1)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.Framework.ConfigurationModel.Xml.Resources.GetString("Msg_LineInfo"), new object[2]
      {
        p0,
        p1
      });
    }

    private static string GetString(string name, params string[] formatterNames)
    {
      string str = Microsoft.Framework.ConfigurationModel.Xml.Resources._resourceManager.GetString(name);
      if (formatterNames != null)
      {
        for (int index = 0; index < formatterNames.Length; ++index)
          str = str.Replace("{" + formatterNames[index] + "}", "{" + (object) index + "}");
      }
      return str;
    }
  }
}
