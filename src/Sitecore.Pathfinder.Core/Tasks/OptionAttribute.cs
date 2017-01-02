// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
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

        [NotNull]
        public string HelpText { get; set; } = string.Empty;

        [NotNull]
        public string Name { get; }

        [NotNull]
        public string PromptText { get; set; } = string.Empty;
    }
}
