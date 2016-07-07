// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class RunScript : BuildTaskBase, IScriptTask
    {
        [CanBeNull]
        private string _script;

        [ImportingConstructor]
        public RunScript([NotNull] IFileSystemService fileSystem, [NotNull] IConsoleService console) : base("run-script")
        {
            FileSystem = fileSystem;
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            if (string.IsNullOrEmpty(_script))
            {
                Console.WriteLine(Texts.No_script_specified);
                return;
            }

            var scriptFileName = Path.Combine(context.ProjectDirectory, "sitecore.project\\scripts\\" + _script);
            if (!FileSystem.FileExists(scriptFileName))
            {
                scriptFileName = Path.Combine(context.ToolsDirectory, "files\\scripts\\" + _script);
            }

            if (!FileSystem.FileExists(scriptFileName))
            {
                Console.WriteLine(Texts.Script_not_found);
                return;
            }

            Console.WriteLine(Texts.Running_script_ + @" " + scriptFileName);

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
                Console.WriteLine(Texts.Sorry__I_do_not_know_how_to_run_ + scriptFileName);
            }
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

            process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args) { Console.WriteLine(args.Data); };

            process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args) { Console.WriteLine(args.Data); };

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
            runspace.SessionStateProxy.SetVariable("websiteDirectory", context.Configuration.GetWebsiteDirectory());

            try
            {
                var pipeline = runspace.CreatePipeline();
                pipeline.Output.DataReady += WriteOutput;
                pipeline.Error.DataReady += WriteError;

                var script = FileSystem.ReadAllText(scriptFileName);
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

        ITask IScriptTask.With(string script)
        {
            _script = script;
            return this;
        }
    }
}
