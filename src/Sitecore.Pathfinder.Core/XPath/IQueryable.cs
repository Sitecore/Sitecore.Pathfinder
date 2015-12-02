
namespace Sitecore.Pathfinder.XPath {

  public interface IQueryable {
    ID[] GetChildren(ID itemID);

    string GetFieldValue(ID itemID, string fieldName);

    string GetName(ID itemID);

    ID GetParentID(ID itemID);

    Item GetQueryContextItem(ID itemID);
  }
}
