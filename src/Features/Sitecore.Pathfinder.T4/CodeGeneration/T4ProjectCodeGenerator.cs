// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.CodeDom.Compiler;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.CodeGeneration;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using T4;

namespace Sitecore.Pathfinder.T4.CodeGeneration
{
    public class T4ProjectCodeGenerator : ProjectCodeGeneratorBase
    {
        public override void Generate(IBuildContext context, IProject project)
        {
            var host = new Host(context, project);

            host.Refs.Add(typeof(Enumerable).Assembly.Location); // System.Core
            host.Refs.Add(typeof(HttpContext).Assembly.Location); // System.Web
            host.Refs.Add(typeof(ICompositionService).Assembly.Location); // System.ComponentModel.Composition
            host.Refs.Add(typeof(XmlDocument).Assembly.Location); // System.Xml
            host.Refs.Add(typeof(XElement).Assembly.Location); // System.Xml.Linq
            host.Refs.Add(typeof(JsonReader).Assembly.Location); // Sitecore.Pathfinder.Core
            host.Refs.Add(typeof(Constants).Assembly.Location); // Sitecore.Pathfinder.Core
            host.Refs.Add(typeof(Host).Assembly.Location); // Sitecore.Pathfinder.T4

            host.Refs.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCodeRefs).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));
            host.Imports.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCodeImports).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));
            host.IncludePaths.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCodeIncludePaths).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));
            host.ReferencePaths.AddRange(context.Configuration.GetString(Constants.Configuration.GenerateCodeReferencePaths).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));

            foreach (var fileName in context.FileSystem.GetFiles(context.ProjectDirectory, "*.project.tt", SearchOption.AllDirectories))
            {
                if (host.ProcessTemplate(fileName, fileName.Left(fileName.Length - 11)))
                {
                    continue;
                }

                foreach (CompilerError error in host.Errors)
                {
                    context.Trace.TraceError(error.ErrorText, PathHelper.UnmapPath(context.ProjectDirectory, fileName), new TextSpan(error.Line, error.Column, 0));
                }
            }
        }
    }
}
