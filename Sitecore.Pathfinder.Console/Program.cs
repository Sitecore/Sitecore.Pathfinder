namespace Sitecore.Pathfinder
{
  using System.Diagnostics;

  internal class Program
  {
    private static void Main(string[] args)
    {
      Trace.Listeners.Add(new ConsoleTraceListener());

      new Startup().Start();
    }
  }
}
