﻿You must install React.NET manually on the website.

Copy these assemblies to /bin:

* ClearScript.dll
* JavaScriptEngineSwitcher.Core.dll
* JavaScriptEngineSwitcher.Msie.dll
* JavaScriptEngineSwitcher.V8.dll
* JSPool.dll
* MsieJavaScriptEngine.dll
* React.Core.dll
* React.Web.dll
* React.Web.Mvc4.dll
* VroomJs.dll
* WebActivatorEx.dll

Replace or update the web.config :

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <sectionGroup name="jsEngineSwitcher">
      <section name="core" type="JavaScriptEngineSwitcher.Core.Configuration.CoreConfiguration, JavaScriptEngineSwitcher.Core" />
      <section name="v8" type="JavaScriptEngineSwitcher.V8.Configuration.V8Configuration, JavaScriptEngineSwitcher.V8" />
      <section name="msie" type="JavaScriptEngineSwitcher.Msie.Configuration.MsieConfiguration, JavaScriptEngineSwitcher.Msie" />
    </sectionGroup>
  <section name="sitecore" type="Sitecore.Configuration.ConfigReader, Sitecore.Kernel" />
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, Sitecore.Logging" />
  </configSections>
  <connectionStrings configSource="App_Config\ConnectionStrings.config" />
  <appSettings>
    <add key="EmailReminder.FromAddress" value="name@server.net" />
    <!-- NetSpell directory -->
    <add key="DictionaryFolder" value="/sitecore/shell/controls/rich text editor/Dictionaries/" />
    <add key="Lucene.Net.FSDirectory.class" value="Sitecore.Search.FSDirectory, Sitecore.Kernel" />
    <add key="PageInspector:ServerCodeMappingSupport" value="Disabled" />
  </appSettings>
  <sitecore configSource="App_Config\Sitecore.config" />
  <!-- SYSTEM.WEBSERVER
       This section is a ASP.NET configuration section when running in Integrated Mode on IIS7.
  -->
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true">
      <remove name="WebDAVModule" />
      <add type="Sitecore.Web.RewriteModule, Sitecore.Kernel" name="SitecoreRewriteModule" />
      <add type="Sitecore.Nexus.Web.HttpModule,Sitecore.Nexus" name="SitecoreHttpModule" />
      <add type="Sitecore.Resources.Media.UploadWatcher, Sitecore.Kernel" name="SitecoreUploadWatcher" />
      <add type="Sitecore.IO.XslWatcher, Sitecore.Kernel" name="SitecoreXslWatcher" />
      <add type="Sitecore.IO.LayoutWatcher, Sitecore.Kernel" name="SitecoreLayoutWatcher" />
      <add type="Sitecore.Configuration.ConfigWatcher, Sitecore.Kernel" name="SitecoreConfigWatcher" />
      <remove name="Session" />
      <add name="Session" type="System.Web.SessionState.SessionStateModule" preCondition="" />
      <add type="Sitecore.Analytics.RobotDetection.Media.MediaRequestSessionModule, Sitecore.Analytics.RobotDetection" name="MediaRequestSessionModule" />
      <add type="Sitecore.Web.HttpModule,Sitecore.Kernel" name="SitecoreHttpModuleExtensions" />
      <add name="SitecoreAntiCSRF" type="Sitecore.Security.AntiCsrf.SitecoreAntiCsrfModule, Sitecore.Security.AntiCsrf" />
    </modules>
    <handlers>
      <remove name="Babel" />
      <add name="Babel" verb="GET" path="*.jsx" type="React.Web.BabelHandlerFactory, React.Web" preCondition="integratedMode" />
      <add name="WebDAVRoot" path="*" verb="OPTIONS,PROPFIND" modules="IsapiModule" scriptProcessor="%windir%\Microsoft.NET\Framework\v4.0.30319\aspnet_isapi.dll" resourceType="Unspecified" preCondition="classicMode,runtimeVersionv4.0,bitness32" />
      <add name="WebDAVRoot64" path="*" verb="OPTIONS,PROPFIND" modules="IsapiModule" scriptProcessor="%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll" resourceType="Unspecified" preCondition="classicMode,runtimeVersionv4.0,bitness64" />
      <add verb="*" path="sitecore_webDAV.ashx" type="Sitecore.Resources.Media.WebDAVMediaRequestHandler, Sitecore.Kernel" name="Sitecore.WebDAVMediaRequestHandler" />
      <add verb="*" path="sitecore_media.ashx" type="Sitecore.Resources.Media.MediaRequestHandler, Sitecore.Kernel" name="Sitecore.MediaRequestHandler" />
      <add verb="*" path="sitecore_xaml.ashx" type="Sitecore.Web.UI.XamlSharp.Xaml.XamlPageHandlerFactory, Sitecore.Kernel" name="Sitecore.XamlPageRequestHandler" />
      <add verb="*" path="sitecore_icon.ashx" type="Sitecore.Resources.IconRequestHandler, Sitecore.Kernel" name="Sitecore.IconRequestHandler" />
      <add verb="*" path="sitecore_feed.ashx" type="Sitecore.Shell.Feeds.FeedRequestHandler, Sitecore.Kernel" name="Sitecore.FeedRequestHandler" />
      <add verb="*" path="sitecore_handlers.ashx" type="Sitecore.Web.CustomHandlerFactory, Sitecore.Kernel" name="Sitecore.GenericHandler" />
      <add verb="*" path="sitecore_device_simulation.ashx" type="Sitecore.Shell.DeviceSimulation.SimulationRequestHandler, Sitecore.Kernel" name="Sitecore.SimulationRequestHandler" />
      <add name="Telerik_Web_UI_DialogHandler_aspx" verb="*" preCondition="integratedMode" path="Telerik.Web.UI.DialogHandler.aspx" type="Telerik.Web.UI.DialogHandler" />
      <add name="Telerik_Web_UI_SpellCheckHandler_axd" verb="*" preCondition="integratedMode" path="Telerik.Web.UI.SpellCheckHandler.axd" type="Telerik.Web.UI.SpellCheckHandler" />
      <add name="Telerik_Web_UI_WebResource_axd" verb="*" preCondition="integratedMode" path="Telerik.Web.UI.WebResource.axd" type="Telerik.Web.UI.WebResource" />
      <add name="LoggerHandler" verb="*" path="*.logger" type="JSNLog.LoggerHandler, Sitecore.Logging.Client" resourceType="Unspecified" preCondition="integratedMode" />
      <add name="LoggerHandler-Classic" path="*.logger" verb="*" modules="IsapiModule" scriptProcessor="%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll" resourceType="Unspecified" preCondition="classicMode" />
      <add verb="*" path="sitecore_expeditor_speak_request.ashx" type="Sitecore.ExperienceEditor.Speak.Server.RequestHandler, Sitecore.ExperienceEditor.Speak" name="Sitecore.ExperienceEditor.Speak" />
      <add verb="*" name="Sitecore.SpeakJS64" path="*/speak/v1/*/*.js" modules="IsapiModule" scriptProcessor="%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll" resourceType="Unspecified" preCondition="classicMode,runtimeVersionv4.0,bitness64" />
      <add verb="*" name="Sitecore.SpeakJS32" path="*/speak/v1/*/*.js" modules="IsapiModule" scriptProcessor="%windir%\Microsoft.NET\Framework\v4.0.30319\aspnet_isapi.dll" resourceType="Unspecified" preCondition="classicMode,runtimeVersionv4.0,bitness32" />
      <add verb="*" name="Sitecore.SpeakClassic64" path="sitecore_speak.ashx" modules="IsapiModule" scriptProcessor="%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_isapi.dll" resourceType="Unspecified" preCondition="classicMode,runtimeVersionv4.0,bitness64" />
      <add verb="*" name="Sitecore.SpeakClassic32" path="sitecore_speak.ashx" modules="IsapiModule" scriptProcessor="%windir%\Microsoft.NET\Framework\v4.0.30319\aspnet_isapi.dll" resourceType="Unspecified" preCondition="classicMode,runtimeVersionv4.0,bitness32" />
      <add verb="*" path="sitecore_speak.ashx" type="Sitecore.Resources.Scripts.ScriptHandler, Sitecore.Speak.Client" name="Sitecore.Speak" />
    </handlers>
    <validation validateIntegratedModeConfiguration="false" />
    <security>
      <requestFiltering>
        <requestLimits maxAllowedContentLength="524288000" />
        <hiddenSegments>
          <add segment="ClearScript.V8" />
        </hiddenSegments>
      </requestFiltering>
    </security>
  </system.webServer>
  <system.web>
    <!-- Continue to run Sitecore without script validations -->
    <pages validateRequest="false">
      <controls>
        <add tagPrefix="sc" namespace="Sitecore.Web.UI.WebControls" assembly="Sitecore.Kernel" />
        <add tagPrefix="asp" namespace="System.Web.UI" assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
        <add tagPrefix="asp" namespace="System.Web.UI.WebControls" assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
        <add tagPrefix="sc" namespace="Sitecore.Web.UI.WebControls" assembly="Sitecore.Analytics" />
      </controls>
      <namespaces>
        <add namespace="System.Web.Mvc" />
        <add namespace="System.Web.Mvc.Ajax" />
        <add namespace="System.Web.Mvc.Html" />
        <add namespace="System.Web.Routing" />
        <add namespace="System.Linq" />
        <add namespace="System.Collections.Generic" />
      </namespaces>
    </pages>
    <membership defaultProvider="sitecore" hashAlgorithmType="SHA1">
      <providers>
        <clear />
        <add name="sitecore" type="Sitecore.Security.SitecoreMembershipProvider, Sitecore.Kernel" realProviderName="sql" providerWildcard="%" raiseEvents="true" />
        <add name="sql" type="System.Web.Security.SqlMembershipProvider" connectionStringName="core" applicationName="sitecore" minRequiredPasswordLength="1" minRequiredNonalphanumericCharacters="0" requiresQuestionAndAnswer="false" requiresUniqueEmail="false" maxInvalidPasswordAttempts="256" />
        <add name="switcher" type="Sitecore.Security.SwitchingMembershipProvider, Sitecore.Kernel" applicationName="sitecore" mappings="switchingProviders/membership" />
      </providers>
    </membership>
    <roleManager defaultProvider="sitecore" enabled="true">
      <providers>
        <clear />
        <add name="sitecore" type="Sitecore.Security.SitecoreRoleProvider, Sitecore.Kernel" realProviderName="sql" raiseEvents="true" />
        <add name="sql" type="System.Web.Security.SqlRoleProvider" connectionStringName="core" applicationName="sitecore" />
        <add name="switcher" type="Sitecore.Security.SwitchingRoleProvider, Sitecore.Kernel" applicationName="sitecore" mappings="switchingProviders/roleManager" />
      </providers>
    </roleManager>
    <profile defaultProvider="sql" enabled="true" inherits="Sitecore.Security.UserProfile, Sitecore.Kernel">
      <providers>
        <clear />
        <add name="sql" type="System.Web.Profile.SqlProfileProvider" connectionStringName="core" applicationName="sitecore" />
        <add name="switcher" type="Sitecore.Security.SwitchingProfileProvider, Sitecore.Kernel" applicationName="sitecore" mappings="switchingProviders/profile" />
      </providers>
      <properties>
        <clear />
        <add type="System.String" name="SC_UserData" />
      </properties>
    </profile>
    <!--  DYNAMIC DEBUG COMPILATION
          Set compilation debug="true" to enable ASPX debugging.  Otherwise, setting this value to
          false will improve runtime performance of this application.
          Set compilation debug="true" to insert debugging symbols (.pdb information)
          into the compiled page. Because this creates a larger file that executes
          more slowly, you should set this value to true only when debugging and to
          false at all other times. For more information, refer to the documentation about
          debugging ASP .NET files.
    -->
    <compilation defaultLanguage="c#" debug="false" targetFramework="4.5">
      <assemblies>
        <add assembly="System.Web.Abstractions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Web.Routing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
        <add assembly="System.Web.Helpers, Version=3.0.0.0, Culture=neutral,PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Web.Mvc, Version=5.2.3.0, Culture=neutral,PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Web.WebPages, Version=3.0.0.0, Culture=neutral,PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Web.WebPages.Razor, Version=3.0.0.0, Culture=neutral,PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Web.Http, Version=5.2.3.0, Culture=neutral,PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Web.Http.WebHost, Version=5.2.3.0, Culture=neutral,PublicKeyToken=31BF3856AD364E35" />
        <add assembly="System.Net.Http.Formatting, Version=5.2.3.0, Culture=neutral,PublicKeyToken=31BF3856AD364E35" />
      </assemblies>
    </compilation>
    <!--  CUSTOM ERROR MESSAGES
          Set customError mode values to control the display of user-friendly
          error messages to users instead of error details (including a stack trace):

          "On" Always display custom (friendly) messages
          "Off" Always display detailed ASP.NET error information.
          "RemoteOnly" Display custom (friendly) messages only to users not running
          on the local Web server. This setting is recommended for security purposes, so
          that you do not display application detail information to remote clients.
    -->
    <customErrors mode="RemoteOnly" />
    <!--  AUTHENTICATION
          This section sets the authentication policies of the application. Possible modes are "Windows", "Forms",
          "Passport" and "None"
    -->
    <authentication mode="None">
      <forms name=".ASPXAUTH" cookieless="UseCookies" />
    </authentication>
    <!--  IDENTITY
          If this setting is true, aspnet will run in the security context of the IIS authenticated
          user (ex. IUSR_xxx).
          If false, aspnet will run in the security context of the default ASPNET user.
    -->
    <identity impersonate="false" />
    <!--  APPLICATION-LEVEL TRACE LOGGING
          Application-level tracing enables trace log output for every page within an application.
          Set trace enabled="true" to enable application trace logging.  If pageOutput="true", the
          trace information will be displayed at the bottom of each page.  Otherwise, you can view the
          application trace log by browsing the "trace.axd" page from your web application
          root.
    -->
    <trace enabled="false" requestLimit="50" pageOutput="false" traceMode="SortByTime" localOnly="true" />
    <!--  SESSION STATE SETTINGS
          By default ASP .NET uses cookies to identify which requests belong to a particular session.
          If cookies are not available, a session can be tracked by adding a session identifier to the URL.
          To disable cookies, in the sessionState element, set the cookieless attribute to "true".

          Sitecore does not support cookieless sessions.
          <sessionState mode="InProc" cookieless="false" timeout="20"/>
          <sessionState mode="StateServer" stateConnectionString="tcpip=127.0.0.1:42424" sqlConnectionString="data source=127.0.0.1;user id=sa;password=" cookieless="false" timeout="20"/>
          
          To enable an out-of-process sessionState:           
          1) Set the value of the mode attribute to "custom".
          2) Add the customProvider attribute and set the value to one of the providers, e.g. "mongo".
          3) Ensure that you have configured a connection string for the out-of-process session provider, e.g. "session".
          
          Example: 
          <sessionState mode="Custom" customProvider="mongo" cookieless="false" timeout="20" sessionIDManagerType="Sitecore.SessionManagement.ConditionalSessionIdManager">
            <providers>
              <add name="mongo" type="Sitecore.SessionProvider.MongoDB.MongoSessionStateProvider, Sitecore.SessionProvider.MongoDB" sessionType="Standard" connectionStringName="session" pollingInterval="2" compression="true" />
              <add name="mssql" type="Sitecore.SessionProvider.Sql.SqlSessionStateProvider, Sitecore.SessionProvider.Sql" sessionType="Standard" connectionStringName="session" pollingInterval="2" compression="true" />
            </providers>
          </sessionState>
    -->
    <sessionState mode="InProc" cookieless="false" timeout="20" sessionIDManagerType="Sitecore.SessionManagement.ConditionalSessionIdManager">
      <providers>
        <add name="mongo" type="Sitecore.SessionProvider.MongoDB.MongoSessionStateProvider, Sitecore.SessionProvider.MongoDB" sessionType="Standard" connectionStringName="session" pollingInterval="2" compression="true" />
        <add name="mssql" type="Sitecore.SessionProvider.Sql.SqlSessionStateProvider, Sitecore.SessionProvider.Sql" sessionType="Standard" connectionStringName="session" pollingInterval="2" compression="true" />
      </providers>
    </sessionState>
    <!--  GLOBALIZATION
          This section sets the globalization settings of the application.
    -->
    <globalization requestEncoding="utf-8" responseEncoding="utf-8" />
    <!--
      httpRuntime Attributes:
        executionTimeout="[seconds]" - time in seconds before request is automatically timed out
        maxRequestLength="[KBytes]" - KBytes size of maximum request length to accept
        useFullyQualifiedRedirectUrl="[true|false]" - Fully qualifiy the URL for client redirects
        minFreeThreads="[count]" - minimum number of free thread to allow execution of new requests
        minLocalRequestFreeThreads="[count]" - minimum number of free thread to allow execution of new local requests
        appRequestQueueLimit="[count]" - maximum number of requests queued for the application

        If you change the maxRequestLength setting, you should also change the Media.MaxSizeInDatabase setting.
        Media.MaxSizeInDatabase should always be less than maxRequestLength.
      -->
    <httpRuntime maxRequestLength="512000" executionTimeout="600" enableKernelOutputCache="false" relaxedUrlToFileSystemMapping="true" />
  </system.web>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Lucene.Net" publicKeyToken="85089178b9ac3181" />
        <bindingRedirect oldVersion="0.0.0.0-2.9.4.0" newVersion="3.0.3.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" />
        <bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" xmlns="urn:schemas-microsoft-com:asm.v1" />
        <bindingRedirect oldVersion="0.0.0.0-5.2.3.0" newVersion="5.2.3.0" xmlns="urn:schemas-microsoft-com:asm.v1" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.WebPages.Razor" publicKeyToken="31bf3856ad364e35" xmlns="urn:schemas-microsoft-com:asm.v1" />
        <bindingRedirect oldVersion="1.0.0.0-3.0.0.0" newVersion="3.0.0.0" xmlns="urn:schemas-microsoft-com:asm.v1" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Http" publicKeyToken="31bf3856ad364e35" xmlns="urn:schemas-microsoft-com:asm.v1" />
        <bindingRedirect oldVersion="1.0.0.0-5.2.3.0" newVersion="5.2.3.0" xmlns="urn:schemas-microsoft-com:asm.v1" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Http.WebHost" publicKeyToken="31bf3856ad364e35" xmlns="urn:schemas-microsoft-com:asm.v1" />
        <bindingRedirect oldVersion="1.0.0.0-5.2.3.0" newVersion="5.2.3.0" xmlns="urn:schemas-microsoft-com:asm.v1" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Net.Http.Formatting" publicKeyToken="31bf3856ad364e35" xmlns="urn:schemas-microsoft-com:asm.v1" />
        <bindingRedirect oldVersion="1.0.0.0-5.2.3.0" newVersion="5.2.3.0" xmlns="urn:schemas-microsoft-com:asm.v1" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Ninject" publicKeyToken="c7192dc5380945e7" xmlns="urn:schemas-microsoft-com:asm.v1" />
        <codeBase version="3.2.0.0" href="bin\Social\Ninject.dll" xmlns="urn:schemas-microsoft-com:asm.v1" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.WebPages" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.0.0.0" newVersion="3.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="WebGrease" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-1.5.2.14234" newVersion="1.5.2.14234" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
  <system.serviceModel>
    <bindings>
      <basicHttpBinding>
        <binding name="SitecoreApplicationCenter" closeTimeout="00:01:00" openTimeout="00:01:00" receiveTimeout="00:20:00" sendTimeout="00:05:00" allowCookies="false" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard" maxBufferSize="65536" maxBufferPoolSize="524288" maxReceivedMessageSize="65536" messageEncoding="Text" textEncoding="utf-8" transferMode="Buffered" useDefaultWebProxy="true">
          <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384" maxBytesPerRead="4096" maxNameTableCharCount="16384" />
        </binding>
      </basicHttpBinding>
    </bindings>
  </system.serviceModel>
  <jsEngineSwitcher xmlns="http://tempuri.org/JavaScriptEngineSwitcher.Configuration.xsd">
    <core>
      <engines>
        <add name="V8JsEngine" type="JavaScriptEngineSwitcher.V8.V8JsEngine, JavaScriptEngineSwitcher.V8" />
        <add name="MsieJsEngine" type="JavaScriptEngineSwitcher.Msie.MsieJsEngine, JavaScriptEngineSwitcher.Msie" />
      </engines>
    </core>
  </jsEngineSwitcher>
</configuration>
```