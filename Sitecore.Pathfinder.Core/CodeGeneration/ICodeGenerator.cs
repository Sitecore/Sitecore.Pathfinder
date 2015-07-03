// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public interface ICodeGenerator
    {
        bool CanGenerate(object instance);

        void Generate(TextWriter output, string fileName, object instance);
    }
}
