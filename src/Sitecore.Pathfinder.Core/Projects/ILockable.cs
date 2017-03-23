// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Projects
{
    public enum Locking
    {
        ReadWrite,

        ReadOnly
    }

    public interface ILockable
    {
        Locking Locking { get; }
    }
}
