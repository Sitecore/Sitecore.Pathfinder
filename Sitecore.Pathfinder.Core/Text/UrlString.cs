// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Text
{
    /// <summary>Creates and builds a string Url.</summary>
    public class UrlString
    {
        private readonly NameValueCollection _parameters = new NameValueCollection();

        private string _protocol = string.Empty;

        public UrlString()
        {
        }

        public UrlString([NotNull] string url)
        {
            Parse(url);
        }

        public UrlString([NotNull] NameValueCollection parameters)
        {
            this._parameters.Add(parameters);
        }

        public string Extension
        {
            set
            {
                var extensionStartIndex = Path.LastIndexOf(@".", StringComparison.Ordinal);
                Path = Path.Substring(0, extensionStartIndex) + @"." + value;
            }
        }

        [NotNull]
        public string Hash { get; set; } = string.Empty;

        public bool HasPath => !string.IsNullOrEmpty(Path);

        [NotNull]
        public string HostName { get; set; } = string.Empty;

        [CanBeNull]
        public string this[[NotNull] [Localizable(false)] string key]
        {
            get
            {
                return _parameters[key];
            }

            set
            {
                Append(key, value ?? string.Empty);
            }
        }

        [NotNull]
        public NameValueCollection Parameters => _parameters;

        [NotNull]
        public string Path { get; set; } = string.Empty;

        [NotNull]
        public string Protocol
        {
            get
            {
                var p = _protocol;
                if (p.Length > 0)
                {
                    return p;
                }

                return @"http";
            }

            set
            {
                _protocol = value;
            }
        }

        [NotNull]
        public string Query
        {
            get
            {
                return GetQuery();
            }

            set
            {
                _parameters.Clear();
                ParseQuery(value);
            }
        }

        [NotNull]
        public string Add([NotNull] string key, [NotNull] string value)
        {
            _parameters[key] = HttpUtility.UrlEncode(value);

            return GetUrl();
        }

        public void Append([NotNull] string key, [NotNull] string value)
        {
            _parameters[key] = HttpUtility.UrlEncode(value);
        }

        public void Append([NotNull] NameValueCollection arguments)
        {
            foreach (string key in arguments.Keys)
            {
                Append(key, arguments[key]);
            }
        }

        public void Append([NotNull] UrlString url)
        {
            Append(url.Parameters);
        }

        [NotNull]
        public string GetUrl(bool xhtmlFormat)
        {
            var result = new StringBuilder();

            if (HostName.Length > 0)
            {
                result.Append(Protocol);
                result.Append(@"://");
                result.Append(HostName);
            }

            result.Append(Path);

            if (Path.Length > 0 && _parameters.Count > 0)
            {
                result.Append('?');
            }

            result.Append(GetQuery());

            if (!string.IsNullOrEmpty(Hash))
            {
                result.Append('#');
                result.Append(Hash);
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
            return GetUrl(false);
        }

        [NotNull]
        public string Remove([NotNull] string key)
        {
            _parameters.Remove(key);

            return GetUrl();
        }

        public override string ToString()
        {
            return GetUrl();
        }

        public void Truncate([NotNull] string key)
        {
            _parameters.Remove(key);
        }

        [NotNull]
        private string GetQuery()
        {
            var result = new StringBuilder();
            var first = true;
            foreach (string key in _parameters.Keys)
            {
                if (!first)
                {
                    result.Append('&');
                }

                result.Append(key);
                if (_parameters[key] != string.Empty)
                {
                    result.Append('=');
                    result.Append(_parameters[key]);
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
                Hash = url.Mid(startOfHashSection + 1);
                url = url.Left(startOfHashSection);
            }

            var startOfParameterSection = url.IndexOf(@"?", StringComparison.Ordinal);
            var hasPathParameterSeparator = startOfParameterSection >= 0;
            var hasQuery = url.Contains(@"=");
            var hasPath = hasPathParameterSeparator || !hasQuery;

            if (hasPathParameterSeparator && !hasQuery)
            {
                Path = url.Left(startOfParameterSection);
                return;
            }

            if (!hasQuery)
            {
                Path = url;
                return;
            }

            if (!hasPath)
            {
                ParseQuery(url);
                return;
            }

            Path = url.Substring(0, startOfParameterSection);

            var parameterSection = url.Substring(startOfParameterSection + 1);

            ParseQuery(parameterSection);
        }

        private void ParseQuery([NotNull] string parameterSection)
        {
            var p = parameterSection.Split('&');

            foreach (var parameterNameValue in p)
            {
                var parameterNameValueArray = parameterNameValue.Split('=');

                var value = parameterNameValueArray.Length == 1 ? string.Empty : parameterNameValueArray[1];

                _parameters.Add(parameterNameValueArray[0], value);
            }
        }
    }
}
