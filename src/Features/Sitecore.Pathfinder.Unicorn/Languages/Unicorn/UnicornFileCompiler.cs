﻿// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Rainbow.Storage.Yaml;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileCompiler : CompilerBase
    {
        [ImportingConstructor]
        public UnicornFileCompiler([NotNull] IFileSystemService fileSystem) : base(1000)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is UnicornFile;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var unicornFile = projectItem as UnicornFile;
            Assert.Cast(unicornFile, nameof(unicornFile));

            var snapshot = unicornFile.Snapshots.First();

            try
            {
                CompileUnicornFile(context, snapshot, unicornFile);
            }
            catch (NotImplementedException)
            {
                // using stub Sitecore.Kernel assembly in unit tests
            }
        }

        protected virtual void CompileUnicornFile([NotNull] ICompileContext context, [NotNull] ISnapshot snapshot, [NotNull] UnicornFile unicornFile)
        {
            // todo: use real Unicorn configuration instead of hacking it
            var formatter = new YamlSerializationFormatter(null, new AllFieldFilter());
            using (var stream = FileSystem.OpenRead(snapshot.SourceFile.AbsoluteFileName))
            {
                var serializedItem = formatter.ReadSerializedItem(stream, unicornFile.ShortName);

                var guid = serializedItem.Id;
                var databaseName = serializedItem.DatabaseName ?? unicornFile.DatabaseName;
                var itemIdOrPath = serializedItem.Path;
                var itemName = serializedItem.Name;
                var templateIdOrPath = serializedItem.TemplateId.Format();

                var item = context.Factory.Item(unicornFile.Project, snapshot, guid, databaseName, itemName, itemIdOrPath, templateIdOrPath);
                item.ItemNameProperty.AddSourceTextNode(snapshot);
                item.IsEmittable = false;
                item.IsImport = false;

                item.References.AddRange(context.ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));

                foreach (var sharedField in serializedItem.SharedFields)
                {
                    var field = context.Factory.Field(item);

                    if (!string.IsNullOrEmpty(sharedField.NameHint))
                    {
                        field.FieldName = sharedField.NameHint;
                    }

                    field.FieldId = sharedField.FieldId;
                    field.Value = sharedField.Value;

                    context.ReferenceParser.ParseReferences(item, field.ValueProperty);
                }

                foreach (var version in serializedItem.Versions)
                {
                    foreach (var versionedField in version.Fields)
                    {
                        var field = context.Factory.Field(item);

                        if (!string.IsNullOrEmpty(versionedField.NameHint))
                        {
                            field.FieldName = versionedField.NameHint;
                        }

                        field.FieldId = versionedField.FieldId;
                        field.Value = versionedField.Value;
                        field.Language = version.Language.ToString();
                        field.Version = version.VersionNumber;

                        context.ReferenceParser.ParseReferences(item, field.ValueProperty);
                    }
                }

                unicornFile.Project.AddOrMerge(item);
            }
        }
    }
}
