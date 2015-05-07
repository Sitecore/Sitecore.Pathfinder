namespace Sitecore.Pathfinder.Parsing
{
  public abstract class ParserBase : IParser
  {
    protected const double BinFiles = 9999;

    protected const double ContentFiles = 2000;

    protected const double Items = 1000;

    protected ParserBase(double sortorder)
    {
      this.Sortorder = sortorder;
    }

    public double Sortorder { get; }

    public abstract void Parse(IParseContext context);
  }
}
