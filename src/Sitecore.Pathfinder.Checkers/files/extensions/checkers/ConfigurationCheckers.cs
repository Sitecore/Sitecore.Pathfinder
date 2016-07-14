// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.BinFiles;
using Sitecore.Pathfinder.Languages.ConfigFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checkers
{
    public class ConfigurationCheckers : Checker
    {
        [ImportingConstructor]
        public ConfigurationCheckers([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [Export("Check")]
        public IEnumerable<Diagnostic> TypeNotFound(ICheckerContext context)
        {
            var ignoreAssemblies = Configuration.GetStringList(Constants.Configuration.CheckProject.ConfigurationCheckerDevAssemblies, GetStringListOptions.UseKey).ToArray();

            foreach (var configFile in context.Project.ProjectItems.OfType<ConfigFile>())
            {
                var fileName = configFile.Snapshot.SourceFile.AbsoluteFileName;

                XDocument doc;
                try
                {
                    doc = FileSystem.ReadXml(fileName, LoadOptions.SetLineInfo);
                }
                catch (XmlException ex)
                {
                    context.Trace.TraceError(Msg.C1055, ex.Message, fileName, new TextSpan(ex.LineNumber, ex.LinePosition, 0));
                    continue;
                }
                catch (Exception ex)
                {
                    context.Trace.TraceError(Msg.C1056, ex.Message, fileName, TextSpan.Empty);
                    continue;
                }

                var elements = doc.XPathSelectElements("//*[@type]");
                foreach (var element in elements)
                {
                    var typeName = element.GetAttributeValue("type");
                    if (string.IsNullOrEmpty(typeName))
                    {
                        continue;
                    }

                    if (typeName == "both")
                    {
                        continue;
                    }

                    typeName = typeName.Replace(", mscorlib", string.Empty).Replace(",mscorlib", string.Empty);

                    var assemblyName = string.Empty;
                    var n = typeName.IndexOf(',');
                    if (n >= 0)
                    {
                        assemblyName = typeName.Mid(n + 1).Trim();
                        typeName = typeName.Left(n).Trim();
                    }

                    if (string.IsNullOrEmpty(assemblyName))
                    {
                        continue;
                    }

                    if (ignoreAssemblies.Any(a => string.Equals(a, assemblyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    // add '.dll' extension
                    if (!string.IsNullOrEmpty(assemblyName) && !assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        assemblyName += ".dll";
                    }

                    Type type = null;
                    try
                    {
                        if (!string.IsNullOrEmpty(assemblyName))
                        {
                            var binFile = context.Project.ProjectItems.OfType<BinFile>().FirstOrDefault(f => string.Equals(f.ShortName, assemblyName, StringComparison.OrdinalIgnoreCase));
                            if (binFile != null)
                            {
                                type = GetTypeFromAssembly(binFile, typeName);
                            }
                        }
                        else
                        {
                            foreach (var binFile in context.Project.ProjectItems.OfType<BinFile>())
                            {
                                type = GetTypeFromAssembly(binFile, typeName);
                                if (type != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        type = null;
                    }

                    if (type != null)
                    {
                        continue;
                    }

                    var lineInfo = (IXmlLineInfo)element;
                    var textSpan = lineInfo != null ? new TextSpan(lineInfo.LineNumber, lineInfo.LinePosition, 0) : TextSpan.Empty;

                    yield return Warning(Msg.C1054, "Type does not exist in a referenced assembly", fileName, textSpan, element.GetAttributeValue("type"));
                }
            }
        }

        [CanBeNull]
        protected virtual Type GetTypeFromAssembly([NotNull] BinFile binFile, [NotNull] string typeName)
        {
            var assembly = Assembly.LoadFrom(binFile.Snapshot.SourceFile.AbsoluteFileName);
            return assembly == null ? null : assembly.GetType(typeName, false);
        }
    }
}
