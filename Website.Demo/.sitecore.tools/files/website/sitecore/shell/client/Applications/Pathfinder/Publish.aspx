<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<%
    var output = new StringWriter();
    Console.SetOut(output);

    var databaseName = WebUtil.GetQueryString("db", "master");
    var mode = WebUtil.GetQueryString("m", "i");

    var database = Factory.GetDatabase(databaseName);

    var publishingTargets = PublishManager.GetPublishingTargets(database);

    var targetDatabases = publishingTargets.Select(target => Factory.GetDatabase(target["Target database"])).ToArray();
    if (!targetDatabases.Any())
    {
        return;
    }

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
          
 