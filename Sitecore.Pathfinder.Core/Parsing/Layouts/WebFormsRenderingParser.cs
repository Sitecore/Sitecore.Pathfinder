namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.TextDocuments;

  public abstract class WebFormsRenderingParser : RenderingParser
  {
    protected WebFormsRenderingParser([NotNull] string fileExtension, [NotNull] string templateIdOrPath) : base(fileExtension, templateIdOrPath)
    {
    }

    protected override IEnumerable<string> GetPlaceholders(IParseContext context, ISourceFile sourceFile)
    {
      var contents = sourceFile.ReadAsText(context);

      var matches = Regex.Matches(contents, "<[^>]*Placeholder[^>]*Key=\"([^\"]*)\"[^>]*>", RegexOptions.IgnoreCase);

      return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
    }
  }
}
