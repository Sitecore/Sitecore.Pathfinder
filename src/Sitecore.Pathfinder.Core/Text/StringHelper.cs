// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Security.Cryptography;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Text
{
    public static class StringHelper
    {
        // [NotNull]
        // private static readonly MD5 Md5Hash = MD5.Create();

        public static Guid GetGuid([NotNull] IProject project, [NotNull] string id)
        {
            Guid guid;
            if (Guid.TryParse(id, out guid))
            {
                return guid;
            }

            if (id.StartsWith("{") && id.EndsWith("}"))
            {
                return ToGuid(id);
            }

            // calculate guid from id and project id case-insensitively
            return ToGuid((project.ProjectUniqueId + "/" + id).ToUpperInvariant());
        }

        public static Guid ToGuid([NotNull] string text)
        {
            var md5 = MD5.Create();

            var bytes = Encoding.UTF8.GetBytes(text);
            var hash = md5.ComputeHash(bytes);
            return new Guid(hash);
        }
    }
}
