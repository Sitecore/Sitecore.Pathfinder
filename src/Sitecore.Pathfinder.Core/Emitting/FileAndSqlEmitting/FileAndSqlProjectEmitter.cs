// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.IO;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Emitting.FileAndSqlEmitting
{
    [Export(typeof(FileAndSqlProjectEmitter))]
    public class FileAndSqlProjectEmitter : ProjectEmitter
    {
        [NotNull]
        private readonly Dictionary<string, DatabaseSql> _databases = new Dictionary<string, DatabaseSql>();

        [ImportingConstructor]
        public FileAndSqlProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IEmitter> emitters) : base(configuration, compositionService, traceService, emitters)
        {
        }

        public override void Emit(IProject project)
        {
            base.Emit(project);

            GetConnectionStringsFromWebsite();
            GetConnectionStringsFromConfiguration();

            ExecuteSql();
        }

        protected virtual void ExecuteSql()
        {
            foreach (var database in _databases)
            {
                // database.Value.Sql.WriteLine("SET NOCOUNT OFF;");

                if (string.IsNullOrEmpty(database.Value.ConnectionString))
                {
                    Trace.TraceError(Msg.E1041, "ConnectionString for database not found", database.Key);
                    continue;
                }

                try
                {
                    using (var connection = new SqlConnection(database.Value.ConnectionString))
                    {
                        var command = new SqlCommand(database.Value.Sql.ToString(), connection);
                        command.Connection.Open();

                        // File.WriteAllText("e:\\" + database.Key + ".sql", database.Value.Sql.ToString());
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

        public virtual void WriteItem([NotNull] Item item)
        {
            DatabaseSql databaseSql;
            if (!_databases.TryGetValue(item.DatabaseName, out databaseSql))
            {
                databaseSql = new DatabaseSql();
                _databases[item.DatabaseName] = databaseSql;

                databaseSql.Sql.WriteLine("SET NOCOUNT ON;");
            }

            var itemId = item.Uri.Guid.ToString("D");
            var templateId = item.Template.Uri.Guid.ToString("D");
            var masterId = Guid.Empty.ToString("D");
            var parentId = (item.GetParent()?.Uri.Guid ?? Guid.Empty).ToString("D");
            var updated = item.Statistics.Updated == DateTime.MinValue ? DateTime.UtcNow.ToString("yyyy/MM/dd hh:mm:ss") : item.Statistics.Updated.ToString("yyyy/MM/dd hh:mm:ss");
            var created = item.Statistics.Created == DateTime.MinValue ? DateTime.UtcNow.ToString("yyyy/MM/dd hh:mm:ss") : item.Statistics.Created.ToString("yyyy/MM/dd hh:mm:ss");

            databaseSql.Sql.WriteLine($"-- {item.Paths.Path}");
            databaseSql.Sql.WriteLine($"UPDATE Items SET Name='{item.ItemName}', TemplateID='{templateId}', ParentID='{parentId}', Updated='{updated}' WHERE ID='{itemId}'");
            databaseSql.Sql.WriteLine("IF @@ROWCOUNT = 0");
            databaseSql.Sql.WriteLine($"  INSERT INTO Items (ID, Name, TemplateID, MasterID, ParentID, Created, Updated) VALUES ('{itemId}', '{item.ItemName}', '{templateId}', '{masterId}', '{parentId}', '{created}', '{updated}')");

            foreach (var field in item.Fields)
            {
                var tableName = field.TemplateField.Shared ? "SharedFields" : field.TemplateField.Unversioned ? "UnversionedFields" : "VersionedFields";
                var id = field.FieldId.ToString("D");
                var language = field.Language == Language.Undefined || field.Language == Language.Empty ? "en" : field.Language.LanguageName;
                var version = field.Version.Number == int.MinValue ? 1 : field.Version.Number;
                var fieldId = field.TemplateField.Uri.Guid.ToString("D");
                var value = field.Value.Replace("'", "''");

                // todo: update language and version as well
                databaseSql.Sql.WriteLine($"UPDATE {tableName} SET Value='{value}', Updated='{updated}' WHERE ID='{id}'");
                databaseSql.Sql.WriteLine("IF @@ROWCOUNT = 0");
                databaseSql.Sql.WriteLine("BEGIN");

                if (field.TemplateField.Shared)
                {
                    databaseSql.Sql.WriteLine($"  INSERT INTO SharedFields (ID, ItemID, FieldId, Value, Created, Updated) VALUES ('{id}', '{itemId}', '{fieldId}', '{value}', '{created}', '{updated}')");
                    databaseSql.Sql.WriteLine($"  DELETE FROM UnversionedFields WHERE ID='{id}'");
                    databaseSql.Sql.WriteLine($"  DELETE FROM VersionedFields WHERE ID='{id}'");
                }
                else if (field.TemplateField.Unversioned)
                {
                    databaseSql.Sql.WriteLine($"  INSERT INTO UnversionedFields (ID, ItemID, Language, FieldId, Value, Created, Updated) VALUES ('{id}', '{itemId}', '{language}', '{fieldId}', '{value}', '{created}', '{updated}')");
                    databaseSql.Sql.WriteLine($"  DELETE FROM SharedFields WHERE ID='{id}'");
                    databaseSql.Sql.WriteLine($"  DELETE FROM VersionedFields WHERE ID='{id}'");
                }
                else
                {
                    databaseSql.Sql.WriteLine($"  INSERT INTO VersionedFields (ID, ItemID, Language, Version, FieldId, Value, Created, Updated) VALUES ('{id}', '{itemId}', '{language}', '{version}', '{fieldId}', '{value}', '{created}', '{updated}')");
                    databaseSql.Sql.WriteLine($"  DELETE FROM SharedFields WHERE ID='{id}'");
                    databaseSql.Sql.WriteLine($"  DELETE FROM UnversionedFields WHERE ID='{id}'");
                }

                databaseSql.Sql.WriteLine("END");
            }
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
