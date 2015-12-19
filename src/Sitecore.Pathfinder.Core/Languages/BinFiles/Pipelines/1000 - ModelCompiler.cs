// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.BinFiles.Pipelines
{
    public class ModelScanner : PipelineProcessorBase<BinFileCompilerPipeline>
    {
        public ModelScanner() : base(1000)
        {
        }

        [CanBeNull]
        protected virtual object GetAttribute([NotNull] MemberInfo memberInfo, [NotNull] Type attributeType)
        {
            var attributes = CustomAttributeData.GetCustomAttributes(memberInfo);

            foreach (var attribute in attributes)
            {
                if (attribute.AttributeType != attributeType)
                {
                    continue;
                }

                var argument = attribute.ConstructorArguments.FirstOrDefault();
                if (argument == null)
                {
                    continue;
                }

                return argument.Value;
            }

            return null;
        }

        [NotNull]
        protected virtual string GetCategoryAttribute([NotNull] MemberInfo memberInfo)
        {
            return GetAttribute(memberInfo, typeof(CategoryAttribute)) as string ?? string.Empty;
        }

        [NotNull]
        protected virtual string GetDescriptionAttribute([NotNull] MemberInfo memberInfo)
        {
            return GetAttribute(memberInfo, typeof(DescriptionAttribute)) as string ?? string.Empty;
        }

        [NotNull]
        protected virtual string GetDisplayNameAttribute([NotNull] MemberInfo memberInfo)
        {
            return GetAttribute(memberInfo, typeof(DisplayNameAttribute)) as string ?? string.Empty;
        }

        protected virtual Guid GetGuidAttribute([NotNull] MemberInfo memberInfo)
        {
            var value = GetAttribute(memberInfo, typeof(GuidAttribute)) as string;
            if (string.IsNullOrEmpty(value))
            {
                return Guid.Empty;
            }

            Guid guid;
            return Guid.TryParse(value, out guid) ? guid : Guid.Empty;
        }

        protected virtual bool GetLocalizableAttribute([NotNull] MemberInfo memberInfo)
        {
            var value = GetAttribute(memberInfo, typeof(LocalizableAttribute));
            return value != null && (bool)value;
        }

        protected override void Process(BinFileCompilerPipeline pipeline)
        {
            var typeName = pipeline.Type.FullName;
            if (!typeName.EndsWith("Model"))
            {
                return;
            }

            var snapshotTextNode = new SnapshotTextNode(pipeline.BinFile.Snapshots.First());

            var itemName = GetDisplayNameAttribute(pipeline.Type);
            if (string.IsNullOrEmpty(itemName))
            {
                itemName = pipeline.Type.Name;
            }

            var databaseName = pipeline.BinFile.Project.Options.DatabaseName;

            var itemIdOrPath = "/" + typeName.Replace('.', '/');
            var n = itemIdOrPath.LastIndexOf('/');
            if (n >= 0)
            {
                itemIdOrPath = itemIdOrPath.Left(n + 1) + itemName;
            }

            var guid = GetGuidAttribute(pipeline.Type);
            if (guid == Guid.Empty)
            {
                guid = StringHelper.GetGuid(pipeline.BinFile.Project, typeName);
            }

            var template = pipeline.Context.Factory.Template(pipeline.BinFile.Project, guid, snapshotTextNode, databaseName, itemName, itemIdOrPath);
            template.IsEmittable = true;
            template.IsImport = false;

            template.ShortHelp = GetDescriptionAttribute(pipeline.Type);
            template.LongHelp = template.ShortHelp;

            // get sections and fields
            var fields = new Dictionary<string, List<PropertyInfo>>();
            foreach (var propertyInfo in pipeline.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var sectionName = GetCategoryAttribute(propertyInfo);
                if (string.IsNullOrEmpty(sectionName))
                {
                    sectionName = "Fields";
                }

                List<PropertyInfo> properties;
                if (!fields.TryGetValue(sectionName, out properties))
                {
                    properties = new List<PropertyInfo>();
                    fields[sectionName] = properties;
                }

                properties.Add(propertyInfo);
            }

            // build sections and fields
            foreach (var pair in fields)
            {
                guid = StringHelper.GetGuid(pipeline.BinFile.Project, itemIdOrPath + "/" + pair.Key);
                var templateSection = pipeline.Context.Factory.TemplateSection(template, guid, snapshotTextNode);
                templateSection.SectionName = pair.Key;

                foreach (var propertyInfo in pair.Value.OrderBy(p => p.Name))
                {
                    guid = StringHelper.GetGuid(pipeline.BinFile.Project, itemIdOrPath + "/" + pair.Key + "/" + propertyInfo.Name);
                    var templateField = pipeline.Context.Factory.TemplateField(template, guid, snapshotTextNode);

                    var fieldName = GetDisplayNameAttribute(propertyInfo);
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        fieldName = propertyInfo.Name;
                    }

                    templateField.FieldName = fieldName;
                    templateField.Type = pipeline.Context.Configuration.GetString("build-project:type-name-to-field-type:" + propertyInfo.PropertyType.FullName, "Single-line Text");
                    templateField.ShortHelp = GetDescriptionAttribute(propertyInfo);
                    templateField.LongHelp = templateField.ShortHelp;

                    if (!GetLocalizableAttribute(propertyInfo))
                    {
                        templateField.Shared = true;
                    }

                    templateSection.Fields.Add(templateField);
                }

                template.Sections.Add(templateSection);
            }

            pipeline.BinFile.Project.AddOrMerge(template);
        }
    }
}
