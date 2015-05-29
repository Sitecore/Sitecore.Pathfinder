namespace Sitecore.Pathfinder.Checking.Checkers
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Documents;

  [Export(typeof(IChecker))]
  public class ReferenceChecker : CheckerBase
  {
    public override void Check(ICheckerContext context)
    {
      foreach (var projectItem in context.Project.Items)
      {
        foreach (var reference in projectItem.References)
        {
          if (!reference.IsValid)
          {
            context.Trace.TraceWarning(Texts.Reference_not_found, projectItem.Snapshot.SourceFile.FileName, reference.SourceTextNode?.Position ?? TextPosition.Empty, reference.TargetQualifiedName);
          }
        }
      }
    }
  }
}
