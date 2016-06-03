// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters.Writers;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Emitters.ThreeWayMerge
{
    [Export(typeof(ThreeWayMergeEmitContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ThreeWayMergeEmitContext : EmitContext, ICanSetFieldValue
    {
        private const string FileName = "base.xml";

        [NotNull, ItemNotNull]
        private List<FieldRecord> _fieldRecords;

        [ImportingConstructor]
        public ThreeWayMergeEmitContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystemService) : base(configuration, traceService, fileSystemService)
        {
        }

        [NotNull]
        protected string BaseDirectory { get; private set; } = string.Empty;

        public virtual bool CanSetFieldValue(Item item, FieldWriter fieldWriter, string fieldValue)
        {
            var fieldRecord = _fieldRecords.FirstOrDefault(f => f.DatabaseName == fieldWriter.ItemWriter.DatabaseName && f.ItemId == fieldWriter.ItemWriter.Guid && ((f.FieldId != Guid.Empty && f.FieldId == fieldWriter.FieldId) || (!string.IsNullOrEmpty(f.FieldName) && f.FieldName == fieldWriter.FieldName)) && f.Language == fieldWriter.Language && f.Version == fieldWriter.Version);
            if (fieldRecord == null)
            {
                fieldRecord = new FieldRecord(fieldWriter.ItemWriter.Guid, fieldWriter.ItemWriter.DatabaseName, fieldWriter.FieldId, fieldWriter.FieldName, fieldWriter.Language, fieldWriter.Version, fieldValue);
                fieldRecord.IsTouched = true;
                _fieldRecords.Add(fieldRecord);
                return true;
            }

            fieldRecord.IsTouched = true;

            // check if field value in database has changed since last install
            if (item[fieldWriter.FieldName] == fieldRecord.Value)
            {
                fieldRecord.Value = fieldValue;
                return true;
            }

            // value in database has changed - do not update
            return false;
        }

        public override void Done()
        {
            SaveFieldRecords();

            base.Done();
        }

        public IEmitContext WithBaseDirectory(string baseDirectory)
        {
            BaseDirectory = baseDirectory;

            LoadFieldRecords();

            return this;
        }

        protected virtual void LoadFieldRecords()
        {
            var fileName = Path.Combine(BaseDirectory, FileName);
            if (!FileSystem.FileExists(fileName))
            {
                _fieldRecords = new List<FieldRecord>();
                return;
            }

            var serializer = new XmlSerializer(typeof(List<FieldRecord>));
            using (var stream = FileSystem.OpenRead(fileName))
            {
                _fieldRecords = serializer.Deserialize(stream) as List<FieldRecord> ?? new List<FieldRecord>();
            }
        }

        protected virtual void SaveFieldRecords()
        {
            _fieldRecords.RemoveAll(f => !f.IsTouched);

            var fileName = Path.Combine(BaseDirectory, FileName);

            FileSystem.CreateDirectoryFromFileName(fileName);

            var serializer = new XmlSerializer(typeof(List<FieldRecord>));
            using (var stream = FileSystem.OpenWrite(fileName))
            {
                serializer.Serialize(stream, _fieldRecords);
            }
        }

        [UsedImplicitly]
        public class FieldRecord
        {
            public FieldRecord()
            {
            }

            public FieldRecord(Guid itemId, [NotNull] string databaseName, Guid fieldId, [NotNull] string fieldName, [NotNull] string language, int version, [NotNull] string value)
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

            [XmlAttribute]
            public Guid FieldId { get; set; }

            [NotNull, XmlAttribute]
            public string FieldName { get; set; } = string.Empty;

            [XmlIgnore]
            public bool IsTouched { get; set; }

            [XmlAttribute]
            public Guid ItemId { get; set; } = Guid.Empty;

            [NotNull, DefaultValue(""), XmlAttribute]
            public string Language { get; set; } = string.Empty;

            [NotNull, DefaultValue("")]
            public string Value { get; set; } = string.Empty;

            [DefaultValue(0), XmlAttribute]
            public int Version { get; set; }
        }
    }
}
