// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Projects
{
    public enum Locking
    {
        ReadWrite,

        ReadOnly,

        CopyOnWrite
    }

    public interface ILockable
    {
        Locking Locking { get; }
    }
}
