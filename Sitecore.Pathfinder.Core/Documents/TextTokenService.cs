// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Documents
{
    [Export(typeof(ITextTokenService))]
    public class TextTokenService : ITextTokenService
    {
        private readonly Dictionary<string, string> _globalTokens = new Dictionary<string, string>();

        public string this[string tokenName]
        {
            get
            {
                string value;
                return _globalTokens.TryGetValue(tokenName, out value) ? value : string.Empty;
            }

            set
            {
                _globalTokens[tokenName] = value;
            }
        }

        public string Replace(string text, Dictionary<string, string> contextTokens)
        {
            foreach (var token in _globalTokens)
            {
                var tokenName = "$" + token.Key;
                text = text.Replace(tokenName, token.Value);
            }

            foreach (var token in contextTokens)
            {
                var tokenName = "$" + token.Key;
                text = text.Replace(tokenName, token.Value);
            }

            return text;
        }
    }
}
