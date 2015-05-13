namespace Sitecore.Pathfinder.Parsing
{
  public abstract class ParserBase : IParser
  {
    protected const double BinFiles = 9999;

    protected const double ContentFiles = 9000;

    protected const double Items = 3000;

    protected const double Media = 2000;

    protected const double Renderings = 5000;

    protected const double System = 100;

    protected const double Templates = 1000;

    protected ParserBase(double sortorder)
    {
      this.Sortorder = sortorder;
    }

    public double Sortorder { get; }

    public abstract bool CanParse(IParseContext context);

    public abstract void Parse(IParseContext context);
  }
}
