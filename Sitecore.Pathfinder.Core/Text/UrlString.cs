// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlString.cs" company="Sitecore A/S">
//   Copyright (C) by Sitecore A/S
// </copyright>
// <summary>
//   Creates and builds a string Url.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Pathfinder.Text
{
  using System;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Text;
  using System.Web;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions;

  /// <summary>Creates and builds a string Url.</summary>
  public class UrlString
  {
    private readonly NameValueCollection parameters = new NameValueCollection();

    private string protocol = string.Empty;

    public UrlString()
    {
    }

    public UrlString([NotNull] string url)
    {
      this.Parse(url);
    }

    public UrlString([NotNull] NameValueCollection parameters)
    {
      this.parameters.Add(parameters);
    }

    public string Extension
    {
      set
      {
        var extensionStartIndex = this.Path.LastIndexOf(@".", StringComparison.Ordinal);
        this.Path = this.Path.Substring(0, extensionStartIndex) + @"." + value;
      }
    }

    public bool HasPath => !string.IsNullOrEmpty(this.Path);

    [NotNull]
    public string Hash { get; set; } = string.Empty;

    [NotNull]
    public string HostName { get; set; } = string.Empty;

    [NotNull]
    public NameValueCollection Parameters => this.parameters;

    [NotNull]
    public string Path { get; set; } = string.Empty;

    [NotNull]
    public string Protocol
    {
      get
      {
        var p = this.protocol;
        if (p.Length > 0)
        {
          return p;
        }

        return @"http";
      }

      set
      {
        this.protocol = value;
      }
    }

    [NotNull]
    public string Query
    {
      get
      {
        return this.GetQuery();
      }

      set
      {
        this.parameters.Clear();
        this.ParseQuery(value);
      }
    }

    [CanBeNull]
    public string this[[NotNull] [Localizable(false)] string key]
    {
      get
      {
        return this.parameters[key];
      }

      set
      {
        this.Append(key, value ?? string.Empty);
      }
    }

    [NotNull]
    public string Add([NotNull] string key, [NotNull] string value)
    {
      this.parameters[key] = HttpUtility.UrlEncode(value);

      return this.GetUrl();
    }

    public void Append([NotNull] string key, [NotNull] string value)
    {
      this.parameters[key] = HttpUtility.UrlEncode(value);
    }

    public void Append([NotNull] NameValueCollection arguments)
    {
      foreach (string key in arguments.Keys)
      {
        this.Append(key, arguments[key]);
      }
    }

    public void Append([NotNull] UrlString url)
    {
      this.Append(url.Parameters);
    }

    [NotNull]
    public string GetUrl(bool xhtmlFormat)
    {
      var result = new StringBuilder();

      if (this.HostName.Length > 0)
      {
        result.Append(this.Protocol);
        result.Append(@"://");
        result.Append(this.HostName);
      }

      result.Append(this.Path);

      if (this.Path.Length > 0 && this.parameters.Count > 0)
      {
        result.Append('?');
      }

      result.Append(this.GetQuery());

      if (!string.IsNullOrEmpty(this.Hash))
      {
        result.Append('#');
        result.Append(this.Hash);
      }

      if (xhtmlFormat)
      {
        result.Replace(@"&", @"&amp;");
      }

      return result.ToString();
    }

    [NotNull]
    public string GetUrl()
    {
      return this.GetUrl(false);
    }

    [NotNull]
    public string Remove([NotNull] string key)
    {
      this.parameters.Remove(key);

      return this.GetUrl();
    }

    public override string ToString()
    {
      return this.GetUrl();
    }

    public void Truncate([NotNull] string key)
    {
      this.parameters.Remove(key);
    }

    [NotNull]
    private string GetQuery()
    {
      var result = new StringBuilder();
      var first = true;
      foreach (string key in this.parameters.Keys)
      {
        if (!first)
        {
          result.Append('&');
        }

        result.Append(key);
        if (this.parameters[key] != string.Empty)
        {
          result.Append('=');
          result.Append(this.parameters[key]);
        }

        first = false;
      }

      return result.ToString();
    }

    private void Parse([NotNull] string url)
    {
      var startOfHashSection = url.IndexOf(@"#", StringComparison.Ordinal);

      if (startOfHashSection >= 0)
      {
        this.Hash = url.Mid(startOfHashSection + 1);
        url = url.Left(startOfHashSection);
      }

      var startOfParameterSection = url.IndexOf(@"?", StringComparison.Ordinal);
      var hasPathParameterSeparator = startOfParameterSection >= 0;
      var hasQuery = url.Contains(@"=");
      var hasPath = hasPathParameterSeparator || !hasQuery;

      if (hasPathParameterSeparator && !hasQuery)
      {
        this.Path = url.Left(startOfParameterSection);
        return;
      }

      if (!hasQuery)
      {
        this.Path = url;
        return;
      }

      if (!hasPath)
      {
        this.ParseQuery(url);
        return;
      }

      this.Path = url.Substring(0, startOfParameterSection);

      var parameterSection = url.Substring(startOfParameterSection + 1);

      this.ParseQuery(parameterSection);
    }

    private void ParseQuery([NotNull] string parameterSection)
    {
      var p = parameterSection.Split('&');

      foreach (var parameterNameValue in p)
      {
        var parameterNameValueArray = parameterNameValue.Split('=');

        var value = parameterNameValueArray.Length == 1 ? string.Empty : parameterNameValueArray[1];

        this.parameters.Add(parameterNameValueArray[0], value);
      }
    }
  }
}