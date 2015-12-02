namespace Sitecore.Data.Query {
  using System;

  /// <summary>
  /// Implements support functions for an Sitecore XPath query.
  /// </summary>
  public static class QueryUtil {

    #region Public methods
    
    /// <summary>
    /// Determines whether an object can be resolved.
    /// </summary>
    /// <param name="obj">Obj.</param>
    /// <param name="uri">URI.</param>
    /// <returns>
    /// 	<c>true</c> if an object can be resolved; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanResolve(object obj, DataUri uri) {
      if (obj is ISupportPaths) {
        return true;
      }

      if (!uri.ItemID.IsNull) {
        return true;
      }

      return (!IsQueryPath(uri.Path));
    }

    /// <summary>
    /// Determines whether if a string is a query path.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <returns>
    /// 	<c>true</c> if a string is a query path; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>A query path is defined by containing either a "[" or a "::" string.</remarks>
    static public bool IsQueryPath(string path) {
      return path.IndexOf('[') >= 0 ||
        path.IndexOf("::", StringComparison.InvariantCulture) >= 0;
    }

    #endregion
  }
}
