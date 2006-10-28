using System.Web.UI;
using DotNetNuke.UI.WebControls;

/// <summary>
/// The NavDataSourceView class encapsulates the capabilities of the NavDataSource data source control.
/// </summary>
public class NavDataSourceView : HierarchicalDataSourceView
{
    private string m_sKey;
    private string m_sNamespace;

    public NavDataSourceView( string viewPath )
    {
        this.m_sNamespace = "MyNS";
        if( viewPath.Length == 0 )
        {
            m_sKey = "";
        }
        else if( viewPath.IndexOf( "\\" ) > - 1 )
        {
            m_sKey = viewPath.Substring( viewPath.LastIndexOf( "\\" ) + 1 ); //, viewPath.Length - viewPath.LastIndexOf("\") - 1)
        }
        else
        {
            m_sKey = viewPath;
        }
    }

    public string Namespace
    {
        get
        {
            return this.m_sNamespace;
        }
        set
        {
            this.m_sNamespace = value;
        }
    }

    /// <Summary>
    /// Starting with the rootNode, recursively build a list of
    /// PageInfo nodes, create PageHierarchyData
    /// objects, add them all to the PageHierarchicalEnumerable, and return the list.
    /// </Summary>
    public override IHierarchicalEnumerable Select()
    {
        NavDataPageHierarchicalEnumerable objPages = new NavDataPageHierarchicalEnumerable();

        DNNNodeCollection objNodes;
        DNNNode objNode;
        objNodes = DotNetNuke.UI.Navigation.GetNavigationNodes(m_sNamespace);
        if (m_sKey.Length > 0)
        {
            objNodes = objNodes.FindNodeByKey(m_sKey).DNNNodes;
        }
        foreach (DNNNode tempLoopVar_objNode in objNodes)
        {
            objNode = tempLoopVar_objNode;
            objPages.Add(new NavDataPageHierarchyData(objNode));
        }
        return objPages;
    }
}