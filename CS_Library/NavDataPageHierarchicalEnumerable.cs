using System.Collections;
using System.Web.UI;

/// <summary>
/// A collection of PageHierarchyData objects
/// </summary>
public class NavDataPageHierarchicalEnumerable : ArrayList, IHierarchicalEnumerable
{
    /// <Summary>Handles enumeration</Summary>
    public virtual IHierarchyData GetHierarchyData( object enumeratedItem )
    {
        return ( (IHierarchyData)enumeratedItem );
    }
}