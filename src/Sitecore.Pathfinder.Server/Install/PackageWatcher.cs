// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sitecore.Diagnostics;
using Sitecore.IO;

namespace Sitecore.Pathfinder.Install
{
    public class PackageWatcher
    {
        private static readonly List<string> Queue = new List<string>();

        private static readonly object SyncRoot = new object();

        public PackageWatcher()
        {
            SourceDirectory = FileUtil.MapDataFilePath("pathfinder");
            Directory.CreateDirectory(SourceDirectory);

            InstallPackages();

            Watcher = new FileSystemWatcher(SourceDirectory);
            Watcher.IncludeSubdirectories = true;
            Watcher.Created += OnFileCreated;
            Watcher.Changed += OnFileCreated;
            Watcher.Renamed += OnFileCreated;
            Watcher.EnableRaisingEvents = true;

            var thread = new Thread(WorkerLoop);
            thread.Start();
        }

        public static PackageWatcher Instance { get; set; }

        private bool IsInstalling { get; set; }

        [NotNull]
        private string SourceDirectory { get; }

        [NotNull]
        private static FileSystemWatcher Watcher { get; set; }

        public static void Initialize()
        {
            Instance = new PackageWatcher();
        }

        private void InstallPackage([NotNull] string fileName)
        {
            try
            {
                var extension = Path.GetExtension(fileName);

                // todo: make extendable
                switch (extension.ToLowerInvariant())
                {
                    case ".zip":
                        new SitecorePackageInstaller().Install(fileName);
                        break;
                    case ".nupkg":
                        new NugetPackageInstaller().Install(fileName);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to install package", ex, GetType());
            }
        }

        private void InstallPackages()
        {
            IsInstalling = true;

            try
            {
                var changedFiles = ScanForChanges();
                foreach (var changedFile in changedFiles)
                {
                    InstallPackage(changedFile);
                }
            }
            finally
            {
                IsInstalling = false;
            }
        }

        private bool IsInstallableFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            // todo: make extendable
            return string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".nupkg", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsPackageChanged(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            var logFileName = fileName + ".log";
            if (!File.Exists(logFileName))
            {
                return true;
            }

            var fileInfo = new FileInfo(fileName);
            var logFileInfo = new FileInfo(logFileName);

            return fileInfo.LastWriteTimeUtc > logFileInfo.LastWriteTimeUtc;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs args)
        {
            lock (SyncRoot)
            {
                if (!Queue.Contains(args.FullPath))
                {
                    Queue.Add(args.FullPath);
                }
            }
        }

        private List<string> ScanForChanges()
        {
            var changedFiles = new List<string>();
            var fileNames = Directory.GetFiles(SourceDirectory);

            foreach (var fileName in fileNames)
            {
                var extension = Path.GetExtension(fileName);
                if (string.Equals(extension, ".log", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (IsInstallableFile(fileName) && IsPackageChanged(fileName))
                {
                    changedFiles.Add(fileName);
                }
            }

            return changedFiles;
        }

        // ReSharper disable once FunctionNeverReturns
        private void WorkerLoop()
        {
            while (true)
            {
                IEnumerable<string> fileNames = null;

                lock (SyncRoot)
                {
                    if (Queue.Count > 0)
                    {
                        fileNames = new List<string>(Queue);
                        Queue.Clear();
                    }
                }

                if (fileNames != null)
                {
                    foreach (var fileName in fileNames)
                    {
                       InstallPackage(fileName);
                    }
                }

                Thread.Sleep(200);
            }
        }
    }
}
