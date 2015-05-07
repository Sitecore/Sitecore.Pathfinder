namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;

  [Export(typeof(IPreprocessor))]
  public class WebControlRenderingPreprocessor : RenderingPreprocessorBase
  {
    public WebControlRenderingPreprocessor() : base("aspx-rendering", "/sitecore/templates/System/Layout/Renderings/Webcontrol")
    {
    }

    protected override IEnumerable<string> GetPlaceholders(string contents)
    {
      return this.GetPlaceholdersFromWebFormsFile(contents);
    }
  }
}
