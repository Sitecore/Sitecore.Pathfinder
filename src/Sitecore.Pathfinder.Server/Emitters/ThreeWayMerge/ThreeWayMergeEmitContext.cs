// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters.Writers;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters.ThreeWayMerge
{
    [Export(typeof(ThreeWayMergeEmitContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ThreeWayMergeEmitContext : EmitContext, IFieldValueTracking
    {
        private const string FileName = "base.xml";

        [NotNull, ItemNotNull]
        private List<BaseField> _baseFieldValues;

        [ImportingConstructor]
        public ThreeWayMergeEmitContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystemService) : base(configuration, traceService, fileSystemService)
        {
            OverwriteDatabase = Configuration.GetBool(Constants.Configuration.InstallPackage.ThreeWayMergeOverwriteDatabase);
        }

        protected bool OverwriteDatabase { get; }

        [NotNull]
        protected string BaseDirectory { get; private set; } = string.Empty;

        public virtual bool CanSetFieldValue(Item item, FieldWriter fieldWriter, string fieldValue)
        {
            var itemId = fieldWriter.ItemWriter.Guid == Guid.Empty ? string.Empty : fieldWriter.ItemWriter.Guid.Format();
            var fieldId = fieldWriter.FieldId == Guid.Empty ? string.Empty : fieldWriter.FieldId.Format();

            var databaseFieldValue = item[fieldWriter.FieldName];

            // todo: consider making this a dictionary for performance
            var baseField = _baseFieldValues.FirstOrDefault(f => f.DatabaseName == fieldWriter.ItemWriter.DatabaseName && f.ItemId == itemId && ((!string.IsNullOrEmpty(f.FieldId) && f.FieldId == fieldId) || (!string.IsNullOrEmpty(f.FieldName) && f.FieldName == fieldWriter.FieldName)) && f.Language == fieldWriter.Language && f.Version == fieldWriter.Version);
            if (baseField == null)
            {
                baseField = new BaseField(fieldWriter.ItemWriter.DatabaseName, itemId, fieldId, fieldWriter.FieldName, fieldWriter.Language, fieldWriter.Version, fieldValue);
                _baseFieldValues.Add(baseField);

                if (OverwriteDatabase)
                {
                    return true;
                }

                // if database value is empty, set it to the new field value
                if (string.IsNullOrEmpty(databaseFieldValue))
                {
                    return true;
                }

                // database field already has value - do not overwrite it
                baseField.Value = databaseFieldValue;
                return false;
            }
            
            baseField.IsActive = true;

            if (OverwriteDatabase)
            {
                baseField.Value = fieldValue;
                return true;
            }

            // check if field value has changed - if not, ignore field
            if (fieldValue == baseField.Value)
            {
                return false;
            }

            // check if both field value and database value has changed - if so, there is a conflict
            if (databaseFieldValue != baseField.Value)
            {
                Trace.TraceError(Msg.E1037, "Merge conflict: Field has changed both in project and in database - skipping", TraceHelper.GetTextNode(fieldWriter.FieldNameProperty, fieldWriter.FieldIdProperty), fieldWriter.FieldName);
                return false;
            }

            // all is good - update value
            baseField.Value = fieldValue;
            return true;
        }

        public override void Done()
        {
            SaveBaseFields();

            base.Done();
        }

        public IEmitContext WithBaseDirectory(string baseDirectory)
        {
            BaseDirectory = baseDirectory;

            LoadBaseFields();

            return this;
        }

        protected virtual void LoadBaseFields()
        {
            var fileName = Path.Combine(BaseDirectory, FileName);
            if (!FileSystem.FileExists(fileName))
            {
                _baseFieldValues = new List<BaseField>();
                return;
            }

            _baseFieldValues = FileSystem.Deserialize(fileName, typeof(List<BaseField>)) as List<BaseField> ?? new List<BaseField>();
        }

        protected virtual void SaveBaseFields()
        {
            _baseFieldValues.RemoveAll(f => !f.IsActive);

            var fileName = Path.Combine(BaseDirectory, FileName);

            FileSystem.CreateDirectoryFromFileName(fileName);
            FileSystem.Serialize(fileName, typeof(List<BaseField>), _baseFieldValues);
        }

        [UsedImplicitly]
        public class BaseField
        {
            public BaseField()
            {
            }

            public BaseField([NotNull] string databaseName, [NotNull] string itemId, [NotNull] string fieldId, [NotNull] string fieldName, [NotNull] string language, int version, [NotNull] string value)
            {
                ItemId = itemId;
                DatabaseName = databaseName;
                FieldId = fieldId;
                FieldName = fieldName;
                Language = language;
                Version = version;
                Value = value;
            }

            [NotNull, XmlAttribute]
            public string DatabaseName { get; set; } = string.Empty;

            [NotNull, XmlAttribute, DefaultValue("")]
            public string FieldId { get; set; } = string.Empty;

            [NotNull, XmlAttribute]
            public string FieldName { get; set; } = string.Empty;

            [XmlIgnore]
            public bool IsActive { get; set; } = true;

            [NotNull, XmlAttribute, DefaultValue("")]
            public string ItemId { get; set; } = string.Empty;

            [NotNull, DefaultValue(""), XmlAttribute]
            public string Language { get; set; } = string.Empty;

            [NotNull, DefaultValue("")]
            public string Value { get; set; } = string.Empty;

            [DefaultValue(0), XmlAttribute]
            public int Version { get; set; }
        }
    }
}
