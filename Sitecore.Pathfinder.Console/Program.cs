namespace Sitecore.Pathfinder
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;

  internal class Program
  {
    private static void Main([NotNull] string[] args)
    {
      Trace.Listeners.Add(new ConsoleTraceListener());

      new Startup().Start();
    }                                      
  }                                 
}                                     
