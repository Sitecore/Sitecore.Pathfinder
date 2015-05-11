namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParser))]
  public class ViewRenderingParser : RenderingParser
  {
    public const string ViewRenderingId = "{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}";

    public ViewRenderingParser() : base(".cshtml", ViewRenderingId)
    {
    }

    protected override IEnumerable<string> GetPlaceholders(IParseContext context, ISourceFile sourceFile)
    {
      var contents = sourceFile.ReadAsText(context);    
                
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
