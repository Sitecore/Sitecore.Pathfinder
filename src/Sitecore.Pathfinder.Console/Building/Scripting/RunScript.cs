// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Scripting
{
    public class RunScript : BuildTaskBase
    {
        [ImportingConstructor]
        public RunScript([NotNull] IConsoleService console) : base("run-script")
        {
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;

            var script = context.Script;
            if (string.IsNullOrEmpty(script))
            {
                Console.WriteLine("No script specified");
                return;
            }

            var scriptFileName = Path.Combine(context.ProjectDirectory, "sitecore.project\\scripts\\" + script);
            if (!context.FileSystem.FileExists(scriptFileName))
            {
                scriptFileName = Path.Combine(context.ToolsDirectory, "files\\scripts\\" + script);
            }

            if (!context.FileSystem.FileExists(scriptFileName))
            {
                Console.WriteLine("Script not found");
                return;
            }

            Console.WriteLine("Running script: " + scriptFileName);

            if (scriptFileName.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                ExecutePowerShellScript(context, scriptFileName);
            }
            else if (scriptFileName.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) || scriptFileName.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
            {
                ExecuteCmdScript(context, scriptFileName);
            }
            else
            {
                Console.WriteLine("Sorry, I do not know how to run " + scriptFileName);
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Runs a PowerShell, .cmd or .bat script.");
        }

        protected virtual void ExecuteCmdScript([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            var process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
            {
                Console.WriteLine(args.Data);
            };

            process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
            {
                Console.WriteLine(args.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        protected virtual void ExecutePowerShellScript([NotNull] IBuildContext context, [NotNull] string scriptFileName)
        {
            var runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            runspace.SessionStateProxy.Path.SetLocation(context.ProjectDirectory);

            runspace.SessionStateProxy.SetVariable("buildContext", context);
            runspace.SessionStateProxy.SetVariable("toolsDirectory", context.ToolsDirectory);
            runspace.SessionStateProxy.SetVariable("projectDirectory", context.ProjectDirectory);
            runspace.SessionStateProxy.SetVariable("dataFolderDirectory", context.Configuration.GetString(Constants.Configuration.DataFolderDirectory));
            runspace.SessionStateProxy.SetVariable("websiteDirectory", context.Configuration.GetString(Constants.Configuration.WebsiteDirectory));

            try
            {
                var pipeline = runspace.CreatePipeline();
                pipeline.Output.DataReady += WriteOutput;
                pipeline.Error.DataReady += WriteError;

                var script = context.FileSystem.ReadAllText(scriptFileName);
                pipeline.Commands.AddScript(script);

                pipeline.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected virtual void WriteError([NotNull] object sender, [NotNull] EventArgs e)
        {
            var error = sender as PipelineReader<object>;
            Assert.Cast(error, nameof(error));

            while (error.Count > 0)
            {
                Console.WriteLine(error.Read().ToString());
            }
        }

        protected virtual void WriteOutput([NotNull] object sender, [NotNull] EventArgs e)
        {
            var output = sender as PipelineReader<object>;
            Assert.Cast(output, nameof(output));

            while (output.Count > 0)
            {
                Console.WriteLine(output.Read().ToString());
            }
        }
    }
}
