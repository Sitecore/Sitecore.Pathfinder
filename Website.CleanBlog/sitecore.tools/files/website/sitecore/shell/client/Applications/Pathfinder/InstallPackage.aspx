<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Sitecore.Pathfinder.Packages" %>
<%@ Import Namespace="Sitecore.Web" %>
<%
    var output = new StringWriter();
    Console.SetOut(output);

    var packageService = new PackageService();

    var versionString = WebUtil.GetQueryString("v", string.Empty);
    NuGet.SemanticVersion version;
    if (!NuGet.SemanticVersion.TryParse(versionString, out version))
    {
        version = null;
    }

    // replace
    var packageId = WebUtil.GetQueryString("rep");
    if (!string.IsNullOrEmpty(packageId))
    {
        packageService.InstallOrUpdatePackage(packageId);
    }

    // install
    packageId = WebUtil.GetQueryString("ins");
    if (!string.IsNullOrEmpty(packageId))
    {
        packageService.InstallPackage(packageId, version);
    }

    // update
    packageId = WebUtil.GetQueryString("upd");
    if (!string.IsNullOrEmpty(packageId))
    {
        packageService.UpdatePackage(packageId, version);
    }

    // remove
    packageId = WebUtil.GetQueryString("rem");
    if (!string.IsNullOrEmpty(packageId))
    {
        packageService.UninstallPackage(packageId);
    }

    var response = output.ToString();
    if (!string.IsNullOrEmpty(response) || WebUtil.GetQueryString("w") == "0")
    {
        Response.Write(HttpUtility.HtmlEncode(response));
    }
    else
    {
        var urlReferrer = Request.UrlReferrer;
        if (urlReferrer == null)
        {
            return;
        }

        var redirect = urlReferrer.ToString();
        if (string.IsNullOrEmpty(redirect))
        {
            redirect = "/sitecore/shell/client/Applications/Pathfinder/InstalledPackages.aspx";
        }

        Response.Redirect(redirect);
    }
%>
     
