// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
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

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.C1041, Texts.Serving_website___);

            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "IIS Express\\iisexpress.exe");
            var port = context.Configuration.GetInt(Constants.Configuration.ServeWebsite.Port, context.Configuration.GetInt(Constants.Configuration.ServeWebsite.DefaultPort));
            var path = context.Configuration.GetWebsiteDirectory();

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
