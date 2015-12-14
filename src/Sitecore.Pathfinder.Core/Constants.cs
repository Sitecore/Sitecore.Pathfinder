// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder
{
    public static class Constants
    {
        public const string ExtensionsAssemblyFileName = "Sitecore.Pathfinder.Checker.dll";

        [NotNull]
        public static readonly char[] Comma =
        {
            ','
        };

        [NotNull, ItemNotNull]
        public static readonly IList<ITextNode> EmptyReadOnlyTextNodeCollection = new ReadOnlyCollection<ITextNode>(new List<ITextNode>());

        [NotNull]
        public static string NullGuidString = Guid.Empty.Format();

        [NotNull]
        public static readonly char[] Pipe =
        {
            '|'
        };

        [NotNull]
        public static readonly char[] Semicolon =
        {
            ';'
        };

        [NotNull]
        public static readonly char[] Slash =
        {
            '/'
        };

        [NotNull]
        public static readonly char[] Space =
        {
            ' '
        };

        public static class Configuration
        {
            public const string BuildingWithNoConfig = "building-with-no-config";

            public const string BuildProject = "build-project:tasks";

            public const string BuildProjectForceUpdate = "build-project:force-update";

            public const string BuildProjectMediaTemplate = "build-project:media:template";

            public const string CheckBinFileVersion = "install-package:check-bin-file-version";

            public const string CheckProjectDisabledCategories = "check-project:disabled-categories";

            public const string CheckProjectDisabledCheckers = "check-project:disabled-checkers";

            public const string CodeGen = "codegen";

            public const string CommandLineConfig = "config";

            public const string CopyDependenciesSourceDirectory = "copy-dependencies:source-directory";

            public const string Database = "database";

            public const string DataFolderDirectory = "data-folder-directory";

            public const string Debug = "debug";

            public const string GenerateCodeImports = "generate-code:t4-imports";

            public const string GenerateCodeIncludePaths = "generate-code:t4-include-paths";

            public const string GenerateCodeNameToken = "generate-code:name-replacement-token";

            public const string GenerateCodeReferencePaths = "generate-code:t4-reference-paths";

            public const string GenerateCodeRefs = "generate-code:t4-refs";

            public const string HostName = "host-name";

            public const string InstallUrl = "install-package:install-url";

            public const string LocalTestDirectory = "run-unittests:local-test-directory";

            public const string MappingContentFiles = "project-website-mappings:content-files";

            public const string MappingFileSearchPattern = "project-website-mappings:file-search-pattern";

            public const string MappingIgnoreDirectories = "project-website-mappings:ignore-directories";

            public const string MappingIgnoreFileNames = "project-website-mappings:ignore-filenames";

            public const string MessagesDisabled = "messages:disabled";

            public const string PackageDirectory = "copy-package:package-directory";

            public const string PackNugetDirectory = "pack-nuget:directory";

            public const string PackNugetExclude = "pack-nuget:exclude";

            public const string PackNugetInclude = "pack-nuget:include";

            public const string Password = "password";

            public const string ProjectConfigFileName = "project";

            public const string ProjectDirectory = "project-directory";

            public const string ProjectExtensionsAssemblyFileName = "extensions:project-extensions-assembly-filename";

            public const string ProjectExtensionsDirectory = "extensions:project-extensions-directory";

            public const string ProjectUniqueId = "project-unique-id";

            public const string ProjectWebsiteMappingsExcludedFields = "project-website-mappings:excluded-fields";

            public const string PublishUrl = "publish-databases:publish-url";

            public const string RemapFileDirectories = "remap-file-directories";

            public const string ScriptExtensions = "scripts:file-extensions";

            public const string StandardTemplateFields = "standard-template-fields";

            public const string SystemConfigFileName = "system:config";

            public const string ToolsDirectory = "system:toolspath";

            public const string UninstallDirectory = "install-package:uninstall-directory";

            public const string UpdateResourcesUrl = "sync-website:sync-url";

            public const string UserName = "user-name";

            public const string WatchProjectExclude = "watch-project:exclude";

            public const string WatchProjectInclude = "watch-project:include";

            public const string WatchProjectPublishDatabase = "watch-project:publish-database";

            public const string WebsiteDirectory = "website-directory";

            public const string WebTestRunnerName = "run-unittests:web-test-runner-name";

            public const string WebTestRunnerUrl = "run-unittests:web-test-runner-url";

            public const string WriteExportsFieldsToWrite = "write-exports:fields-to-write";

            public const string WriteExportsFileName = "write-exports:filename";

            public const string WriteSerializationDirectory = "write-serialization:directory";

            public const string WriteSerializationFlat = "write-serialization:flat";

            public const string WwwrootDirectory = "new-project:wwwroot-repository";

            public static class ValidateWebsite
            {
                public const string InactiveValidations = "validate-website:inactive-validations";

                public const string Languages = "validate-website:languages";

                public const string ProcessSiteValidation = "validate-website:process-site-validations";

                public const string RootItemPath = "validate-website:root-item-path";

                public const string Site = "validate-website:site";

                public const string Timeout = "validate-website:timeout";

                public const string Url = "validate-website:url";
            }
        }

        public static class Emitters
        {
            public const double BinFiles = 9999;

            public const double ContentFiles = 4000;

            public const double Items = 2000;

            public const double Layouts = 2500;

            public const double MediaFiles = 1500;

            public const double Templates = 1000;
        }

        public static class FieldCompilers
        {
            public const double High = 500;

            public const double Low = 2000;

            public const double Normal = 1000;
        }

        public static class Fields
        {
            public const string InsertOptionsFieldId = "{1172F251-DAD4-4EFB-A329-0C63500E4F1E}";

            public const string IsEmittable = "IsEmittable";

            public const string IsImport = "IsExternalReference";

            public static readonly Guid ArchiveDate = new Guid("{56C15C6D-FD5A-40CA-BB37-64CEEC6A9BD5}");

            public static readonly Guid NeverPublish = new Guid("{9135200A-5626-4DD8-AB9D-D665B8C11748}");

            public static readonly Guid PublishDate = new Guid("{86FE4F77-4D9A-4EC3-9ED9-263D03BD1965}");

            public static readonly Guid ReminderDate = new Guid("{ABE5D54C-59D7-41E6-8D3F-C1A3E4EC9B9E}");

            public static readonly Guid UnpublishDate = new Guid("{7EAD6FD6-6CF1-4ACA-AC6B-B200E7BAFE88}");

            public static readonly Guid Updated = new Guid("{D9CF14B1-FA16-4BA6-9288-E8A174D4D522}");

            public static readonly Guid UpdatedBy = new Guid("{BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}");

            public static readonly Guid ValidFrom = new Guid("{C8F93AFE-BFD4-4E8F-9C61-152559854661}");

            public static readonly Guid ValidTo = new Guid("{4C346442-E859-4EFD-89B2-44AEDF467D21}");
        }

        public static class Parsers
        {
            public const double BinFiles = 9999;

            public const double ContentFiles = 9000;

            public const double Items = 3000;

            public const double Media = 2000;

            public const double Renderings = 5000;

            public const double System = 100;

            public const double Templates = 1000;
        }

        public static class Templates
        {
            public const string Layout = "{3A45A723-64EE-4919-9D41-02FD40FD1466}";

            public const string StandardTemplate = "{1930BBEB-7805-471A-A3BE-4858AC7CF696}";

            public const string Sublayout = "{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}";

            public const string Template = "{AB86861A-6030-46C5-B394-E8F99E8B87DB}";

            public const string TemplateField = "{455A3E98-A627-4B40-8035-E683A0331AC7}";

            public const string TemplatePath = "/sitecore/templates/System/Templates/Template";

            public const string TemplateSection = "{E269FBB5-3750-427A-9149-7AA950B49301}";

            public const string ViewRendering = "{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}";
        }

        public static class TextNodeParsers
        {
            public const double Content = 9999;

            public const double Items = 1000;

            public const double Layouts = 1000;

            public const double Templates = 1000;
        }
    }
}
