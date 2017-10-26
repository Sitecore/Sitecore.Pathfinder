using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    public abstract class SqlGeneratorBase : ISqlGenerator
    {
        [NotNull]
        protected Dictionary<string, object> processedItems = new Dictionary<string, object>();

        public string GenerateAddBlobStatements(Blob blob)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($@"
INSERT INTO #Blobs (BlobId, [Index], Data, Created)
VALUES ('{blob.Id}', '{0}', 0x{FormatBlobValue(blob.Data)}, GETUTCDATE())");

            stringBuilder.AppendLine("GO");

            return stringBuilder.ToString();
        }

        public virtual string GenerateAddItemStatements(VersionedItem item)
        {
            var stringBuilder = new StringBuilder();
            var itemTableOperations = this.ScriptItemTableOperations(item);
            if (!string.IsNullOrEmpty(itemTableOperations))
            {
                stringBuilder.AppendLine(itemTableOperations);
            }

            var sharedFieldsTableOperations = this.ScriptSharedFieldsTableOperations(item);
            if (!string.IsNullOrEmpty(sharedFieldsTableOperations))
            {
                stringBuilder.AppendLine(sharedFieldsTableOperations);
            }

            return stringBuilder.ToString();
        }

        public string GenerateAddRoleStatements(Role role)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DECLARE @applicationId nvarchar(256)");

            stringBuilder.AppendLine(
                "SELECT TOP 1 @applicationId = [ApplicationId] FROM [aspnet_Applications] WHERE [ApplicationName] = 'sitecore'");
            stringBuilder.AppendLine(
                $"IF NOT EXISTS (SELECT TOP 1 [RoleId] FROM [aspnet_Roles] WHERE [ApplicationId] = @applicationId AND [RoleName] = '{role.Name}')");
            stringBuilder.AppendLine("BEGIN");
            stringBuilder.AppendLine(
                "    INSERT INTO [aspnet_Roles] (ApplicationId, RoleId, RoleName, LoweredRoleName, Description)");
            stringBuilder.AppendLine(
                $"    VALUES (@applicationId, NEWID(), '{role.Name}', LOWER('{role.Name}'), NULL)");
            stringBuilder.AppendLine("END");

            foreach (var memberOf in role.Membership)
            {
                stringBuilder.AppendLine(
                    $"IF NOT EXISTS (SELECT TOP 1 * FROM [RolesInRoles] WHERE [MemberRoleName] = '{role.Name}' AND [TargetRoleName] = '{memberOf}')");
                stringBuilder.AppendLine("BEGIN");
                stringBuilder.AppendLine(
                    "    INSERT INTO [RolesInRoles] (Id, MemberRoleName, TargetRoleName, ApplicationName, Created)");
                stringBuilder.AppendLine($"    VALUES (NEWID(), '{role.Name}', '{memberOf}', '', SYSUTCDATETIME())");
                stringBuilder.AppendLine("END");
            }

            stringBuilder.AppendLine("GO");

            return stringBuilder.ToString();
        }

        public virtual string GeneratePrependStatements()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("BEGIN TRANSACTION;");
            this.AddCreateTableStatements(stringBuilder);
            return stringBuilder.ToString();
        }

        public virtual string GenerateAppendStatements()
        {
            var stringBuilder = new StringBuilder();
            this.AddPreprocessStatements(stringBuilder);
            this.AddMergeStatements(stringBuilder);
            this.AddDropTemporaryTablesStatements(stringBuilder);

            stringBuilder.AppendLine("COMMIT TRANSACTION;");
            return stringBuilder.ToString();
        }

        [NotNull]
        public static string FormatBlobValue([NotNull] byte[] value)
        {
            var hexString = new StringBuilder(value.Length * 2);
            foreach (var b in value)
            {
                hexString.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            }

            return hexString.ToString();
        }

        [NotNull]
        protected static string FormatValue([NotNull] string value)
        {
            var result = value.Replace("'", "''").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Replace("$(", "$' as nvarchar(max)),cast(N'(");

            // If script contains replacement for sign '$('- wrap statement into concat
            if (result.Contains("$' as nvarchar(max)),cast(N'("))
            {
                return $"concat(cast(N'{result}' as nvarchar(max)))";
            }

            return $"'{result}'";
        }

        protected virtual void AddCreateTableStatements([NotNull] StringBuilder result)
        {
            result.AppendLine(
                "CREATE TABLE #Blobs (BlobId uniqueidentifier NOT NULL, [Index] int NOT NULL, Data image NOT NULL, Created datetime NOT NULL);");
            result.AppendLine(
                $"CREATE TABLE #Items (ID uniqueidentifier NOT NULL PRIMARY KEY, Name nvarchar(256) COLLATE database_default NOT NULL, TemplateID uniqueidentifier NOT NULL, MasterID uniqueidentifier NOT NULL, ParentID uniqueidentifier NOT NULL, Created datetime NOT NULL, Updated datetime NOT NULL);");
            result.AppendLine(
                "CREATE TABLE #SharedFields (ItemId uniqueidentifier NOT NULL, FieldId uniqueidentifier NOT NULL, Value nvarchar(max) COLLATE database_default NOT NULL, Created datetime NOT NULL, Updated datetime NOT NULL, PRIMARY KEY CLUSTERED ([ItemID], [FieldID]));");
        }

        protected virtual void AddMergeStatements([NotNull] StringBuilder result)
        {
            result.AppendLine(@"
MERGE [Blobs] AS target
USING #Blobs AS source
ON target.BlobId = source.BlobId AND target.[Index] = source.[Index]
WHEN MATCHED THEN
    UPDATE SET
        Data = source.Data
WHEN NOT MATCHED THEN
    INSERT (Id, BlobId, [Index], Data, Created)
    VALUES (NEWID(), source.BlobId, source.[Index], source.Data, source.Created);
");
            result.AppendLine(@"
MERGE [Items] AS target
USING #Items AS source
ON target.ID = source.ID
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        TemplateID = source.TemplateID,
        MasterID = source.MasterID,
        ParentID = source.ParentID,
        Updated = source.Updated
WHEN NOT MATCHED THEN
    INSERT (ID, Name, TemplateID, MasterID, ParentID, Created, Updated)
    VALUES (source.ID, source.Name, source.TemplateID, source.MasterID, source.ParentID, source.Created, source.Updated);
");
            result.AppendLine(@"
MERGE [SharedFields] AS target
USING #SharedFields AS source
ON target.ItemId = source.ItemId AND target.FieldId = source.FieldId
WHEN MATCHED THEN
    UPDATE SET
        Value = source.Value,
        Updated = source.Updated
WHEN NOT MATCHED THEN
    INSERT (Id, ItemId, FieldId, Value, Created, Updated)
    VALUES (NEWID(), source.ItemId, source.FieldId, source.Value, source.Created, source.Updated);
");
        }

        protected virtual void AddPreprocessStatements([NotNull] StringBuilder result)
        {
        }

        protected virtual void AddDropTemporaryTablesStatements([NotNull] StringBuilder result)
        {
            result.AppendLine("DROP TABLE #Blobs");
            result.AppendLine("DROP TABLE #Items");
            result.AppendLine("DROP TABLE #SharedFields");
        }

        [NotNull]
        protected string ScriptItemTableOperations([NotNull] Item item)
        {
            var key = $"i{item.Database}{item.Uri.Guid.Format()}";
            if (this.processedItems.ContainsKey(key))
            {
                return string.Empty;
            }

            this.processedItems[key] = null;

            return $@"
INSERT INTO #Items (ID, Name, TemplateID, MasterID, ParentID, Created, Updated)
    VALUES ('{item.Uri.Guid.Format()}', {FormatValue(item.ItemName)}, '{item.Template.Uri.Guid.Format()}', '{Constants.NullGuidString}', '{item.GetParent().Uri.Guid.Format()}', GETUTCDATE(), GETUTCDATE());";
        }

        [NotNull]
        protected string ScriptSharedFieldsTableOperations([NotNull] Item item)
        {
            var stringBuilder = new StringBuilder();

            foreach (var field in item.Fields.Where(f => f.TemplateField.Shared))
            {
                var key = $"s{item.Database}{item.Uri.Guid.Format()}{field.FieldId.Format()}";
                if (this.processedItems.ContainsKey(key))
                {
                    continue;
                }

                this.processedItems[key] = null;

                stringBuilder.AppendLine("INSERT INTO #SharedFields (ItemId, FieldId, Value, Created, Updated)");
                stringBuilder.AppendLine(
                    $"    VALUES ('{item.Uri.Guid.Format()}', '{field.FieldId.Format()}', {FormatValue(field.CompiledValue)}, GETUTCDATE(), GETUTCDATE());");
            }

            return stringBuilder.ToString();
        }
    }
}
