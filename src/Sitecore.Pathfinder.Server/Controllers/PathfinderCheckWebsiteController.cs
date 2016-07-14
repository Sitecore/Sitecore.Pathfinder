// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Jobs;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderCheckWebsiteController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            try
            {
                var output = new StringWriter();
                Console.SetOut(output);

                var host = new Startup().WithToolsDirectory(FileUtil.MapPath("/bin")).WithBinDirectory(FileUtil.MapPath("/bin")).WithProjectDirectory(FileUtil.MapPath("/")).WithWebsiteDirectory(FileUtil.MapPath("/")).WithDataFolderDirectory(FileUtil.MapPath(Settings.DataFolder)).DoNotLoadConfigFiles().Start();
                if (host == null)
                {
                    return new HttpNotFoundResult();
                }

                if (!string.IsNullOrEmpty(WebUtil.GetFormValue("StartButton")))
                {
                    return Start();
                }

                ViewBag.State = "Unchecked";

                var checkerService = host.CompositionService.Resolve<ICheckerService>();
                ViewBag.CheckerNames = checkerService.Checkers.Select(c => new Tuple<string, string>(c.Method.Name, c.Method.DeclaringType?.Name ?? string.Empty));

                var enabledCheckersFileName = FileUtil.MapPath(Path.Combine(TempFolder.Folder, "Pathfinder.CheckWebsite.Checkers.txt"));
                var enabledCheckerNames = System.IO.File.Exists(enabledCheckersFileName) ? System.IO.File.ReadAllText(enabledCheckersFileName).Split(',') : Enumerable.Empty<string>();
                ViewBag.EnabledCheckerNames = enabledCheckerNames;

                var jobHandle = WebUtil.GetQueryString("job");
                if (!string.IsNullOrEmpty(jobHandle))
                {
                    var job = JobManager.GetJob(Handle.Parse(jobHandle));
                    if (job != null)
                    {
                        if (job.IsDone)
                        {
                            ViewBag.State = "Checked";
                            ViewBag.Diagnostics = LoadDiagnostics(job.Handle.ToString());
                        }
                        else
                        {
                            ViewBag.State = "Checking";
                        }
                    }
                }

                return View("~/sitecore/shell/client/Applications/Pathfinder/CheckWebsite.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred", ex, GetType());
                throw;
            }
        }

        [NotNull, ItemNotNull]
        private IEnumerable<Diagnostic> LoadDiagnostics([NotNull] string jobHandle)
        {
            var xmlFileName = FileUtil.MapPath(Path.Combine(TempFolder.Folder, "Pathfinder.CheckWebsite." + jobHandle + ".xml"));
            if (!System.IO.File.Exists(xmlFileName))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            XDocument doc;
            try
            {
                doc = XDocument.Load(xmlFileName);
            }
            catch
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var root = doc.Root;
            if (root == null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var diagnostics = new List<Diagnostic>();

            foreach (var element in root.Elements())
            {
                int msg;
                if (!int.TryParse(element.GetAttributeValue("msg"), out msg))
                {
                    msg = 0;
                }

                var fileName = element.GetAttributeValue("filename");

                TextSpan span;
                if (!TextSpan.TryParse(element.GetAttributeValue("span"), out span))
                {
                    span = TextSpan.Empty;
                }

                Severity severity;
                if (!Enum.TryParse(element.GetAttributeValue("severity"), out severity))
                {
                    severity = Severity.Information;
                }

                var text = element.Value;

                var diagnostic = new Diagnostic(msg, fileName, span, severity, text);
                diagnostics.Add(diagnostic);
            }

            return diagnostics;
        }

        private void RunCheckWebsite([NotNull, ItemNotNull] IEnumerable<string> checkerNames)
        {
            var host = new Startup().WithToolsDirectory(FileUtil.MapPath("/bin")).WithBinDirectory(FileUtil.MapPath("/bin")).WithProjectDirectory(FileUtil.MapPath("/")).WithWebsiteDirectory(FileUtil.MapPath("/")).WithDataFolderDirectory(FileUtil.MapPath(Settings.DataFolder)).DoNotLoadConfigFiles().Start();
            if (host == null)
            {
                return;
            }

            var checkerService = host.CompositionService.Resolve<ICheckerService>();

            var options = new ProjectOptions(host.Configuration.GetProjectDirectory(), "master");

            var project = host.CompositionService.Resolve<WebsiteProject>().With(Factory.GetDatabase("master"), options, host.Configuration.GetProjectDirectory(), "WebsiteChecker");

            try
            {
                checkerService.CheckProject(project, project, checkerNames);
            }
            catch (Exception ex)
            {
                ((IDiagnosticCollector)project).Add(new Diagnostic(Msg.G1000, string.Empty, TextSpan.Empty, Severity.Error, ex.Message));
            }

            var fileName = FileUtil.MapPath(Path.Combine(TempFolder.Folder, "Pathfinder.CheckWebsite." + Context.Job.Handle + ".xml"));

            using (var output = new XmlTextWriter(fileName, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            })
            {
                output.WriteStartElement("diagnostics");

                foreach (var diagnostic in project.Diagnostics)
                {
                    output.WriteStartElement("diagnostic");

                    output.WriteAttributeString("msg", diagnostic.Msg.ToString());
                    output.WriteAttributeString("severity", diagnostic.Severity.ToString());

                    if (!string.IsNullOrEmpty(diagnostic.FileName))
                    {
                        output.WriteAttributeString("filename", diagnostic.FileName);
                    }

                    if (diagnostic.Span != TextSpan.Empty)
                    {
                        output.WriteAttributeString("span", diagnostic.Span.ToString());
                    }

                    output.WriteValue(diagnostic.Text);

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }
        }

        [NotNull]
        private ActionResult Start()
        {
            var checkerNames = new List<string>();

            foreach (var key in Request.Form.AllKeys)
            {
                if (key != null && key.StartsWith("checker_"))
                {
                    checkerNames.Add(key.Mid(8));
                }
            }


            System.IO.File.WriteAllText(FileUtil.MapPath(Path.Combine(TempFolder.Folder, "Pathfinder.CheckWebsite.Checkers.txt")), string.Join(",", checkerNames));

            var handle = BackgroundJob.Run("Pathfinder Check Website", "Check Website", () => RunCheckWebsite(checkerNames));

            return new RedirectResult("/pathfinder/check-website?job=" + handle);
        }
    }
}
