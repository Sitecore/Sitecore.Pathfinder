// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Text;
using System.Xml;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;

// to generate Factory.generated.cs, run "gulp generate-factory" in the root directory

namespace Sitecore.Pathfinder.Configuration
{
    [Export(typeof(IFactory)), Shared]
    public partial class Factory
    {
        [ImportingConstructor]
        public Factory([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService trace, [NotNull] IConsoleService console, [NotNull] IFileSystemService fileSystem)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Trace = trace;
            Console = console;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public virtual Field Field(Item item, string fieldName, string fieldValue)
        {
            var field = new Field(item);
            field.FieldNameProperty.SetValue(fieldName);
            field.ValueProperty.SetValue(fieldValue);
            return field;
        }

        public T Resolve<T>() => CompositionService.Resolve<T>();

        public T Resolve<T>(string contractName) => CompositionService.Resolve<T>(contractName);

        public IEnumerable<T> ResolveMany<T>() => CompositionService.ResolveMany<T>();

        // ReSharper disable once InconsistentNaming
        public virtual XmlWriter XmlWriter(TextWriter writer, bool encoderShouldEmitUTF8Identifier = false)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier),
                Indent = true
            };

            return System.Xml.XmlWriter.Create(writer, settings);
        }

        // ReSharper disable once InconsistentNaming
        public XmlWriter XmlWriter(StringBuilder writer, bool encoderShouldEmitUTF8Identifier = false)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier),
                Indent = true
            };

            return System.Xml.XmlWriter.Create(writer, settings);
        }
    }
}
