// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Pathfinder.Diagnostics;
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

        [NotNull]
        [ItemNotNull]
        public static readonly IList<ITextNode> EmptyReadOnlyTextNodeCollection = new ReadOnlyCollection<ITextNode>(new List<ITextNode>());

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
            public const string BuildProject = "build-project:tasks";

            public const string CheckBinFileVersion = "install-package:check-bin-file-version";

            public const string CodeGen = "codegen";

            public const string CommandLineConfig = "config";

            public const string ContentFiles = "build-project:content-files";

            public const string Database = "database";

            public const string DataDirectoryName = "copy-package:data-directory-name";

            public const string Debug = "system:debug";

            public const string ExternalDirectory = "build-project:external-directory";

            public const string Files = "build-project:files";

            public const string HostName = "host-name";

            public const string IgnoreDirectories = "build-project:ignore-directories";

            public const string IgnoreFileNames = "build-project:ignore-filenames";

            public const string InstallUrl = "install-package:install-url";

            public const string LocalTestDirectory = "run-unittests:local-test-directory";

            public const string PackageDirectory = "copy-package:package-directory";

            public const string PackagesDirectory = "copy-dependencies:packages-directory";

            public const string PackNugetDirectory = "pack-nuget:directory";

            public const string PackNugetExclude = "pack-nuget:exclude";

            public const string PackNugetInclude = "pack-nuget:include";

            public const string Password = "password";

            public const string Pathfinder = "Pathfinder";

            public const string ProjectConfigFileName = "project";

            public const string ProjectUniqueId = "project-unique-id";

            public const string PublishUrl = "publish-databases:publish-url";

            public const string RemapFileDirectories = "remap-file-directories";

            public const string ProjectDirectory = "project-directory";

            public const string StandardTemplateFields = "standard-template-fields";

            public const string SystemConfigFileName = "system:config";

            public const string ToolsDirectory = "system:toolspath";

            public const string UninstallDirectory = "install-package:uninstall-directory";

            public const string UpdateResourcesUrl = "sync-website:sync-url";

            public const string UserName = "user-name";

            public const string WebsiteDirectoryName = "run-unittests:website-directory-name";

            public const string WebTestRunnerName = "run-unittests:web-test-runner-name";

            public const string WebTestRunnerUrl = "run-unittests:web-test-runner-url";

            public const string Wwwroot = "wwwroot";

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

            public const string IsExtern = "IsExternalReference";
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
