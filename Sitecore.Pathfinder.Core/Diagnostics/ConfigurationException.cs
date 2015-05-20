namespace Sitecore.Pathfinder.Diagnostics
{
  using System;

  public class ConfigurationException : Exception
  {
    private static readonly object[] EmptyArgs = new object[0];

    public ConfigurationException([NotNull] string text, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
    {
      this.Text = text;
      this.Details = details;
    }

    [NotNull]
    public string Details { get; }

    [NotNull]
    public string Text { get; }
  }
}
