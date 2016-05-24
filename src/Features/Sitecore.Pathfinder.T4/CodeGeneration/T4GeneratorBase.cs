// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.CodeDom.Compiler;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Tasks.Building;
using T4;

namespace Sitecore.Pathfinder.T4.CodeGeneration
{
    public abstract class T4GeneratorBase : ProjectCodeGeneratorBase
    {
        [NotNull]
        protected virtual Host GetHost([NotNull] IBuildContext context, [NotNull] IProject project)
        {
            var host = new Host(context, project);

            host.Refs.Add(typeof(Enumerable).Assembly.Location); // System.Core
            host.Refs.Add(typeof(HttpContext).Assembly.Location); // System.Web
            host.Refs.Add(typeof(ICompositionService).Assembly.Location); // System.ComponentModel.Composition
            host.Refs.Add(typeof(XmlDocument).Assembly.Location); // System.Xml
            host.Refs.Add(typeof(XElement).Assembly.Location); // System.Xml.Linq

            host.Refs.Add(typeof(IConfiguration).Assembly.Location); // Microsoft.Framework.ConfigurationModel.Interfaces

            host.Refs.Add(typeof(JsonReader).Assembly.Location); // Sitecore.Pathfinder.Core
            host.Refs.Add(typeof(Constants).Assembly.Location); // Sitecore.Pathfinder.Core
            host.Refs.Add(typeof(Host).Assembly.Location); // Sitecore.Pathfinder.T4

            host.Refs.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCode.Refs).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));
            host.Imports.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCode.Imports).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));
            host.IncludePaths.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCode.IncludePaths).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));
            host.ReferencePaths.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCode.ReferencePaths).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));

            return host;
        }

        protected virtual bool Ignore([NotNull] string fileName)
        {
            return fileName.IndexOf("\\sitecore.filetemplates\\", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual void TraceErrors([NotNull] IBuildContext context, [NotNull] Host host, [NotNull] string fileName)
        {
            foreach (CompilerError error in host.Errors)
            {
                context.Trace.TraceError(Msg.G1006, error.ErrorText, PathHelper.UnmapPath(context.Project.ProjectDirectory, fileName), new TextSpan(error.Line, error.Column, 0));
            }
        }
    }
}
