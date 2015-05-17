namespace Sitecore.Pathfinder.Diagnostics
{
  using System;

  public class ConfigurationException : Exception
  {
    private static readonly object[] EmptyArgs = new object[0];

    public ConfigurationException(int text) : base(Texts.Messages[text])
    {
      this.Text = text;
      this.Args = EmptyArgs;
    }

    public ConfigurationException(int text, [NotNull] params object[] args) : base(string.Format(Texts.Messages[text], args))
    {
      this.Text = text;
      this.Args = args;
    }

    [NotNull]
    public object[] Args { get; }

    public int Text { get; }
  }
}
