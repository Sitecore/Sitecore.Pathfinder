// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Install.Utils
{
    public enum InstallMode
    {
        Overwrite = 1,

        Merge = 2,

        Skip = 3,

        SideBySide = 4,

        Undefined = 0
    }

    public enum MergeMode
    {
        Clear = 1,

        Append = 2,

        Merge = 3,

        Undefined = 0
    }

    public class BehaviourOptions
    {
        public BehaviourOptions(InstallMode installMode, MergeMode mergeMode)
        {
            throw new NotImplementedException();
        }
    }
}
