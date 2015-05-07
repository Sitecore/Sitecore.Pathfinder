namespace Sitecore.Pathfinder.Building.Builders.ItemFiles
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.SecurityModel;

  [Export(typeof(IEmitter))]
  public class ItemFileEmitter : EmitterBase
  {
    [ImportingConstructor]
    public ItemFileEmitter([NotNull] ICompositionService compositionService) : base(Items)
    {
      this.CompositionService = compositionService;
    }

    [NotNull]
    public ICompositionService CompositionService { get; }

    [NotNull]
    [ImportMany]
    public IEnumerable<IItemFileBuilder> ItemFileBuilders { get; private set; }

    public override void Emit(IEmitContext context)
    {
      this.BuildItemFiles(context, context.ProjectDirectory);
    }

    protected virtual ParseResult BuildItemFile([NotNull] IEmitContext context, [NotNull] string databaseName, [NotNull] string fileName, [NotNull] List<Tuple<string, Exception>> retries)
    {
      // todo: use abstract factory pattern
      var itemFileBuildContext = new ItemFileBuildContext(context, databaseName, fileName);
      var buildResult = ParseResult.None;

      foreach (var itemFileBuilder in this.ItemFileBuilders.OrderBy(b => b.Priority))
      {
        if (!itemFileBuilder.CanBuild(itemFileBuildContext))
        {
          continue;
        }

        retries.RemoveAll(t => t.Item1 == fileName);

        try
        {
          // todo: should use a proper user
          using (new SecurityDisabler())
          {
            itemFileBuilder.Build(itemFileBuildContext);
          }
        }
        catch (RetryableBuildException ex)
        {
          retries.Add(new Tuple<string, Exception>(fileName, ex));
          buildResult = ParseResult.Retry;
        }
        catch (BuildException ex)
        {
          context.Trace.TraceError(ex.Text, ex.FileName, ex.Line, ex.Column, ex.Args);
          buildResult = ParseResult.Failed;
        }
        catch (Exception ex)
        {
          retries.Add(new Tuple<string, Exception>(fileName, ex));
          buildResult = ParseResult.Retry;
        }
      }

      return buildResult;
    }

    protected virtual void BuildItemFiles([NotNull] IEmitContext context, [NotNull] string projectDirectory)
    {
      var serializationDirectory = Path.Combine(projectDirectory, "serialization");
      if (!context.FileSystem.DirectoryExists(serializationDirectory))
      {
        return;
      }

      foreach (var databaseDirectory in context.FileSystem.GetDirectories(serializationDirectory))
      {
        var databaseName = Path.GetFileNameWithoutExtension(databaseDirectory) ?? "master";

        var retries = new List<Tuple<string, Exception>>();

        this.BuildItemFiles(context, databaseName, databaseDirectory, retries);

        this.RetryBuildItemFiles(context, databaseName, retries);
      }
    }

    protected virtual void BuildItemFiles([NotNull] IEmitContext context, [NotNull] string databaseName, [NotNull] string directory, [NotNull] List<Tuple<string, Exception>> retries)
    {
      var fileNames = this.GetFileNames(context, directory);

      foreach (var fileName in fileNames)
      {
        var result = this.BuildItemFile(context, databaseName, fileName, retries);
        if (result != ParseResult.Success)
        {
          continue;
        }

        var subFileNames = this.GetFileNames(context, directory, fileName);
        foreach (var subFileName in subFileNames)
        {
          this.BuildItemFile(context, databaseName, subFileName, retries);
        }
      }

      foreach (var subdirectory in context.FileSystem.GetDirectories(directory))
      {
        this.BuildItemFiles(context, databaseName, subdirectory, retries);
      }
    }

    protected virtual void RetryBuildItemFiles([NotNull] IEmitContext context, [NotNull] string databaseName, [NotNull] List<Tuple<string, Exception>> retries)
    {
      var count = retries.Count;

      while (count > 0)
      {
        var list = new List<Tuple<string, Exception>>(retries);
        list.Reverse();

        retries.Clear();

        foreach (var tuple in list)
        {
          var fileName = tuple.Item1;

          try
          {
            this.BuildItemFile(context, databaseName, fileName, retries);
          }
          catch (Exception ex)
          {
            retries.Add(new Tuple<string, Exception>(fileName, ex));
          }
        }

        if (retries.Count >= count)
        {
          // did not succeed to install any items
          break;
        }

        count = retries.Count;
      }

      foreach (var tuple in retries)
      {
        var fileName = tuple.Item1;
        var ex = tuple.Item2;

        var buildException = ex as BuildException;
        if (buildException != null)
        {
          context.Trace.TraceError(buildException.Text, buildException.FileName, buildException.Line, buildException.Column, buildException.Args);
        }
        else if (ex != null)
        {
          context.Trace.TraceError(Texts.Text9998, fileName, 0, 0, ex.Message);
        }
        else
        {
          context.Trace.TraceError(Texts.Text9999, "An error occured in " + fileName);
        }
      }
    }

    [NotNull]
    private IEnumerable<string> GetFileNames([NotNull] IBuildContextBase context, [NotNull] string directory, [NotNull] string parentFileName = "")
    {
      var pattern = string.IsNullOrEmpty(parentFileName) ? "*" : Path.GetFileName(parentFileName) + ".*";

      var fileNames = context.FileSystem.GetFiles(directory, pattern).ToList();

      fileNames.Remove(parentFileName);

      // remove sub file names
      var subFileNames = new List<string>();

      foreach (var fileName in fileNames)
      {
        var subFileName = fileName + ".";
        subFileNames.AddRange(fileNames.Where(f => f.StartsWith(subFileName, StringComparison.OrdinalIgnoreCase)));
      }

      fileNames.RemoveAll(f => subFileNames.Contains(f));

      return fileNames;
    }
  }
}
