// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class ServeWebsite : BuildTaskBase
    {
        [ImportingConstructor]
        public ServeWebsite() : base("serve-website")
        {
            Alias = "serve";
            Shortcut = "s";
        }

        [Option("log", Alias = "l", DefaultValue = false, HelpText = "Show log in a window")]
        public bool Log { get; set; } = false;

        [NotNull, Option("path", Alias = "p", HelpText = "Path to website directory", PositionalArg = 2)]
        public string Path { get; set; } = string.Empty;

        [Option("port", Alias = "o", HelpText = "Port number", PositionalArg = 1)]
        public int Port { get; set; } = 0;

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.C1041, Texts.Serving_website___);

            var port = Port;
            if (port == 0)
            {
                port = context.Configuration.GetInt(Constants.Configuration.ServeWebsite.Port, context.Configuration.GetInt(Constants.Configuration.ServeWebsite.DefaultPort));
            }

            var path = Path;
            if (string.IsNullOrEmpty(path))
            {
                path = context.Configuration.GetWebsiteDirectory();
            }

            var fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "IIS Express\\iisexpress.exe");

            if (!context.FileSystem.FileExists(fileName))
            {
                context.Trace.TraceError(Msg.G1020, Texts.IIS_Express_not_found, fileName);
                context.Trace.TraceInformation(Msg.G1020, Texts.Please_install_IIS_Express);
                return;
            }

            var process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = $"/port:{port} /path:\"{path}\"";
            process.StartInfo.WorkingDirectory = path;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;

            if (!Log)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            process.Start();

            context.Trace.TraceInformation(Msg.C1041, $"Website running on port: {port}, path: {path}");
        }
    }
}
