namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Sitecore.Pathfinder.Extensions.StringExtensions;

  [Export(typeof(IPreprocessor))]
  public class ViewRenderingPreprocessor : RenderingPreprocessorBase
  {
    public ViewRenderingPreprocessor() : base("view-rendering", "/sitecore/templates/System/Layout/Renderings/View rendering")
    {
    }

    protected override IEnumerable<string> GetPlaceholders(string contents)
    {
      var matches = Regex.Matches(contents, "\\@Html\\.Sitecore\\(\\)\\.Placeholder\\(([^\"\\)]*)\"([^\"]*)\"\\)", RegexOptions.IgnoreCase);

      var result = new List<string>();
      foreach (var match in matches.OfType<Match>())
      {
        var prefix = match.Groups[1].ToString().Trim();
        var name = match.Groups[2].ToString().Trim();

        if (!string.IsNullOrEmpty(prefix))
        {
          if (name.StartsWith("."))
          {
            name = name.Mid(1);
          }

          name = "$Id." + name;
        }

        result.Add(name);
      }

      return result;
    }
  }
}
