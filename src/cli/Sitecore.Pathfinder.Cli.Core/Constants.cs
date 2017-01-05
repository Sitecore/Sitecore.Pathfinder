// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace Sitecore.Pathfinder
{
    public static class Constants
    {
        public const string ExtensionsAssemblyFileName = "Sitecore.Pathfinder.Checker.dll";

        public const string SafeProjectUniqueIdRegex = @"[^a-zA-Z0-9_\.]";

        [NotNull]
        public static readonly char[] Comma =
        {
            ','
        };

        [NotNull, ItemNotNull]
        public static readonly ICollection<ITextNode> EmptyReadOnlyTextNodeCollection = new ReadOnlyCollection<ITextNode>(new List<ITextNode>());

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

        public static class Cache
        {
            public const string NugetRepositories = "NugetRepositories";

            public const string ProjectPackages = "ProjectPackages";
        }

        public static class Configuration
        {
            public const string BinDirectory = "bin-directory";

            public const string Checkers = "checkers";

            public const string CommandLineConfig = "config";

            public const string CopyPackage = "copy-package";

            public const string Culture = "culture";

            public const string Database = "database";

            public const string DataFolderDirectory = "data-folder-directory";

            public const string Debug = "debug";

            public const string Dependencies = "dependencies";

            public const string DisableExtensions = "disable-extensions";

            public const string FeaturesDirectory = "features-directory";

            public const string HostName = "host-name";

            public const string IsProjectConfigured = "is-project-configured";

            public const string NugetPackageRootDirectory = "system:nuget-package-root-directory";

            public const string NugetRepositories = "nuget-repositories";

            public const string Password = "password";

            public const string ProjectConfigFileName = "project";

            public const string ProjectDirectory = "project-directory";

            public const string ProjectRole = "project-role";

            public const string ProjectRoleCheckers = "project-role-checkers";

            public const string ProjectUniqueId = "project-unique-id";

            public const string Run = "run";

            public const string SearchAndReplaceTokens = "search-and-replace-tokens";

            public const string StandardTemplateFields = "standard-template-fields";

            public const string SystemConfigFileName = "system:config";

            public const string ToolsDirectory = "system:toolspath";

            public const string UserName = "user-name";

            public const string WebsiteDirectory = "website-directory";

            public static class BuildProject
            {
                public const string CompileBinFilesExclude = "build-project:compile-bin-files:exclude";

                public const string CompileBinFilesInclude = "build-project:compile-bin-files:include";

                public const string ForceUpdate = "build-project:force-update";

                public const string MediaTemplate = "build-project:media:template";

                public const string ParseAllFiles = "build-project:parse-all-files";

                public const string RunValidators = "build-project:run-sitecore-validators";

                public const string Tasks = "build-project:tasks";

                public static class Renderings
                {
                    public const string CreateItemsForPartialViews = "build-project:renderings:create-items-for-partial-views";
                }
            }

            public static class CheckProject
            {
                public const string Checkers = "check-project:checkers";

                public const string ConfigurationCheckerDevAssemblies = "check-project:ConfigurationChecker:dev-assemblies";

                public const string IgnoredReferences = "check-project:ignored-references";

                public const string PathFields = "check-project:path-fields";

                public const string StopOnErrors = "check-project:stop-on-errors";

                public const string TreatWarningsAsErrors = "check-project:treat-warnings-as-errors";
            }

            public static class CopyDependencies
            {
                public const string SourceDirectory = "copy-dependencies:source-directory";
            }

            public static class Extensions
            {
                public const string AssemblyFileName = "extensions:project-extensions-assembly-filename";

                public const string Directory = "extensions:project-extensions-directory";

                public const string ExternalAssemblyDirectories = "extensions:external-assembly-directories";
            }

            public static class GenerateCode
            {
                public const string Imports = "generate-code:t4-imports";

                public const string IncludePaths = "generate-code:t4-include-paths";

                public const string NameToken = "generate-code:name-replacement-token";

                public const string ReferencePaths = "generate-code:t4-reference-paths";

                public const string Refs = "generate-code:t4-refs";
            }

            public static class InstallPackage
            {
                public const string AddProjectDirectoriesAsFeeds = "install-package:add-project-directories-as-feeds";

                public const string CheckBinFileSizeAndTimestamp = "install-package:check-bin-file-size-and-timestamp";

                public const string CheckBinFileVersion = "install-package:check-bin-file-version";

                public const string DeleteProjectItems = "install-package:delete-project-items";

                public const string InstallUrl = "install-package:install-url";

                public const string MarkItemsWithPathfinderProjectUniqueId = "install-package:mark-items-with-pathfinder-project-unique-id";

                public const string ShowDiagnostics = "install-package:show-diagnostics";

                public const string ThreeWayMerge = "install-package:three-way-merge";

                public const string ThreeWayMergeOverwriteDatabase = "install-package:three-way-merge-overwrite-database";
            }

            public static class Messages
            {
                public const string Disabled = "messages:disabled";
            }

            public static class NewProject
            {
                public const string DefaultDataFolderDirectory = "new-project:default-data-folder-directory";

                public const string DefaultHostName = "new-project:default-host-name";

                public const string DefaultWebsiteDirectory = "new-project:default-wwwroot-directory";

                public const string WwwrootDirectory = "new-project:wwwroot-directory";
            }

            public static class Packages
            {
                public const string IncludePackagesConfigAsDependencies = "packages:include-packages-config-as-dependencies";

                public const string NpmDirectory = "packages:npm-directory";

                public const string NugetDirectory = "packages:nuget-directory";
            }

            public static class PackNpm
            {
                public const string OutputFile = "pack-npm:output-file";

                public const string PackageJsonFile = "pack-npm:package-json-file";
            }

            public static class PackNuGet
            {
                public const string BasePath = "pack-nuget:base-path";

                public const string Directory = "pack-nuget:directory";

                public const string Exclude = "pack-nuget:exclude";

                public const string Include = "pack-nuget:include";

                public const string Tokens = "pack-nuget:tokens";
            }

            public static class ProjectWebsiteMappings
            {
                public const string ContentFiles = "project-website-mappings:content-files";

                public const string ExcludedFields = "project-website-mappings:excluded-fields";

                public const string FileSearchPattern = "project-website-mappings:file-search-pattern";

                public const string IgnoreDirectories = "project-website-mappings:ignore-directories";

                public const string IgnoreFileNames = "project-website-mappings:ignore-filenames";
            }

            public static class PublishDatabases
            {
                public const string PublishUrl = "publish-databases:publish-url";
            }

            public static class RestorePackages
            {
                public const string Directory = "restore-packages:directory";
            }

            public static class Scripts
            {
                public const string Extensions = "scripts:file-extensions";
            }

            public static class ShowWebsite
            {
                public const string StartUrl = "show-website:start-url";
            }

            public static class StartWebsite
            {
                public const string Exclude = "start-website:exclude";

                public const string Include = "start-website:include";

                public const string Port = "start-website:port";
            }

            public static class System
            {
                public const string MultiThreaded = "system:multi-threaded";

                public const string ShowStackTrace = "system:show-stack-trace";

                public const string ShowTaskTime = "system:show-task-time";

                public static class WebRequests
                {
                    public const string Timeout = "system:web-requests: time-out";
                }
            }

            public static class WatchProject
            {
                public const string Exclude = "watch-project:exclude";

                public const string Include = "watch-project:include";

                public const string PublishDatabase = "watch-project:publish-database";

                public const string ResetWebsite = "watch-project:reset-website";
            }

            public static class WriteExports
            {
                public const string FieldsToWrite = "write-exports:fields-to-write";

                public const string FileName = "write-exports:filename";
            }

            public static class WriteSerialization
            {
                public const string Directory = "write-serialization:directory";

                public const string Flat = "write-serialization:flat";
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

            public static readonly Guid AppearanceEvaluatorType = new Guid("{DF36505E-A70A-4414-A9AE-AE26DB86FE19}");

            public static readonly Guid ArchiveDate = new Guid("{56C15C6D-FD5A-40CA-BB37-64CEEC6A9BD5}");

            public static readonly Guid ArchiveVersionDate = new Guid("{1D99005E-65CA-45CA-9D9A-FD7016E23F1E}");

            public static readonly Guid BaseTemplate = new Guid("{12C33F3F-86C5-43A5-AEB4-5598CEC45116}");

            public static readonly Guid Branches = new Guid("{1172F251-DAD4-4EFB-A329-0C63500E4F1E}");

            public static readonly Guid Command = new Guid("{854CC8F6-94AD-4521-A4B6-44ED8F794C98}");

            public static readonly Guid CommentDialogHeight = new Guid("{9D3D61EF-6F65-4F71-ABD4-9172549B15C2}");

            public static readonly Guid CommentTemplate = new Guid("{F1090046-2F13-4F19-BB41-C75DF4F89516}");

            public static readonly Guid ContextMenu = new Guid("{D3AE7222-425D-4B77-95D8-EE33AC2B6730}");

            public static readonly Guid Created = new Guid("{25BED78C-4957-4165-998A-CA1B52F67497}");

            public static readonly Guid CreatedBy = new Guid("{5DD74568-4D4B-44C1-B513-0AF5F4CDA34F}");

            public static readonly Guid DefaultCommentDialogHeight = new Guid("{591F16DA-4020-4FFD-9560-EF4996FC2A61}");

            public static readonly Guid DefaultCommentTemplate = new Guid("{2A59C3DC-E9AB-4108-BEB2-1CA9AEE56A33}");

            public static readonly Guid DefaultDomain = new Guid("{35374AF0-8C35-44C9-8580-783A9CD511E7}");

            public static readonly Guid DefaultWorkflow = new Guid("{CA9B9F52-4FB0-4F87-A79F-24DEA62CDA65}");

            public static readonly Guid DictionaryKey = new Guid("{580C75A8-C01A-4580-83CB-987776CEB3AF}");

            public static readonly Guid DictionaryPhrase = new Guid("{2BA3454A-9A9C-4CDF-A9F8-107FD484EB6E}");

            public static readonly Guid DisplayName = new Guid("{B5E02AD9-D56F-4C41-A065-A133DB87BDEB}");

            public static readonly Guid DomainMembershipProvGuider = new Guid("{E6509F86-520E-4F08-8D16-D9CB9A81E2FE}");

            public static readonly Guid DomainRoleNameTemplate = new Guid("{14CAC558-13DE-4795-A999-50A6B74D88BD}");

            public static readonly Guid DomainUniqueName = new Guid("{FF62DBCC-FEA8-4373-8014-09E97362911B}");

            public static readonly Guid DomainUserNameTemplate = new Guid("{B68F6FCE-A6BC-4A8D-930D-407F6F72E79C}");

            public static readonly Guid EditorPath = new Guid("{EED015E2-914C-469E-8B12-0B86EE18E5CF}");

            public static readonly Guid EnableItemFallback = new Guid("{FD4E2050-186C-4375-8B99-E8A85DD7436E}");

            public static readonly Guid EnableLanguageFallback = new Guid("{FA622538-0C13-4130-A001-45984241AA00}");

            public static readonly Guid EnableSharedLanguageFallback = new Guid("{24CB32F0-E364-4F37-B400-0F2899097B5B}");

            public static readonly Guid EnforceVersionPresence = new Guid("{61CF7151-0CBD-4DB4-9738-D753A55A6E65}");

            public static readonly Guid FallbackDomain = new Guid("{CD72E892-D8BB-487C-9EB1-6A57C48C72CC}");

            public static readonly Guid FallbackLanguage = new Guid("{892975AC-496F-4AC9-8826-087095C68E1D}");

            public static readonly Guid FinalLayoutField = new Guid("{04BF00DB-F5FB-41F7-8AB7-22408372A981}");

            public static readonly Guid HGuidden = new Guid("{39C4902E-9960-4469-AEEF-E878E9C8218F}");

            public static readonly Guid HGuideVersion = new Guid("{B8F42732-9CB8-478D-AE95-07E25345FB0F}");

            public static readonly Guid Icon = new Guid("{06D5295C-ED2F-4A54-9BF2-26228D113318}");

            public static readonly Guid InheritSecurity = new Guid("{6917D2D9-3F44-49FB-9319-0EB7D900D8DC}");

            public static readonly Guid LanguageIso = new Guid("{C437E416-8948-427D-A982-8ED37AE3F553}");

            public static readonly Guid LayoutField = new Guid("{F1A1FE9E-A60C-4DDB-A3A0-BB5B29FE732E}");

            public static readonly Guid Lock = new Guid("{001DD393-96C5-490B-924A-B0F25CD9EFD8}");

            public static readonly Guid NeverPublish = new Guid("{9135200A-5626-4DD8-AB9D-D665B8C11748}");

            public static readonly Guid NextState = new Guid("{DCBEBC58-6124-4100-A248-FC717D6C78D5}");

            public static readonly Guid Originator = new Guid("{F6D8A61C-2F84-4401-BD24-52D2068172BC}");

            public static readonly Guid Owner = new Guid("{52807595-0F8F-4B20-8D2A-CB71D28C6103}");

            public static readonly Guid PageDefinition = new Guid("{8FFF5707-C288-4CDF-B3CE-4851EAD9C16C}");

            public static readonly Guid Presentation = new Guid("{978EFCEB-A3B1-430F-9BB6-B8A93D436932}");

            public static readonly Guid Preview = new Guid("{41C6CC0E-389F-4D51-9990-FE35417B6666}");

            public static readonly Guid ProxyInsertionType = new Guid("{E4A55896-CF58-4C9C-8284-F01B141B0E85}");

            public static readonly Guid ProxySourceDatabase = new Guid("{02BD4E57-EBAC-4C04-935A-8A6B6D551C5C}");

            public static readonly Guid ProxySourceItem = new Guid("{C5211535-E9A7-4214-84BC-93F240A23602}");

            public static readonly Guid ProxyTargetItem = new Guid("{98B1EE13-D363-49FE-93C4-F48DC1191008}");

            public static readonly Guid PublishDate = new Guid("{86FE4F77-4D9A-4EC3-9ED9-263D03BD1965}");

            public static readonly Guid PublishingTargetDatabase = new Guid("{39ECFD90-55D2-49D8-B513-99D15573DE41}");

            public static readonly Guid PublishingTargets = new Guid("{74484BDF-7C86-463C-B49F-7B73B9AFC965}");

            public static readonly Guid ReadOnly = new Guid("{9C6106EA-7A5A-48E2-8CAD-F0F693B1E2D4}");

            public static readonly Guid Reference = new Guid("{C9CE97FB-3E8E-4384-9430-7DBA75B275DD}");

            public static readonly Guid ReminderDate = new Guid("{ABE5D54C-59D7-41E6-8D3F-C1A3E4EC9B9E}");

            public static readonly Guid ReminderRecipients = new Guid("{2ED9C4D0-9EFF-490D-A40A-B5D856499C40}");

            public static readonly Guid ReminderText = new Guid("{BB6C8540-118E-4C49-9157-830576D7345A}");

            public static readonly Guid Renderers = new Guid("{B03569B1-1534-43F2-8C83-BD064B7D782C}");

            public static readonly Guid Revision = new Guid("{8CDC337E-A112-42FB-BBB4-4143751E123F}");

            public static readonly Guid Ribbon = new Guid("{0C894AAB-962B-4A84-B923-CB24B05E60D2}");

            public static readonly Guid Security = new Guid("{DEC8D2D5-E3CF-48B6-A653-8E69E2716641}");

            public static readonly Guid Skin = new Guid("{079AFCFE-8ACA-4863-BDA7-07893541E2F5}");

            public static readonly Guid Sortorder = new Guid("{BA3F86A2-4A1C-4D78-B63D-91C2779C1B5E}");

            public static readonly Guid Source = new Guid("{1B86697D-60CA-4D80-83FB-7555A2E6CE1C}");

            public static readonly Guid StandardFieldsGuid = new Guid("{C29C1D32-9FB7-4DA5-8508-3FEC2AF3A81D}");

            public static readonly Guid StandardValueHolderGuid = new Guid("{716D0C7F-84F9-4fc8-9817-E489A8DC07AE}");

            public static readonly Guid StandardValues = new Guid("{F7D48A55-2158-4F02-9356-756654404F73}");

            public static readonly Guid State = new Guid("{3E431DE1-525E-47A3-B6B0-1CCBEC3A8C98}");

            public static readonly Guid Style = new Guid("{A791F095-2521-4B4D-BEF9-21DDA221F608}");

            public static readonly Guid SubitemsSorting = new Guid("{6FD695E7-7F6D-4CA5-8B49-A829E5950AE9}");

            public static readonly Guid Thumbnail = new Guid("{C7C26117-DBB1-42B2-AB5E-F7223845CCA3}");

            public static readonly Guid UnpublishDate = new Guid("{7EAD6FD6-6CF1-4ACA-AC6B-B200E7BAFE88}");

            public static readonly Guid Updated = new Guid("{D9CF14B1-FA16-4BA6-9288-E8A174D4D522}");

            public static readonly Guid UpdatedBy = new Guid("{BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}");

            public static readonly Guid UserMembership = new Guid("{1F37F0DD-4E61-4C90-984D-8B24317D6B32}");

            public static readonly Guid ValGuidFrom = new Guid("{C8F93AFE-BFD4-4E8F-9C61-152559854661}");

            public static readonly Guid ValGuidTo = new Guid("{4C346442-E859-4EFD-89B2-44AEDF467D21}");

            public static readonly Guid ValidFrom = new Guid("{C8F93AFE-BFD4-4E8F-9C61-152559854661}");

            public static readonly Guid ValidTo = new Guid("{4C346442-E859-4EFD-89B2-44AEDF467D21}");

            public static readonly Guid Workflow = new Guid("{A4F985D9-98B3-4B52-AAAF-4344F6E747C6}");

            public static readonly Guid WorkflowState = new Guid("{3E431DE1-525E-47A3-B6B0-1CCBEC3A8C98}");
        }

        public static class Layouts
        {
            public const string MvcLayout = "/sitecore/layout/layouts/Pathfinder/MvcLayout";
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
            public const string LayoutId = "{3A45A723-64EE-4919-9D41-02FD40FD1466}";

            public const string StandardTemplateId = "{1930BBEB-7805-471A-A3BE-4858AC7CF696}";

            public const string SublayoutId = "{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}";

            public const string TemplateFieldId = "{455A3E98-A627-4B40-8035-E683A0331AC7}";

            public const string TemplateId = "{AB86861A-6030-46C5-B394-E8F99E8B87DB}";

            public const string TemplatePathId = "/sitecore/templates/System/Templates/Template";

            public const string ViewRenderingId = "{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}";

            public static readonly Guid Alias = new Guid("{54BCFFB7-8F46-4948-AE74-DA5B6B5AFA86}");

            public static readonly Guid Application = new Guid("{EB06CEC0-5E2D-4DC4-875B-01ADCC577D13}");

            public static readonly Guid ArchivedItem = new Guid("{BF2B8DA2-3CBA-485D-8F85-3788B8AFBDBF}");

            public static readonly Guid BranchTemplate = new Guid("{35E75C72-4985-4E09-88C3-0EAC6CD1E64F}");

            public static readonly Guid BranchTemplateFolder = new Guid("{85ADBF5B-E836-4932-A333-FE0F9FA1ED1E}");

            public static readonly Guid Command = new Guid("{A66F4A32-23A6-4AC3-AB14-84F383C5F3BA}");

            public static readonly Guid CommandGroup = new Guid("{FBDD7D4F-3300-4432-9CB1-FDAD551DDD5E}");

            public static readonly Guid CommandMaster = new Guid("{B2613CC1-A748-46A3-A0DB-3774574BD339}");

            public static readonly Guid Device = new Guid("{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}");

            public static readonly Guid DictionaryDomain = new Guid("{0A2847E6-9885-450B-B61E-F9E6528480EF}");

            public static readonly Guid DictionaryEntry = new Guid("{6D1CD897-1936-4A3A-A511-289A94C2A7B1}");

            public static readonly Guid Domain = new Guid("{438F33BA-504D-4EFB-BE78-42B98603A7E8}");

            public static readonly Guid DynamicMaster = new Guid("{B4D19D07-B3EB-4F7D-98EC-8BCB41CCC58E}");

            public static readonly Guid File = new Guid("{611933AC-CE0C-4DDC-9683-F830232DB150}");

            public static readonly Guid Folder = new Guid("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}");

            public static readonly Guid HelpText = new Guid("{11A111FF-4343-4AF8-91DC-3C49DBEDD9C6}");

            public static readonly Guid Image = new Guid("{C97BA923-8009-4858-BDD5-D8BE5FCCECF7}");

            public static readonly Guid Language = new Guid("{F68F13A6-3395-426A-B9A1-FA2DC60D94EB}");

            public static readonly Guid Layout = new Guid("{3A45A723-64EE-4919-9D41-02FD40FD1466}");

            public static readonly Guid LayoutGroup = new Guid("{85B7F5D9-6F3A-4AD1-B218-D9FBE1DF8BB3}");

            public static readonly Guid LinkedDatabase = new Guid("{2085BFB3-1371-4092-BF06-32EF5849ED87}");

            public static readonly Guid MainSection = new Guid("{E3E2D58C-DF95-4230-ADC9-279924CECE84}");

            public static readonly Guid MediaFolder = new Guid("{FE5DD826-48C6-436D-B87A-7C4210C7413B}");

            public static readonly Guid MenuDivGuider = new Guid("{35753BF3-8A94-4DA6-A5D7-DBD945D1AA59}");

            public static readonly Guid MenuItem = new Guid("{998B965E-6AB8-4568-810F-8101D60D0CC3}");

            public static readonly Guid Node = new Guid("{239F9CF4-E5A0-44E0-B342-0F32CD4C6D8B}");

            public static readonly Guid Notification = new Guid("{35ABD142-200F-43F7-ADB6-0E10EDB82C0B}");

            public static readonly Guid PackageRegistration = new Guid("{22A11D20-5F1D-4216-BF3F-18C016F1F98E}");

            public static readonly Guid Preset = new Guid("{0FC09EA4-8D87-4B0E-A5C9-8076AE863D9C}");

            public static readonly Guid PresetPersona = new Guid("{448C4D57-F0F1-4278-8AB8-93DB46C94F1B}");

            public static readonly Guid Property = new Guid("{97D75760-CF8B-4740-810B-7727B564EF4D}");

            public static readonly Guid Proxy = new Guid("{CB3942DC-CBBA-4332-A7FB-C4E4204C538A}");

            public static readonly Guid Reference = new Guid("{EF295CD8-19D4-4E02-9438-94C926EF5284}");

            public static readonly Guid RenderingGroup = new Guid("{4B8AD536-4FA6-4122-A31E-32BB6BC42806}");

            public static readonly Guid Role = new Guid("{A7DF04B4-4C4B-44B7-BE1E-AD901BD53DAD}");

            public static readonly Guid Schedule = new Guid("{70244923-FA84-477C-8CBD-62F39642C42B}");

            public static readonly Guid StandardTemplate = new Guid("{1930BBEB-7805-471A-A3BE-4858AC7CF696}");

            public static readonly Guid Sublayout = new Guid("{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}");

            public static readonly Guid Template = new Guid("{AB86861A-6030-46C5-B394-E8F99E8B87DB}");

            public static readonly Guid TemplateField = new Guid("{455A3E98-A627-4B40-8035-E683A0331AC7}");

            public static readonly Guid TemplateFieldType = new Guid("{F8A17D6A-118E-4CD7-B5F5-88FF37A4F237}");

            public static readonly Guid TemplateFolder = new Guid("{0437FEE2-44C9-46A6-ABE9-28858D9FEE8C}");

            public static readonly Guid TemplateSection = new Guid("{E269FBB5-3750-427A-9149-7AA950B49301}");

            public static readonly Guid UnversionedImage = new Guid("{F1828A2C-7E5D-4BBD-98CA-320474871548}");

            public static readonly Guid User = new Guid("{642C9A7E-EE31-4979-86F0-39F338C10AFB}");

            public static readonly Guid VersionedImage = new Guid("{C97BA923-8009-4858-BDD5-D8BE5FCCECF7}");

            public static readonly Guid Workflow = new Guid("{1C0ACC50-37BE-4742-B43C-96A07A7410A5}");

            public static readonly Guid WorkflowCommand = new Guid("{CB01F9FC-C187-46B3-AB0B-97A8468D8303}");

            public static readonly Guid WorkflowState = new Guid("{4B7E2DA9-DE43-4C83-88C3-02F042031D04}");

            public static readonly Guid XmlLayout = new Guid("{1163DA83-B2EF-4381-BF09-B2FF714B1B3F}");

            public static readonly Guid XslRendering = new Guid("{F1F1D639-4F54-40C2-8BE0-81266B392CEB}");
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
