﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

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
            var bytes = Encoding.UTF8.GetBytes(text);
            var hash = Md5Hash.ComputeHash(bytes);
            return new Guid(hash);
        }

        [NotNull]
        public static string UnescapeXmlNodeName([NotNull] string nodeName)
        {
            return nodeName.Replace("--", " ");
        }
    }
}