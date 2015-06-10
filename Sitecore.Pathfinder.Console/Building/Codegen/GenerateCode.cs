// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Building.Codegen
{
    [Export(typeof(ITask))]
    public class GenerateCode : TaskBase
    {
        private IRazorEngineService _razorEngine;

        public GenerateCode() : base("generate-code")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Generating_code___);

            CreateRazorEngine();

            var templates = GetTemplates(context);

            foreach (var projectItem in context.Project.Items)
            {
                var type = projectItem.GetType();

                foreach (var pair in templates)
                {
                    if (type == pair.Value)
                    {
                        Compile(context, projectItem, pair.Key);
                    }
                    else if (type.IsSubclassOf(pair.Value))
                    {
                        Compile(context, projectItem, pair.Key);
                    }
                    else if (pair.Value.IsInterface && pair.Value.IsAssignableFrom(pair.Value))
                    {
                        Compile(context, projectItem, pair.Key);
                    }
                }
            }
        }

        protected virtual void Compile([NotNull] IBuildContext context, [NotNull] IProjectItem projectItem, [NotNull] string fileName)
        {
            fileName = Path.Combine(context.SolutionDirectory, fileName);

            var template = context.FileSystem.ReadAllText(fileName);

            var viewBag = new DynamicViewBag();
            viewBag.AddValue("BuildContext", context);

            var result = _razorEngine.RunCompile(template, fileName, projectItem.GetType(), projectItem, viewBag);

            var targetFileName = Path.GetDirectoryName(projectItem.Snapshots.First().SourceFile.FileName) ?? string.Empty;
            targetFileName = Path.Combine(targetFileName, projectItem.ShortName) + ".g.cs";

            context.FileSystem.CreateDirectory(Path.GetDirectoryName(targetFileName) ?? string.Empty);
            context.FileSystem.WriteAllText(targetFileName, result);

            context.Trace.TraceInformation(PathHelper.UnmapPath(context.SolutionDirectory, targetFileName));
        }

        protected virtual void CreateRazorEngine()
        {
            if (_razorEngine != null)
            {
                return;
            }

            var config = new TemplateServiceConfiguration
            {
                DisableTempFileLocking = true,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            _razorEngine = RazorEngineService.Create(config);
        }

        [NotNull]
        protected virtual Dictionary<string, Type> GetTemplates([NotNull] IBuildContext context)
        {
            var map = new Dictionary<string, Type>();
            foreach (var pair in context.Configuration.GetSubKeys(Constants.Configuration.CodeGen))
            {
                var typeName = context.Configuration.Get(Constants.Configuration.CodeGen + ":" + pair.Key);

                var type = Type.GetType(typeName);
                if (type == null)
                {
                    throw new ConfigurationException(Texts.Type_not_found, typeName);
                }

                map[pair.Key] = type;
            }

            return map;
        }
    }
}
