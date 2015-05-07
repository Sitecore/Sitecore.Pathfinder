namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;

  [Export(typeof(IPreprocessor))]
  public class SublayoutRenderingPreprocessor : RenderingPreprocessorBase
  {
    public SublayoutRenderingPreprocessor() : base("ascx-rendering", "/sitecore/templates/System/Layout/Renderings/Sublayout")
    {
    }

    protected override IEnumerable<string> GetPlaceholders(string contents)
    {
      return this.GetPlaceholdersFromWebFormsFile(contents);
    }
  }
}
