<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Sitecore.Configuration" %>
<%@ Import Namespace="Sitecore.Data.Managers" %>
<%@ Import Namespace="Sitecore.Publishing" %>
<%@ Import Namespace="Sitecore.Web" %>
<%
  var output = new StringWriter();
  Console.SetOut(output);

  var databaseName = WebUtil.GetQueryString("db", "master");
  var mode = WebUtil.GetQueryString("m", "i");

  var database = Factory.GetDatabase(databaseName);

  var publishingTargets = PublishManager.GetPublishingTargets(database);

  var targetDatabases = publishingTargets.Select(target => Factory.GetDatabase(target["Target database"])).ToArray();

  var languages = LanguageManager.GetLanguages(database).ToArray();

  switch (mode)
  {
    case "r":
      PublishManager.Republish(database, targetDatabases, languages);
      break;

    case "i":
      PublishManager.PublishIncremental(database, targetDatabases, languages);
      break;

    case "s":
      PublishManager.PublishSmart(database, targetDatabases, languages);
      break;

    case "b":
      PublishManager.RebuildDatabase(database, targetDatabases);
      break;
  }
%>
