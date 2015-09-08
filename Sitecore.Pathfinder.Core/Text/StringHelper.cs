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
        private static readonly MD5 Md5Hash = MD5.Create();

        public static Guid GetGuid(IProject project, string id)
        {
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {
                // calculate guid from id and project id case-insensitively
                var text = (project.ProjectUniqueId + "/" + id).ToUpperInvariant();
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = Md5Hash.ComputeHash(bytes);
                guid = new Guid(hash);
            }

            return guid;
        }

        [NotNull]
        public static string UnescapeXmlNodeName([NotNull] string nodeName)
        {
            return nodeName.Replace("--", " ");
        }
    }
}
