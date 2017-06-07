using System.Composition;
using System.IO;
using System.Text;
using System.Xml;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

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
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public virtual Field Field(Item item, string fieldName, string fieldValue)
        {
            var field = new Field(item);
            field.FieldNameProperty.SetValue(fieldName);
            field.ValueProperty.SetValue(fieldValue);
            return field;
        }

        public virtual ISnapshot Snapshot(ISourceFile sourceFile) => new Snapshot().With(sourceFile);

        public virtual ISourceFile SourceFile(IFileSystemService fileSystem, string absoluteFileName) => new SourceFile(Configuration, fileSystem, absoluteFileName);

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
    }
}
