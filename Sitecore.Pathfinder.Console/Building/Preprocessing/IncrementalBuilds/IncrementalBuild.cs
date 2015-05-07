namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class IncrementalBuild : TaskBase
  {
    private List<Tuple<string, IPreprocessor>> fileTypes;

    public IncrementalBuild() : base("incremental-build")
    {
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<IPreprocessor> Preprocessors { get; [UsedImplicitly] private set; }

    public override void Execute(IBuildContext buildContext)
    {
      buildContext.Trace.TraceInformation(ConsoleTexts.Text1002);

      var contentDirectory = PathHelper.NormalizeFilePath(buildContext.Configuration.Get(Building.Constants.ContentDirectory) ?? string.Empty).TrimStart('\\');
      var serializationDirectory = PathHelper.NormalizeFilePath(buildContext.Configuration.Get(Building.Constants.SerializationDirectory) ?? string.Empty).TrimStart('\\');
      var databaseName = buildContext.Configuration.Get(Building.Constants.Database);

      var context = new PreprocessingContext(buildContext)
      {
        ItemPath = string.Empty, 
        ContentDirectory = contentDirectory, 
        SerializationDirectory = serializationDirectory, 
        Database = databaseName
      };

      var projectVisitor = new ProjectVisitor(context, PathHelper.Combine(buildContext.SolutionDirectory, buildContext.ProjectDirectory));

      projectVisitor.VisitingFile += this.VisitFile;

      projectVisitor.Visit();

      buildContext.Trace.TraceInformation(ConsoleTexts.Text1003, buildContext.SourceFiles.Count);
    }

    [NotNull]
    protected virtual List<Tuple<string, IPreprocessor>> ParseFileTypes([NotNull] IPreprocessingContext context, [NotNull] IConfiguration configuration)
    {
      var result = new List<Tuple<string, IPreprocessor>>();

      var keys = configuration.GetSubKeys("preprocessing:filetypes");

      foreach (var pair in keys)
      {
        var pattern = pair.Key;
        var preprocessorName = configuration["preprocessing:filetypes:" + pattern];
        var preprocessor = this.Preprocessors.FirstOrDefault(a => string.Compare(a.Name, preprocessorName, StringComparison.OrdinalIgnoreCase) == 0);
        if (preprocessor == null)
        {
          context.BuildContext.Trace.TraceError(ConsoleTexts.Text1003, preprocessorName);
          context.BuildContext.IsDeployable = false;
          continue;
        }

        result.Add(new Tuple<string, IPreprocessor>(pattern, preprocessor));
      }

      return result;
    }

    protected virtual void VisitFile([NotNull] IPreprocessingContext context, [NotNull] string fileName)
    {
      if (this.fileTypes == null)
      {
        this.fileTypes = this.ParseFileTypes(context, context.BuildContext.Configuration).ToList();
      }

      if (!this.fileTypes.Any())
      {
        return;
      }

      foreach (var pair in this.fileTypes)
      {
        var pattern = pair.Item1;
        var preprocessor = pair.Item2;

        if (!PathHelper.MatchesPattern(fileName, pattern))
        {
          continue;
        }

        try
        {
          preprocessor.Execute(context, fileName);
        }
        catch (BuildException ex)
        {
          context.BuildContext.Trace.TraceError(ex.Text, ex.Message);
          context.BuildContext.IsDeployable = false;
        }
      }
    }
  }
}
