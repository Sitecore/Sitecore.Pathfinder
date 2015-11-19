// � 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Scripts
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
            var proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();

            proc.WaitForExit();

            var output = proc.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }

            var error = proc.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
            }
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
            if (error == null)
            {
                return;
            }

            while (error.Count > 0)
            {
                Console.WriteLine(error.Read().ToString());
            }
        }

        protected virtual void WriteOutput([NotNull] object sender, [NotNull] EventArgs e)
        {
            var output = sender as PipelineReader<PSObject>;
            if (output == null)
            {
                return;
            }

            while (output.Count > 0)
            {
                Console.WriteLine(output.Read().ToString());
            }
        }
    }
}
