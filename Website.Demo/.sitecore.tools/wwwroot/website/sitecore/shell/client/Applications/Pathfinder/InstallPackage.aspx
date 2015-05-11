<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Sitecore.Pathfinder.Packages" %>
<%@ Import Namespace="Sitecore.Web" %>
<%
  var output = new StringWriter();
  Console.SetOut(output);

  var packageService = new PackageService();

  var packageId = WebUtil.GetQueryString("iu");
  if (!string.IsNullOrEmpty(packageId))
  {
    packageService.InstallOrUpdatePackage(packageId);
  }

  packageId = WebUtil.GetQueryString("i");
  if (!string.IsNullOrEmpty(packageId))
  {
    packageService.InstallPackage(packageId);
  }

  packageId = WebUtil.GetQueryString("u");
  if (!string.IsNullOrEmpty(packageId))
  {
    packageService.UpdatePackage(packageId);
  }

  packageId = WebUtil.GetQueryString("r");
  if (!string.IsNullOrEmpty(packageId))
  {
    packageService.UninstallPackage(packageId);
  }

  var response = output.ToString();
  if (!string.IsNullOrEmpty(response) || WebUtil.GetQueryString("w") == "0")
  {
    this.Response.Write(HttpUtility.HtmlEncode(response));
  }
  else
  {
    var urlReferrer = this.Request.UrlReferrer;
    if (urlReferrer == null)
    {
      return;
    }

    var redirect = urlReferrer.ToString();
    if (string.IsNullOrEmpty(redirect))
    {
      redirect = "/sitecore/shell/client/Applications/Pathfinder/InstalledPackages.aspx";
    }

    this.Response.Redirect(redirect);
  }
%>
