#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
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