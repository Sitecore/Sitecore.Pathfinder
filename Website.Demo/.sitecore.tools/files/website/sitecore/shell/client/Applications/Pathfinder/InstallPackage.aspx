<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Sitecore.Pathfinder.Packages" %>
<%@ Import Namespace="Sitecore.Web" %>
<%
    var output = new StringWriter();
    Console.SetOut(output);

    var packageService = new PackageService();

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
        packageService.InstallPackage(packageId);
    }

    // update
    packageId = WebUtil.GetQueryString("upd");
    if (!string.IsNullOrEmpty(packageId))
    {
        packageService.UpdatePackage(packageId);
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
     
