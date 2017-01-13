// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Emitting.ItemsAndFilesEmitting
{
    [Export(typeof(ItemsAndFilesProjectEmitter))]
    public class ItemsAndFilesProjectEmitter : ProjectEmitter
    {
        [NotNull]
        private readonly Dictionary<string, DatabaseSql> _databases = new Dictionary<string, DatabaseSql>();

        [ImportingConstructor]
        public ItemsAndFilesProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IEmitter> emitters) : base(configuration, compositionService, traceService, emitters)
        {
        }

        public override void Emit(IProject project)
        {
            base.Emit(project);

            GetConnectionStringsFromWebsite();
            GetConnectionStringsFromConfiguration();

            ExecuteSql();
        }

        public virtual void WriteItem([NotNull] Item item)
        {
            DatabaseSql databaseSql;
            if (!_databases.TryGetValue(item.DatabaseName, out databaseSql))
            {
                databaseSql = new DatabaseSql();
                _databases[item.DatabaseName] = databaseSql;
            }

            var itemId = FormatGuid(item.Uri.Guid);
            var name = FormatString(item.ItemName);
            var templateId = FormatGuid(item.Template.Uri.Guid);
            var masterId = FormatGuid(Guid.Empty);
            var parentId = FormatGuid(item.GetParent()?.Uri.Guid ?? Guid.Empty);
            var updated = FormatDateTime(item.Statistics.Updated == DateTime.MinValue ? DateTime.UtcNow : item.Statistics.Updated);
            var created = FormatDateTime(item.Statistics.Created == DateTime.MinValue ? DateTime.UtcNow : item.Statistics.Created);

            databaseSql.Sql.WriteLine($"-- {item.Paths.Path}");
            databaseSql.Sql.WriteLine($"UPDATE [Items] SET [Name]={name}, [TemplateID]={templateId}, [ParentID]={parentId}, [Updated]={updated} WHERE [ID]={itemId}");
            databaseSql.Sql.WriteLine("IF @@ROWCOUNT = 0 BEGIN");
            databaseSql.Sql.WriteLine($"  INSERT INTO [Items] ([ID], [Name], [TemplateID], [MasterID], [ParentID], [Created], [Updated]) VALUES ({itemId}, {name}, {templateId}, {masterId}, {parentId}, {created}, {updated})");
            databaseSql.Sql.WriteLine("END");

            var fields = new List<Field>(item.Fields);

            // make sure item has a version - otherwise it cannot be published
            if (!item.Fields.Any())
            {
                var defaultLanguage = Configuration.GetString(Constants.Configuration.DefaultLanguage);

                var id = FormatGuid(Guid.NewGuid());
                var fieldId = FormatGuid(Constants.Fields.CreatedBy);
                var value = FormatString("sitecore/admin");
                var language = FormatString(item.Database.GetLanguage(defaultLanguage).LanguageName);
                var version = 1;

                databaseSql.Sql.WriteLine($"UPDATE [VersionedFields] SET [Value]={value}, [Updated]={updated} WHERE [ItemID]={itemId} and [FieldID]={fieldId} and [Language]={language} and [Version]={version}");
                databaseSql.Sql.WriteLine("IF @@ROWCOUNT = 0 BEGIN");
                databaseSql.Sql.WriteLine($"  INSERT INTO [VersionedFields] ([ID], [ItemID], [Language], [Version], [FieldId], [Value], [Created], [Updated]) VALUES ({id}, {itemId}, {language}, {version}, {fieldId}, {value}, {created}, {updated})");
                databaseSql.Sql.WriteLine("END");
            }

            foreach (var field in item.Fields)
            {
                var id = FormatGuid(field.FieldId);
                var language = FormatString(field.Language == Language.Undefined || field.Language == Language.Empty ? "en" : field.Language.LanguageName);
                var version = field.Version.Number == int.MinValue ? 1 : field.Version.Number;
                var fieldId = FormatGuid(field.TemplateField.Uri.Guid);
                var value = FormatString(field.Value);

                if (field.TemplateField.Shared)
                {
                    databaseSql.Sql.WriteLine($"UPDATE [SharedFields] SET [Value]={value}, [Updated]={updated} WHERE [ItemID]={itemId} and [FieldID]={fieldId}");
                    databaseSql.Sql.WriteLine("IF @@ROWCOUNT = 0 BEGIN");
                    databaseSql.Sql.WriteLine($"  INSERT INTO [SharedFields] ([ID], [ItemID], [FieldID], [Value], [Created], [Updated]) VALUES ({id}, {itemId}, {fieldId}, {value}, {created}, {updated})");
                    databaseSql.Sql.WriteLine($"  DELETE FROM [UnversionedFields] WHERE [ItemID]={itemId} and [FieldID]={fieldId}");
                    databaseSql.Sql.WriteLine($"  DELETE FROM [VersionedFields] WHERE [ItemID]={itemId} and [FieldID]={fieldId}");
                    databaseSql.Sql.WriteLine("END");
                }
                else if (field.TemplateField.Unversioned)
                {
                    databaseSql.Sql.WriteLine($"UPDATE [UnversionedFields] SET [Value]={value}, [Updated]={updated} WHERE [ItemID]={itemId} and [FieldID]={fieldId} and [Language]={language}");
                    databaseSql.Sql.WriteLine("IF @@ROWCOUNT = 0 BEGIN");
                    databaseSql.Sql.WriteLine($"  INSERT INTO [UnversionedFields] ([ID], [ItemID], [Language], [FieldId], [Value], [Created], [Updated]) VALUES ({id}, {itemId}, {language}, {fieldId}, {value}, {created}, {updated})");
                    databaseSql.Sql.WriteLine($"  DELETE FROM [SharedFields] WHERE [ItemID]={itemId} and [FieldID]={fieldId}");
                    databaseSql.Sql.WriteLine($"  DELETE FROM [VersionedFields] WHERE [ItemID]={itemId} and [FieldID]={fieldId}");
                    databaseSql.Sql.WriteLine("END");
                }
                else
                {
                    databaseSql.Sql.WriteLine($"UPDATE [VersionedFields] SET [Value]={value}, [Updated]={updated} WHERE [ItemID]={itemId} and [FieldID]={fieldId} and [Language]={language} and [Version]={version}");
                    databaseSql.Sql.WriteLine("IF @@ROWCOUNT = 0 BEGIN");
                    databaseSql.Sql.WriteLine($"  INSERT INTO [VersionedFields] ([ID], [ItemID], [Language], [Version], [FieldId], [Value], [Created], [Updated]) VALUES ({id}, {itemId}, {language}, {version}, {fieldId}, {value}, {created}, {updated})");
                    databaseSql.Sql.WriteLine($"  DELETE FROM [SharedFields] WHERE [ItemID]={itemId} and [FieldID]={fieldId}");
                    databaseSql.Sql.WriteLine($"  DELETE FROM [UnversionedFields] WHERE [ItemID]={itemId} and [FieldID]={fieldId}");
                    databaseSql.Sql.WriteLine("END");
                }
            }

            // todo: handle blob fields
        }

        protected virtual void ExecuteSql()
        {
            foreach (var database in _databases)
            {
                // File.WriteAllText("e:\\" + database.Key + ".sql", database.Value.Sql.ToString());

                if (string.IsNullOrEmpty(database.Value.ConnectionString))
                {
                    Trace.TraceError(Msg.E1041, "ConnectionString for database not found", database.Key);
                    continue;
                }

                try
                {
                    using (var connection = new SqlConnection(database.Value.ConnectionString))
                    {
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandText = database.Value.Sql.ToString();

                        var count = command.ExecuteNonQuery();

                        Trace.TraceInformation("SQL Rows affected: " + count.ToString("#,##0"));
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(Msg.E1040, "Failed to execute SQL", ex.Message);
                }
            }
        }

        [NotNull]
        protected string FormatDateTime(DateTime dateTime)
        {
            return FormatString(dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
        }

        [NotNull]
        protected string FormatGuid(Guid guid)
        {
            return "'" + guid.ToString("D") + "'";
        }

        [NotNull]
        protected string FormatString([NotNull] string s)
        {
            return "N'" + s.Replace("'", "''") + "'";
        }

        protected virtual void GetConnectionStringsFromConfiguration()
        {
            foreach (var database in _databases)
            {
                var key = "copy-project:connection-strings:" + database.Key;
                var connectionString = Configuration.GetString(key);

                if (!string.IsNullOrEmpty(connectionString))
                {
                    database.Value.ConnectionString = connectionString;
                }
            }
        }

        protected virtual void GetConnectionStringsFromWebsite()
        {
            var connectionStringsFileName = Path.Combine(Configuration.GetWebsiteDirectory(), "App_Config\\ConnectionStrings.config");

            if (!File.Exists(connectionStringsFileName))
            {
                return;
            }

            var root = File.ReadAllText(connectionStringsFileName).ToXElement();
            if (root == null)
            {
                return;
            }

            foreach (var element in root.Elements())
            {
                var databaseName = element.GetAttributeValue("name");
                var connectionString = element.GetAttributeValue("connectionString");

                DatabaseSql databaseSql;
                if (_databases.TryGetValue(databaseName, out databaseSql))
                {
                    databaseSql.ConnectionString = connectionString;
                }
            }
        }

        private class DatabaseSql
        {
            [NotNull]
            public string ConnectionString { get; set; } = string.Empty;

            [NotNull]
            public StringWriter Sql { get; } = new StringWriter();
        }
    }
}
