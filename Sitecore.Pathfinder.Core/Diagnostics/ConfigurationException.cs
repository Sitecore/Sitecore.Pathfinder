namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel;

  public class ConfigurationException : Exception
  {
    public ConfigurationException([Localizable(true)] [NotNull] string text, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
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
