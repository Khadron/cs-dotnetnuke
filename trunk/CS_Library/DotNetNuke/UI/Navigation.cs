#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.Containers;
using DotNetNuke.UI.WebControls;
using TabInfo=DotNetNuke.Entities.Tabs.TabInfo;

namespace DotNetNuke.UI
{
    public class Navigation
    {
        public enum NavNodeOptions
        {
            IncludeSelf = 1,
            IncludeParent = 2,
            IncludeSiblings = 4,
            MarkPendingNodes = 8,
        }

        public enum ToolTipSource
        {
            TabName = 0,
            Title = 1,
            Description = 2,
            None = 3,
        }

        /// <Summary>
        /// Allows for DNNNode object to be easily obtained based off of passed in ID
        /// </Summary>
        /// <Param name="strID">NodeID to retrieve</Param>
        /// <Param name="strNamespace">
        /// Namespace for node collection (usually control's ClientID)
        /// </Param>
        /// <Param name="objActionRoot">Root Action object used in searching</Param>
        /// <Param name="objModule">Module to base actions off of</Param>
        /// <Returns>DNNNode</Returns>
        public static DNNNode GetActionNode( string strID, string strNamespace, ModuleAction objActionRoot, ActionBase objModule )
        {
            DNNNodeCollection objNodes = GetActionNodes(objActionRoot, objModule, -1);
            DNNNode objNode = objNodes.FindNode(strID);
            DNNNodeCollection objReturnNodes = new DNNNodeCollection(strNamespace);
            objReturnNodes.Import(objNode);
            objReturnNodes[0].ID = strID;
            return objReturnNodes[0];
        }

        /// <Summary>
        /// This function provides a central location to obtain a generic node collection of the actions associated
        /// to a module based off of the current user's context
        /// </Summary>
        /// <Param name="objActionRoot">Root module action</Param>
        /// <Param name="objModule">Module whose actions you wish to obtain</Param>
        public static DNNNodeCollection GetActionNodes( ModuleAction objActionRoot, ActionBase objModule, Control objControl )
        {
            return Navigation.GetActionNodes( objActionRoot, objModule, -1 );
        }

        /// <Summary>
        /// This function provides a central location to obtain a generic node collection of the actions associated
        /// to a module based off of the current user's context
        /// </Summary>
        /// <Param name="objActionRoot">Root module action</Param>
        /// <Param name="objRootNode">Root node on which to populate children</Param>
        /// <Param name="objModule">Module whose actions you wish to obtain</Param>
        /// <Param name="intDepth">How many levels deep should be populated</Param>
        public static DNNNodeCollection GetActionNodes( ModuleAction objActionRoot, DNNNode objRootNode, ActionBase objModule, int intDepth )
        {
            DNNNodeCollection objCol = objRootNode.ParentNode.DNNNodes;
            AddChildActions(objActionRoot, objRootNode, objRootNode, objModule, DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo(), intDepth);
            return objCol;
        }

        /// <Summary>
        /// This function provides a central location to obtain a generic node collection of the actions associated
        /// to a module based off of the current user's context
        /// </Summary>
        /// <Param name="objActionRoot">Root module action</Param>
        /// <Param name="objModule">Module whose actions you wish to obtain</Param>
        /// <Param name="intDepth">How many levels deep should be populated</Param>
        public static DNNNodeCollection GetActionNodes( ModuleAction objActionRoot, ActionBase objModule, int intDepth )
        {
            DNNNodeCollection objCol = new DNNNodeCollection(objModule.ClientID);
            if (objActionRoot.Visible)
            {
                objCol.Add();
                DNNNode objRoot = objCol[0];
                objRoot.ID = objActionRoot.ID.ToString();
                objRoot.Key = objActionRoot.ID.ToString();
                objRoot.Text = objActionRoot.Title;
                objRoot.NavigateURL = objActionRoot.Url;
                objRoot.Image = objActionRoot.Icon;
                AddChildActions(objActionRoot, objRoot, objRoot.ParentNode, objModule, DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo(), intDepth);
            }
            return objCol;
        }

        /// <Summary>
        /// Allows for DNNNode object to be easily obtained based off of passed in ID
        /// </Summary>
        /// <Param name="strID">NodeID to retrieve</Param>
        /// <Param name="strNamespace">
        /// Namespace for node collection (usually control's ClientID)
        /// </Param>
        /// <Returns>DNNNode</Returns>
        public static DNNNode GetNavigationNode( string strID, string strNamespace )
        {
            //TODO:  FIX THIS MESS!
            DNNNodeCollection objNodes = GetNavigationNodes(strNamespace);
            DNNNode objNode = objNodes.FindNode(strID);
            DNNNodeCollection objReturnNodes = new DNNNodeCollection(strNamespace);
            objReturnNodes.Import(objNode);
            objReturnNodes[0].ID = strID;
            return objReturnNodes[0];
        }

        public static DNNNodeCollection GetNavigationNodes( string strNamespace, ToolTipSource eToolTips, int intStartTabId, int intDepth, int intNavNodeOptions )
        {
            DNNNodeCollection objCol = new DNNNodeCollection(strNamespace);
            return GetNavigationNodes(new DNNNode(objCol.XMLNode), eToolTips, intStartTabId, intDepth, intNavNodeOptions);
        }

        public static DNNNodeCollection GetNavigationNodes( DNNNode objRootNode, ToolTipSource eToolTips, int intStartTabId, int intDepth, int intNavNodeOptions )
        {
            int i;
            DotNetNuke.Entities.Portals.PortalSettings objPortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
            bool blnAdminMode = DotNetNuke.Security.PortalSecurity.IsInRoles( objPortalSettings.AdministratorRoleName ) || DotNetNuke.Security.PortalSecurity.IsInRoles( objPortalSettings.ActiveTab.AdministratorRoles.ToString() );
            bool blnFoundStart = intStartTabId == - 1; //if -1 then we want all

            Hashtable objBreadCrumbs = new Hashtable();
            Hashtable objTabLookup = new Hashtable();
            DNNNodeCollection objRootNodes = objRootNode.DNNNodes;
            int intLastBreadCrumbId = 0;
            DotNetNuke.Entities.Tabs.TabInfo objTab;

            //--- cache breadcrumbs in hashtable so we can easily set flag on node denoting it as a breadcrumb node (without looping multiple times) ---'
            for( i = 0; i <= ( objPortalSettings.ActiveTab.BreadCrumbs.Count - 1 ); i++ )
            {
                objBreadCrumbs.Add( ( (TabInfo)objPortalSettings.ActiveTab.BreadCrumbs[i] ).TabID, 1 );
                intLastBreadCrumbId = ( (TabInfo)objPortalSettings.ActiveTab.BreadCrumbs[i] ).TabID;
            }

            for( i = 0; i <= objPortalSettings.DesktopTabs.Count - 1; i++ )
            {
                objTab = (TabInfo)objPortalSettings.DesktopTabs[i];
                objTabLookup.Add( objTab.TabID, objTab );
            }

            for( i = 0; i <= objPortalSettings.DesktopTabs.Count - 1; i++ )
            {
                try
                {
                    objTab = (TabInfo)objPortalSettings.DesktopTabs[i];

                    if( IsTabShown( objTab, blnAdminMode ) ) //based off of tab properties, is it shown
                    {
                        DNNNodeCollection objParentNodes;
                        DNNNode objParentNode = objRootNodes.FindNode( objTab.ParentId.ToString() );
                        bool blnParentFound = objParentNode != null;
                        if( objParentNode == null )
                        {
                            objParentNode = objRootNode;
                        }
                        objParentNodes = objParentNode.DNNNodes;

                        //If objTab.ParentId = -1 OrElse ((intNavNodeOptions And NavNodeOptions.IncludeRootOnly) = 0) Then
                        if( objTab.TabID == intStartTabId )
                        {
                            //is this the starting tab
                            if( ( intNavNodeOptions & (int)NavNodeOptions.IncludeParent ) != 0 )
                            {
                                //if we are including parent, make sure there is one, then add
                                if( objTabLookup[objTab.ParentId] != null )
                                {
                                    AddNode( ( (TabInfo)objTabLookup[objTab.ParentId] ), objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips );
                                    objParentNode = objRootNodes.FindNode( objTab.ParentId.ToString() );
                                    objParentNodes = objParentNode.DNNNodes;
                                }
                            }
                            if( ( intNavNodeOptions & (int)NavNodeOptions.IncludeSelf ) != 0 )
                            {
                                //if we are including our self (starting tab) then add
                                AddNode( objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips );
                            }
                        }
                        else if( ( ( intNavNodeOptions & (int)NavNodeOptions.IncludeSiblings ) != 0 ) && IsTabSibling( objTab, intStartTabId, objTabLookup ) )
                        {
                            //is this a sibling of the starting node, and we are including siblings, then add it
                            AddNode( objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips );
                        }
                        else
                        {
                            if( blnParentFound ) //if tabs parent already in hierarchy (as is the case when we are sending down more than 1 level)
                            {
                                //parent will be found for siblings.  Check to see if we want them, if we don't make sure tab is not a sibling
                                if( ( ( intNavNodeOptions & (int)NavNodeOptions.IncludeSiblings ) != 0 ) || IsTabSibling( objTab, intStartTabId, objTabLookup ) == false )
                                {
                                    //determine if tab should be included or marked as pending
                                    bool blnPOD = ( intNavNodeOptions & (int)NavNodeOptions.MarkPendingNodes ) != 0;
                                    if( IsTabPending( objTab, objParentNode, objRootNode, intDepth, objBreadCrumbs, intLastBreadCrumbId, blnPOD ) )
                                    {
                                        if( blnPOD )
                                        {
                                            objParentNode.HasNodes = true; //mark it as a pending node
                                        }
                                    }
                                    else
                                    {
                                        AddNode( objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips );
                                    }
                                }
                            }
                            else if( ( intNavNodeOptions & (int)NavNodeOptions.IncludeSelf ) == 0 && objTab.ParentId == intStartTabId )
                            {
                                //if not including self and parent is the start id then add
                                AddNode( objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips );
                            }
                        }
                    }
                    //End If
                }
                catch( Exception ex )
                {
                    throw ( ex );
                }
            }

            return objRootNodes;
        }

        /// <Summary>
        /// This function provides a central location to obtain a generic node collection of the pages/tabs included in
        /// the current context's (user) navigation hierarchy
        /// </Summary>
        /// <Param name="strNamespace">
        /// Namespace (typically control's ClientID) of node collection to create
        /// </Param>
        /// <Returns>Collection of DNNNodes</Returns>
        public static DNNNodeCollection GetNavigationNodes( string strNamespace )
        {
            return Navigation.GetNavigationNodes( strNamespace, ToolTipSource.None, -1, -1, 0 );
        }

        private static bool IsActionPending( DNNNode objParentNode, DNNNode objRootNode, int intDepth )
        {
            //if we aren't restricting depth then its never pending
            if (intDepth == -1)
            {
                return false;
            }

            //parents level + 1 = current node level
            //if current node level - (roots node level) <= the desired depth then not pending
            if (objParentNode.Level + 1 - objRootNode.Level <= intDepth)
            {
                return false;
            }
            return true;
        }

        private static bool IsTabPending( TabInfo objTab, DNNNode objParentNode, DNNNode objRootNode, int intDepth, Hashtable objBreadCrumbs, int intLastBreadCrumbId, bool blnPOD )
        {
            //
            // A
            // |
            // --B
            // | |
            // | --B-1
            // | | |
            // | | --B-1-1
            // | | |
            // | | --B-1-2
            // | |
            // | --B-2
            // |   |
            // |   --B-2-1
            // |   |
            // |   --B-2-2
            // |
            // --C
            //   |
            //   --C-1
            //   | |
            //   | --C-1-1
            //   | |
            //   | --C-1-2
            //   |
            //   --C-2
            //     |
            //     --C-2-1
            //     |
            //     --C-2-2

            //if we aren't restricting depth then its never pending
            if (intDepth == -1)
            {
                return false;
            }

            //parents level + 1 = current node level
            //if current node level - (roots node level) <= the desired depth then not pending
            if (objParentNode.Level + 1 - objRootNode.Level <= intDepth)
            {
                return false;
            }

            //--- These checks below are here so tree becomes expands to selected node ---'
            if (blnPOD)
            {
                //really only applies to controls with POD enabled, since the root passed in may be some node buried down in the chain
                //and the depth something like 1.  We need to include the appropriate parent's and parent siblings
                //Why is the check for POD required?  Well to allow for functionality like RootOnly requests.  We do not want any children
                //regardless if they are a breadcrumb

                //if tab is in the breadcrumbs then obviously not pending
                if (objBreadCrumbs.Contains(objTab.TabID))
                {
                    return false;
                }

                //if parent is in the breadcrumb and it is not the last breadcrumb then not pending
                //in tree above say we our breadcrumb is (A, B, B-2) we want our tree containing A, B, B-2 AND B-1 AND C since A and B are expanded
                //we do NOT want B-2-1 and B-2-2, thus the check for Last Bread Crumb
                if (objBreadCrumbs.Contains(objTab.ParentId) && intLastBreadCrumbId != objTab.ParentId)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsTabShown( TabInfo objTab, bool blnAdminMode )
        {
            //if tab is visible, not deleted, not expired (or admin), and user has permission to see it...
            if (objTab.IsVisible && objTab.IsDeleted == false && ((objTab.StartDate < DateTime.Now && objTab.EndDate > DateTime.Now) || blnAdminMode) && DotNetNuke.Security.PortalSecurity.IsInRoles(objTab.AuthorizedRoles))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsTabSibling( TabInfo objTab, int intStartTabId, Hashtable objTabLookup )
        {
            if (intStartTabId == -1)
            {
                if (objTab.ParentId == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (objTab.ParentId == ((TabInfo)objTabLookup[intStartTabId]).ParentId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <Summary>
        /// Recursive function to add module's actions to the DNNNodeCollection based off of passed in ModuleActions
        /// </Summary>
        /// <Param name="objParentAction">Parent action</Param>
        /// <Param name="objParentNode">Parent node</Param>
        /// <Param name="objModule">Module to base actions off of</Param>
        /// <Param name="objUserInfo">User Info Object</Param>
        private static void AddChildActions( ModuleAction objParentAction, DNNNode objParentNode, ActionBase objModule, UserInfo objUserInfo, Control objControl )
        {
            Navigation.AddChildActions( objParentAction, objParentNode, objParentNode, objModule, objUserInfo, -1 );
        }

        /// <Summary>
        /// Recursive function to add module's actions to the DNNNodeCollection based off of passed in ModuleActions
        /// </Summary>
        /// <Param name="objParentAction">Parent action</Param>
        /// <Param name="objParentNode">Parent node</Param>
        /// <Param name="objModule">Module to base actions off of</Param>
        /// <Param name="objUserInfo">User Info Object</Param>
        /// <Param name="intDepth">How many levels deep should be populated</Param>
        private static void AddChildActions( ModuleAction objParentAction, DNNNode objParentNode, DNNNode objRootNode, ActionBase objModule, UserInfo objUserInfo, int intDepth )
        {
            // Add Menu Items

            DotNetNuke.Entities.Modules.Actions.ModuleAction objAction;
            bool blnPending;
            foreach (DotNetNuke.Entities.Modules.Actions.ModuleAction tempLoopVar_objAction in objParentAction.Actions)
            {
                objAction = tempLoopVar_objAction;
                blnPending = IsActionPending(objParentNode, objRootNode, intDepth);
                if (objAction.Title == "~")
                {
                    if (blnPending == false)
                    {
                        //A title (text) of ~ denotes a break
                        objParentNode.DNNNodes.AddBreak();
                    }
                }
                else
                {
                    //if action is visible and user has permission
                    if (objAction.Visible == true && DotNetNuke.Security.PortalSecurity.HasNecessaryPermission(objAction.Secure, ((DotNetNuke.Entities.Portals.PortalSettings)HttpContext.Current.Items["PortalSettings"]), objModule.ModuleConfiguration, objUserInfo.UserID.ToString()))
                    {
                        //(if edit mode and not admintab and not admincontrol) OR (action security is NOT Anonymous and security access is NOT View)
                        if ((objModule.EditMode && objModule.PortalSettings.ActiveTab.IsAdminTab == false && DotNetNuke.Common.Globals.IsAdminControl() == false) || (objAction.Secure != DotNetNuke.Security.SecurityAccessLevel.Anonymous && objAction.Secure != DotNetNuke.Security.SecurityAccessLevel.View))
                        {
                            if (blnPending)
                            {
                                objParentNode.HasNodes = true;
                            }
                            else
                            {
                                DNNNode objNode;
                                int i = objParentNode.DNNNodes.Add();
                                objNode = objParentNode.DNNNodes[i];
                                objNode.ID = objAction.ID.ToString();
                                objNode.Key = objAction.ID.ToString();
                                objNode.Text = objAction.Title; //no longer including SPACE in generic node collection, each control must handle how they want to display
                                if (objAction.ClientScript.Length > 0)
                                {
                                    objNode.JSFunction = objAction.ClientScript;
                                    objNode.ClickAction = eClickAction.None;
                                }
                                else
                                {
                                    objNode.NavigateURL = objAction.Url;
                                    if (objAction.UseActionEvent == false && objNode.NavigateURL.Length > 0)
                                    {
                                        objNode.ClickAction = eClickAction.Navigate;
                                    }
                                    else
                                    {
                                        objNode.ClickAction = eClickAction.PostBack;
                                    }
                                }
                                objNode.Image = objAction.Icon;

                                if (objAction.HasChildren()) //if action has children then call function recursively
                                {
                                    AddChildActions(objAction, objNode, objRootNode, objModule, objUserInfo, intDepth);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assigns common properties from passed in tab to newly created DNNNode that is added to the passed in DNNNodeCollection
        /// </summary>
        /// <param name="objTab">Tab to base DNNNode off of</param>
        /// <param name="objNodes">Node collection to append new node to</param>
        /// <param name="objBreadCrumbs">Hashtable of breadcrumb IDs to efficiently determine node's BreadCrumb property</param>
        /// <param name="objPortalSettings">Portal settings object to determine if node is selected</param>
        /// <remarks>
        /// Logic moved to separate sub to make GetNavigationNodes cleaner
        /// </remarks>
        private static void AddNode( TabInfo objTab, DNNNodeCollection objNodes, Hashtable objBreadCrumbs, PortalSettings objPortalSettings, ToolTipSource eToolTips )
        {
            DNNNode objNode = new DNNNode();

            if( objTab.Title == "~" ) // NEW!
            {
                //A title (text) of ~ denotes a break
                objNodes.AddBreak();
            }
            else
            {
                //assign breadcrumb and selected properties
                if( objBreadCrumbs.Contains( objTab.TabID ) )
                {
                    objNode.BreadCrumb = true;
                    if( objTab.TabID == objPortalSettings.ActiveTab.TabID )
                    {
                        objNode.Selected = true;
                    }
                }

                if( objTab.DisableLink )
                {
                    objNode.Enabled = false;
                }

                objNode.ID = objTab.TabID.ToString();
                objNode.Key = objNode.ID;
                objNode.Text = objTab.TabName;
                objNode.NavigateURL = objTab.FullUrl;
                objNode.ClickAction = eClickAction.Navigate;

                //admin tabs have their images found in a different location, since the DNNNode has no concept of an admin tab, this must be set here
                if( objTab.IsAdminTab )
                {
                    if( !String.IsNullOrEmpty(objTab.IconFile) )
                    {
                        objNode.Image = Common.Globals.ApplicationPath + "/images/" + objTab.IconFile;
                    }
                }
                else
                {
                    if( !String.IsNullOrEmpty(objTab.IconFile) )
                    {
                        objNode.Image = objTab.IconFile;
                    }
                }

                switch( eToolTips )
                {
                    case ToolTipSource.TabName:

                        objNode.ToolTip = objTab.TabName;
                        break;
                    case ToolTipSource.Title:

                        objNode.ToolTip = objTab.Title;
                        break;
                    case ToolTipSource.Description:

                        objNode.ToolTip = objTab.Description;
                        break;
                }

                objNodes.Add( objNode );
            }
        
        }
    }
}