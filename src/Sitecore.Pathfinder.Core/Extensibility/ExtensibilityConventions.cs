// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition.Registration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Extensibility
{
    public class ExtensibilityConventions
    {
        [NotNull]
        public RegistrationBuilder GetConventions()
        {
            var conventions = new RegistrationBuilder();

            // conventions.ForTypesDerivedFrom<IParser>().Export(c => c.AsContractType<IParser>());

            return conventions;
        }
    }
}
