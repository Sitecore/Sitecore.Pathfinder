<%@ WebHandler Language="C#" Class="SitecorePathfinderWebTestRunnerHandler" %>

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public class SitecorePathfinderWebTestRunnerHandler : IHttpHandler
{
    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
       context.Response.ContentType = "text/plain";
       var output = new StringWriter();
       Console.SetOut(output);
       var testName = context.Request.QueryString["test"] ?? string.Empty;

       var compiler = new Sitecore.CodeDom.Compiler.CSharpCompiler();
       var references = new StringCollection();
       references.Add(Sitecore.IO.FileUtil.MapPath("/bin/nunit.framework.dll"));
       references.Add("System.Configuration.dll");

       var assembly = compiler.FileToMemory("/sitecore/shell/client/Applications/Pathfinder/Tests/PathfinderTests.cs", references);
       var type = assembly.GetType("Sitecore.Pathfinder.Tests.PathfinderTests");
       var instance = Activator.CreateInstance(type);

       var methodInfo = type.GetMethod(testName);
       methodInfo.Invoke(instance, null);

       Console.WriteLine("Passed: " + testName);
       context.Response.Write(HttpUtility.HtmlEncode(output.ToString()));
    }
}
