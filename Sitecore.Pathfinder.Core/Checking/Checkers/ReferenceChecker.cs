namespace Sitecore.Pathfinder.Checking.Checkers
{
  using System.ComponentModel.Composition;

  [Export(typeof(IChecker))]
  public class ReferenceChecker : CheckerBase
  {
    public override void Check(ICheckerContext context)
    {
      foreach (var projectItem in context.Project.Items)
      {
        foreach (var reference in projectItem.References)
        {
          if (reference.IsValid)
          {
            continue;
          }

          var textNode = reference.SourceTextNode;

          if (textNode != null)
          {
            context.Trace.TraceWarning(Texts.Text3024, projectItem.Document.SourceFile.FileName, textNode.LineNumber, textNode.LinePosition, reference.TargetQualifiedName);
          }
          else
          {
            context.Trace.TraceWarning(Texts.Text3024, projectItem.Document.SourceFile.FileName, 0, 0, reference.TargetQualifiedName);
          }
        }
      }
    }
  }
}
