namespace Sitecore.Pathfinder.Diagnostics
{
  using System;

  public class TraceException : Exception
  {
    public TraceException([NotNull] string message) : base(message)
    {
    }
  }
}
