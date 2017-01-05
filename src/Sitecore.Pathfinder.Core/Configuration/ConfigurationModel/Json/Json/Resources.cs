// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Json.Json
{
    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("Microsoft.Framework.ConfigurationModel.Json.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// Unable to commit because the following keys are missing from the configuration file: {0}.
        /// </summary>
        internal static string Error_CommitWhenKeyMissing
        {
            get { return GetString("Error_CommitWhenKeyMissing"); }
        }

        /// <summary>
        /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
        /// </summary>
        internal static string Error_CommitWhenNewKeyFound
        {
            get { return GetString("Error_CommitWhenNewKeyFound"); }
        }

        /// <summary>
        /// The configuration file '{0}' was not found and is not optional.
        /// </summary>
        internal static string Error_FileNotFound
        {
            get { return GetString("Error_FileNotFound"); }
        }

        /// <summary>File path must be a non-empty string.</summary>
        internal static string Error_InvalidFilePath
        {
            get { return GetString("Error_InvalidFilePath"); }
        }

        /// <summary>A duplicate key '{0}' was found.</summary>
        internal static string Error_KeyIsDuplicated
        {
            get { return GetString("Error_KeyIsDuplicated"); }
        }

        /// <summary>
        /// Only an object can be the root. Path '{0}', line {1} position {2}.
        /// </summary>
        internal static string Error_RootMustBeAnObject
        {
            get { return GetString("Error_RootMustBeAnObject"); }
        }

        /// <summary>
        /// Unexpected end when parsing JSON. Path '{0}', line {1} position {2}.
        /// </summary>
        internal static string Error_UnexpectedEnd
        {
            get { return GetString("Error_UnexpectedEnd"); }
        }

        /// <summary>
        /// Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.
        /// </summary>
        internal static string Error_UnsupportedJSONToken
        {
            get { return GetString("Error_UnsupportedJSONToken"); }
        }

        /// <summary>
        /// Unable to commit because the following keys are missing from the configuration file: {0}.
        /// </summary>
        internal static string FormatError_CommitWhenKeyMissing(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_CommitWhenKeyMissing"), new object[1]
            {
                p0
            });
        }

        /// <summary>
        /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
        /// </summary>
        internal static string FormatError_CommitWhenNewKeyFound(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_CommitWhenNewKeyFound"), new object[1]
            {
                p0
            });
        }

        /// <summary>
        /// The configuration file '{0}' was not found and is not optional.
        /// </summary>
        internal static string FormatError_FileNotFound(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_FileNotFound"), new object[1]
            {
                p0
            });
        }

        /// <summary>File path must be a non-empty string.</summary>
        internal static string FormatError_InvalidFilePath()
        {
            return GetString("Error_InvalidFilePath");
        }

        /// <summary>A duplicate key '{0}' was found.</summary>
        internal static string FormatError_KeyIsDuplicated(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_KeyIsDuplicated"), new object[1]
            {
                p0
            });
        }

        /// <summary>
        /// Only an object can be the root. Path '{0}', line {1} position {2}.
        /// </summary>
        internal static string FormatError_RootMustBeAnObject(object p0, object p1, object p2)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_RootMustBeAnObject"), p0, p1, p2);
        }

        /// <summary>
        /// Unexpected end when parsing JSON. Path '{0}', line {1} position {2}.
        /// </summary>
        internal static string FormatError_UnexpectedEnd(object p0, object p1, object p2)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_UnexpectedEnd"), p0, p1, p2);
        }

        /// <summary>
        /// Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}.
        /// </summary>
        internal static string FormatError_UnsupportedJSONToken(object p0, object p1, object p2, object p3)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_UnsupportedJSONToken"), p0, p1, p2, p3);
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var str = _resourceManager.GetString(name);
            if (formatterNames != null)
            {
                for (var index = 0; index < formatterNames.Length; ++index)
                {
                    str = str.Replace("{" + formatterNames[index] + "}", "{" + index + "}");
                }
            }
            return str;
        }
    }
}
