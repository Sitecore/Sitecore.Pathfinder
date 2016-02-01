// © 2015-2016 Sitecore Corporation A/S. All rights reserved.
namespace Sitecore.Data.Items
{
    public class LayoutItem : CustomBaseItem
    {
        public LayoutItem([NotNull] Item innerItem) : base(innerItem)
        {
        }

        [NotNull]
        public string FilePath
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}