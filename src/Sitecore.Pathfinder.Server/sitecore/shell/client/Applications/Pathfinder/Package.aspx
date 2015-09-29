<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Pathfinder.Packages" %>
<%@ Import Namespace="NuGet" %>
<!DOCTYPE html>
<%                                          
    var packageId = Request.QueryString["id"] ?? string.Empty;

    var packageService = new PackageService();
    var packages = packageService.FindPackagesById(packageId);
    var nuget = packages.First();

    var installedPackage = packageService.FindInstalledPackageById(packageId);

    var packageName = nuget.Package.Title ?? nuget.Package.Id;
    var iconUrl = nuget.Package.IconUrl != null ? nuget.Package.IconUrl.ToString() : "packageDefaultIcon-50x50.png";
    var published = nuget.Package.Published != null ? nuget.Package.Published.Value.ToString("d") ?? string.Empty : string.Empty;
    var projectUrl = nuget.Package.ProjectUrl != null ? nuget.Package.ProjectUrl.ToString() : string.Empty;
    var licenseUrl = nuget.Package.LicenseUrl != null ? nuget.Package.LicenseUrl.ToString() : string.Empty;
    var owners = nuget.Package.Owners;
    var authors = nuget.Package.Authors;
%>
<html class="fuelux">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <title>Sitecore Pathfinder</title>
    <link rel="stylesheet" type="text/css" href="//fonts.googleapis.com/css?family=Open+Sans" />
    <link href="/sitecore/shell/client/Speak/Assets/css/speak-default-theme.css" rel="stylesheet" type="text/css" />
    <link href="/sitecore/shell/client/Applications/Pathfinder/Packages.css" rel="stylesheet" type="text/css" />
</head>
<body class="sc sc-fullWidth">
    <div class="sc-list">
        <div class="container-narrow">
            <header class="sc-globalHeader">
                <div class="row sc-globalHeader-content">
                    <div class="col-md-6">
                        <div class="sc-globalHeader-startButton">
                            <a class="sc-global-logo medium" href="/sitecore/shell/sitecore/client/Applications/Launchpad"></a>
                        </div>
                        <div class="sc-globalHeader-navigationToggler">
                            <div class="sc-navigationPanelToggleButton">
                                <button class="btn sc-togglebutton btn-default noText" type="button">
                                    <div class="sc-icon" style="background-image: url(/sitecore/shell/client/Speak/Assets/img/Speak/NavigationPanelToggleButton/navigationPanelToggleIcon.png); background-position: 50% 50%;">
                                    </div>
                                    <span class="sc-togglebutton-text"></span>
                                </button>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="sc-globalHeader-loginInfo">
                            <ul class="sc-accountInformation">
                                <li>
                                    <a class="logout" href="/api/sitecore/Authentication/Logout?sc_database=master">Logout</a>
                                </li>
                                <li>
                                    <%= Sitecore.Context.User.Profile.FullName %>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </header>

            <section class="sc-applicationContent">
                <div class="sc-navigation-wrapper">
                    <nav class="sc-applicationContent-navigation sc-navigation-menu">

                        <div class="sc-menu">
                            <div class="menuroot">
                                <div class="header menuItem open">
                                    <a href="#">
                                        <img class="menuicon" src="/~/icon/OfficeWhite/24x24/checkbox_selected.png" alt="Navigation"><span class="toplevel">Pathfinder</span>
                                    </a>
                                    <img class="menuchevron">
                                </div>
                                <div class="toplevelcontainer itemsContainer" style="display: block;">
                                    <div>
                                        <div class="itemRow menuItem depth2">
                                            <div class="leftcolumn">&nbsp;</div>
                                            <div class="rightcolumn">
                                                <a href="/sitecore/shell/client/Applications/Pathfinder/Packages.aspx" class="sc-hyperlinkbutton">Packages</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </nav>
                </div>
            </section>
        </div>
    </div>
    <div class="sc-navigation-content">
        <header class="sc-applicationHeader">
            <div class="sc-applicationHeader-row1">
                <div class="sc-applicationHeader-content">
                    <div class="sc-applicationHeader-title">
                        <span class="sc-text"><% =packageName  %></span>
                    </div>
                </div>

                <div class="sc-applicationHeader-content breadcrumb">
                    <div class="sc-applicationHeader-breadCrumb">
                        <div class="sc-breadcrumb">
                            <ul>
                                <li>
                                    <a href="/sitecore/shell/client/Applications/Pathfinder/Packages.aspx">Pathfinder</a>
                                </li>
                                <li>
                                    <a href="/sitecore/shell/client/Applications/Pathfinder/Package.aspx"><% =packageName %></a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="sc-applicationHeader-row2">
                <div class="sc-applicationHeader-back">
                    <p style="font-size: 24px"></p>
                </div>
                <div class="sc-applicationHeader-contextSwitcher">
                </div>
                <div class="sc-applicationHeader-actions">
                </div>
            </div>
        </header>

        <section class="sc-applicationContent-main">
            <div class="row">
                <div class="col-md-2">
                    <p>
                        <img src="<% =iconUrl %>" width="50" height="50" />
                    </p>   
                    <% if (!string.IsNullOrEmpty(published))
                       { %>
                    <h2><% =published %></h2>
                    <p>
                        Last published
                    </p>
                    <% } %>

                    <% if (!string.IsNullOrEmpty(projectUrl))
                       { %>
                    <p>
                        <a href="<% =projectUrl %>">Project Site</a>
                    </p>
                    <% } %>

                    <% if (!string.IsNullOrEmpty(licenseUrl))
                       { %>
                    <p>
                        <a href="<% =licenseUrl %>">License</a>
                    </p>
                    <% } %>
                    
                </div>
                <div class="col-md-10">
                    <h1><% =packageName %></h1>
                    <hr/>
                    <p>
                        <% =nuget.Package.Description %>
                    </p>
                    
                    <h3>Owners</h3>  
                    <% foreach (var owner in owners)
                       { %>
                    <p><% = owner %></p>
                    <% } %>

                    <h3>Authors</h3>  
                    <% foreach (var author in authors)
                       { %>
                    <p><% = author %></p>
                    <% } %>
                                
                    <% if (!string.IsNullOrEmpty(nuget.Package.Copyright))
                       { %>
                    <h3>Copyright</h3>  
                    <% = nuget.Package.Copyright %>                          
                    <% } %>
                        
                    <% if (nuget.Package.DependencySets.Any(s => s.TargetFramework != null))
                       { %>
                    <h3>Dependencies</h3>  
                    <% foreach (var set in nuget.Package.DependencySets)
                       {
                           if (set.TargetFramework == null)
                           {
                               continue;
                           }

                           if (!set.Dependencies.Any())
                           {
                               continue;
                           }

                    %>   
                    <p>
                        <u><% = set.TargetFramework.Identifier %> version <% = set.TargetFramework.Version.ToString() %></u>
                    </p>   
                      
                    <%
                        foreach (var dependency in set.Dependencies)
                        {
                            var packageUrl = "/sitecore/shell/client/Applications/Pathfinder/Package.aspx?id=" + dependency.Id;
                    %>
                     <p>
                         <a href="<% = packageUrl %>"><% = dependency.Id %></a>
                         <span><% = VersionUtility.PrettyPrint(dependency.VersionSpec) %></span>
                     </p>
                     <%
                        }
                        }
                        }
                     %>

                    <h3>Version History</h3>  
                    <table class="table">
                        <tr>
                            <th>Version</th>
                            <th>Last update</th>
                            <th>Action</th>
                        </tr>
                    <% foreach (var package in packages.OrderByDescending(p => p.Package.Version))
                       {
                           var isInstalled = installedPackage != null && package.Version == installedPackage.Version;
                            var installHref = "/sitecore/shell/client/Applications/Pathfinder/InstallPackage.aspx?ins=" + System.Web.HttpUtility.UrlEncode(package.PackageId) + "&v=" + System.Web.HttpUtility.UrlEncode(package.Version.ToString());
                            var updateHref = "/sitecore/shell/client/Applications/Pathfinder/InstallPackage.aspx?upd=" + System.Web.HttpUtility.UrlEncode(package.PackageId) + "&v=" + System.Web.HttpUtility.UrlEncode(package.Version.ToString());
                            var uninstallHref = "/sitecore/shell/client/Applications/Pathfinder/InstallPackage.aspx?rem=" + System.Web.HttpUtility.UrlEncode(package.PackageId);

                            %>
                       <tr>
                           <td>       
                               <% =package.Package.Version.ToString() %>
                           </td>
                           <td>       
                               <% =package.Package.Published != null ? package.Package.Published.Value.ToString("d") ?? string.Empty : string.Empty %>
                           </td>
                           <td>
                            <% if (isInstalled)
                                { %> 
                                <span>Installed</span>
                            <% } %>

                            <% if (isInstalled)
                                { %> 
                                | <a href="<% =uninstallHref %>">Uninstall</a>
                            <% } %>

                            <% if (!isInstalled && installedPackage != null)
                                { %> 
                                <a href="<% =updateHref %>">Install</a>
                            <% } %>

                            <% if (!isInstalled && installedPackage == null)
                                { %>
                                 <a href="<% =installHref %>">Install</a>
                            <% } %>
                               
                           </td>
                       </tr>
                    <% } %>
                   </table>
                </div>
            </div>
            
        </section>
    </div>
</body>
</html>
