using Sitecore.Data;
using Sitecore.Globalization;

namespace Sitecore.Resources.Media
{
    public class MediaCreatorOptions
    {
        public string AlternateText { get; set; }

        public Database Database { get; set; }

        public bool FileBased { get; set; }

        public bool IncludeExtensionInItemName { get; set; }

        public bool KeepExisting { get; set; }

        public Language Language { get; set; }

        public bool Versioned { get; set; }

        public string Destination { get; set; }
    }
}