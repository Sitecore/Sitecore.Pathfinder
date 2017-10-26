// © 2017 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.

using System.Linq;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    public class SitecorePackageSqlGenerator : SqlGeneratorBase
    {
        public override string GenerateAddItemStatements(VersionedItem item)
        {
            var stringBuilder = new StringBuilder(base.GenerateAddItemStatements(item));
            var unversionedFieldsTableOperations = ScriptUnversionedFieldsTableOperations(item);
            if (!string.IsNullOrEmpty(unversionedFieldsTableOperations))
            {
                stringBuilder.AppendLine(unversionedFieldsTableOperations);
            }
            var versionedFieldsTableOperations = ScriptVersionedFieldsTableOperations(item);
            if (!string.IsNullOrEmpty(versionedFieldsTableOperations))
            {
                stringBuilder.AppendLine(versionedFieldsTableOperations);
            }

            stringBuilder.AppendLine("GO");

            return stringBuilder.ToString();
        }

        protected override void AddCreateTableStatements(StringBuilder result)
        {
            base.AddCreateTableStatements(result);
            result.AppendLine(
                "CREATE TABLE #VersionedFields (ItemId uniqueidentifier NOT NULL, FieldId uniqueidentifier NOT NULL, Language nvarchar(50) COLLATE database_default NOT NULL, Version int NOT NULL, Value nvarchar(max) COLLATE database_default NOT NULL, Created datetime NOT NULL, Updated datetime NOT NULL, PRIMARY KEY CLUSTERED ([ItemID], [FieldID], [Language], [Version]));");
            result.AppendLine(
                "CREATE TABLE #UnversionedFields (ItemId uniqueidentifier NOT NULL, FieldId uniqueidentifier NOT NULL, Language nvarchar(50) COLLATE database_default NOT NULL, Value nvarchar(max) COLLATE database_default NOT NULL, Created datetime NOT NULL, Updated datetime NOT NULL, PRIMARY KEY CLUSTERED ([ItemID], [FieldID], [Language]));");
        }

        protected override void AddMergeStatements(StringBuilder result)
        {
            base.AddMergeStatements(result);

            result.AppendLine(@"
MERGE [UnversionedFields] AS target
USING #UnversionedFields AS source
ON target.ItemId = source.ItemId AND target.FieldId = source.FieldId AND target.Language = source.Language
WHEN MATCHED THEN
    UPDATE SET
        Value = source.Value,
        Updated = source.Updated
WHEN NOT MATCHED THEN
    INSERT (Id, ItemId, FieldId, Language, Value, Created, Updated)
    VALUES (NEWID(), source.ItemId, source.FieldId, source.Language, source.Value, source.Created, source.Updated);
");
            result.AppendLine(@"
MERGE [VersionedFields] AS target
USING #VersionedFields AS source
ON target.ItemId = source.ItemId AND target.FieldId = source.FieldId AND target.Language = source.Language AND target.Version = source.Version
WHEN MATCHED THEN
    UPDATE SET
        Value = source.Value,
        Updated = source.Updated
WHEN NOT MATCHED THEN
    INSERT (Id, ItemId, FieldId, Language, Version, Value, Created, Updated)
    VALUES (NEWID(), source.ItemId, source.FieldId, source.Language, source.Version, source.Value, source.Created, source.Updated);
");
        }

        protected override void AddDropTemporaryTablesStatements(StringBuilder result)
        {
            base.AddDropTemporaryTablesStatements(result);
            result.AppendLine("DROP TABLE #UnversionedFields");
            result.AppendLine("DROP TABLE #VersionedFields");
        }

        [NotNull]
        private string ScriptVersionedFieldsTableOperations([NotNull] VersionedItem item)
        {
            var stringBuilder = new StringBuilder();
            foreach (var field in item.Fields.Where(f => !f.TemplateField.Shared && !f.TemplateField.Unversioned))
            {
                var key = $"u{item.Database}{item.Uri.Guid.Format()}{field.FieldId.Format()}{item.Language}{item.Version}";
                if (processedItems.ContainsKey(key))
                {
                    continue;
                }

                processedItems[key] = null;
                stringBuilder.AppendLine(
                    "INSERT INTO #VersionedFields (ItemId, FieldId, Language, Version, Value, Created, Updated)");
                stringBuilder.AppendLine(
                    $"    VALUES ('{item.Uri.Guid.Format()}', '{field.FieldId.Format()}', '{item.Language}', '{item.Version}', {FormatValue(field.Value)}, GETUTCDATE(), GETUTCDATE());");
            }
            return stringBuilder.ToString();
        }

        [NotNull]
        private string ScriptUnversionedFieldsTableOperations([NotNull] VersionedItem item)
        {
            var stringBuilder = new StringBuilder();
            foreach (var field in item.Fields.Where(f => !f.TemplateField.Shared && f.TemplateField.Unversioned))
            {
                var key = $"u{item.Database}{item.Uri.Guid.Format()}{field.FieldId.Format()}{item.Language}";
                if (processedItems.ContainsKey(key))
                {
                    continue;
                }

                processedItems[key] = null;
                stringBuilder.AppendLine(
                    "INSERT INTO #UnversionedFields (ItemId, FieldId, Language, Value, Created, Updated)");
                stringBuilder.AppendLine(
                    $"    VALUES ('{item.Uri.Guid.Format()}', '{field.FieldId.Format()}', '{item.Language}', {FormatValue(field.Value)}, GETUTCDATE(), GETUTCDATE());");
            }

            return stringBuilder.ToString();
        }
    }
}
