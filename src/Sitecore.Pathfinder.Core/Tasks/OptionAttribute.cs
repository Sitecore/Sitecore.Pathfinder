// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    public delegate Dictionary<string, string> GetOptionsDelegate();

    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute([NotNull] string name)
        {
            Name = name;
        }

        [NotNull]
        public string Alias { get; set; } = string.Empty;

        [NotNull]
        public string DefaultValue { get; set; } = string.Empty;

        [CanBeNull]
        public GetOptionsDelegate GetOptions { get; set; }

        [NotNull]
        public string HelpText { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = false;

        [NotNull]
        public string Name { get; }

        public int PositionalArg { get; set; } = -1;

        [NotNull]
        public string PromptText { get; set; } = string.Empty;
    }
}
