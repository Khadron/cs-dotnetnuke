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
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.Utilities
{
    /// <Summary>Library responsible for interacting with DNN Client API.</Summary>
    public class DNNClientAPI
    {
        public enum MinMaxPersistanceType
        {
            None = 0,
            Page = 1,
            Cookie = 2,
        }

        public static bool GetMinMaxContentVisibile( Control objButton, int intModuleId, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType )
        {
            if( HttpContext.Current != null )
            {
                switch( ePersistanceType )
                {
                    case MinMaxPersistanceType.Page:

                        string sExpanded = ClientAPI.GetClientVariable( objButton.Page, objButton.ClientID + ":exp" );
                        if( !String.IsNullOrEmpty( sExpanded ) )
                        {
                            bool boolResult;
                            if(Boolean.TryParse( sExpanded, out boolResult ))
                            {
                                return boolResult;
                            }
                            return !blnDefaultMin;
                        }
                        else
                        {
                            return ! blnDefaultMin;
                        }
                        
                    case MinMaxPersistanceType.Cookie:

                        if( intModuleId != - 1 )
                        {
                            HttpCookie objModuleVisible = HttpContext.Current.Request.Cookies["_Module" + intModuleId + "_Visible"];
                            if( objModuleVisible != null )
                            {
                                return objModuleVisible.Value != "false";
                            }
                            else
                            {
                                return ! blnDefaultMin;
                            }
                        }
                        else
                        {
                            return true;
                        }
                        
                    case MinMaxPersistanceType.None:

                        return ! blnDefaultMin;
                }
            }
            
            return false;
            
        }

        public static bool GetMinMaxContentVisibile( Control objButton, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType )
        {
            return GetMinMaxContentVisibile( objButton, -1, blnDefaultMin, ePersistanceType );
        }

        private static void AddAttribute( Control objControl, string strName, string strValue )
        {
            if (objControl is HtmlControl)
            {
                ((HtmlControl)objControl).Attributes.Add(strName, strValue);
            }
            else if (objControl is WebControl)
            {
                ((WebControl)objControl).Attributes.Add(strName, strValue);
            }
        }

        /// <Summary>Adds client side body.onload event handler</Summary>
        /// <Param name="objPage">Current page rendering content</Param>
        /// <Param name="strJSFunction">Javascript function name to execute</Param>
        public static void AddBodyOnloadEventHandler( Page objPage, string strJSFunction )
        {
            if (strJSFunction.EndsWith(";") == false)
            {
                strJSFunction += ";";
            }
            ClientAPI.RegisterClientReference(objPage, ClientAPI.ClientNamespaceReferences.dnn);
            if (ClientAPI.GetClientVariable(objPage, "__dnn_pageload").IndexOf(strJSFunction) == -1)
            {
                ClientAPI.RegisterClientVariable(objPage, "__dnn_pageload", strJSFunction, false);
            }
        }

        private static void AddStyleAttribute( Control objControl, string strName, string strValue )
        {
            if (objControl is HtmlControl)
            {
                // HACK : Modified to not error if object is null.
                //if (strValue.Length > 0)
                if (!String.IsNullOrEmpty(strValue))
                {
                    ((HtmlControl)objControl).Style.Add(strName, strValue);
                }
                else
                {
                    ((HtmlControl)objControl).Style.Remove(strName);
                }
            }
            else if (objControl is WebControl)
            {
                // HACK : Modified to not error if object is null.
                //if (strValue.Length > 0)
                if (!String.IsNullOrEmpty(strValue))
                {
                    ((WebControl)objControl).Style.Add(strName, strValue);
                }
                else
                {
                    ((WebControl)objControl).Style.Remove(strName);
                }
            }
        }

        /// <Summary>
        /// Allows any module to have drag and drop functionality enabled
        /// </Summary>
        /// <Param name="objTitle">
        /// Title element that responds to the click and dragged
        /// </Param>
        public static void EnableContainerDragAndDrop( Control objTitle, Control objContainer, int ModuleID )
        {
            if (ClientAPI.ClientAPIDisabled() == false && ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.Positioning))
            {
                AddBodyOnloadEventHandler(objTitle.Page, "__dnn_enableDragDrop()");
                ClientAPI.RegisterClientReference(objTitle.Page, ClientAPI.ClientNamespaceReferences.dnn_dom_positioning);
                ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_dragDrop", objContainer.ClientID + " " + objTitle.ClientID + " " + ModuleID + ";", false);

                string strPanes = "";
                string strPaneNames = "";
                PortalSettings objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                foreach (string strPane in objPortalSettings.ActiveTab.Panes)
                {                    
                    Control objCtl = Common.Globals.FindControlRecursive(objContainer.Parent, strPane);
                    if (objCtl != null)
                    {
                        strPanes += objCtl.ClientID + ";";
                    }
                    strPaneNames += strPane + ";";
                }
                ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_Panes", strPanes, true);
                ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_PaneNames", strPaneNames, true);
            }
        }

        public static void EnableMinMax( Control objButton, Control objContent, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType )
        {
            EnableMinMax( objButton, objContent, -1, blnDefaultMin, "", "", ePersistanceType );
        }

        public static void EnableMinMax( Control objButton, Control objContent, int intModuleId, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType )
        {
            EnableMinMax( objButton, objContent, intModuleId, blnDefaultMin, "", "", ePersistanceType );
        }

        public static void EnableMinMax( Control objButton, Control objContent, int intModuleId, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType, int intAnimationFrames )
        {
            if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
            {
                ClientAPI.RegisterClientReference(objButton.Page, ClientAPI.ClientNamespaceReferences.dnn_dom);

                switch (ePersistanceType)
                {
                    case MinMaxPersistanceType.None:

                        AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" + objContent.ClientID + "')) return false;");

                        // HACK : Modified to not error if object is null.
                        //if (strMinIconLoc.Length > 0)
                        if (!String.IsNullOrEmpty(strMaxIconLoc))
                        {
                            AddAttribute(objButton, "max_icon", strMaxIconLoc);
                            AddAttribute(objButton, "min_icon", strMinIconLoc);
                        }
                        break;
                    case MinMaxPersistanceType.Page:

                        AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" + objContent.ClientID + "')) return false;");

                        // HACK : Modified to not error if object is null.
                        //if (strMinIconLoc.Length > 0)
                        if (!String.IsNullOrEmpty(strMaxIconLoc))
                        {
                            AddAttribute(objButton, "max_icon", strMaxIconLoc);
                            AddAttribute(objButton, "min_icon", strMinIconLoc);
                        }
                        break;
                    case MinMaxPersistanceType.Cookie:

                        if (intModuleId != -1)
                        {
                            AddAttribute(objButton, "onclick", "if (__dnn_ContainerMaxMin_OnClick(this, '" + objContent.ClientID + "')) return false;");
                            AddAttribute(objButton, "containerid", intModuleId.ToString());
                            AddAttribute(objButton, "cookieid", "_Module" + intModuleId + "_Visible"); //needed to set cookie on the client side

                            ClientAPI.RegisterClientVariable(objButton.Page, "min_icon_" + intModuleId, strMinIconLoc, true);
                            ClientAPI.RegisterClientVariable(objButton.Page, "max_icon_" + intModuleId, strMaxIconLoc, true);

                            ClientAPI.RegisterClientVariable(objButton.Page, "max_text", Localization.GetString("Maximize"), true);
                            ClientAPI.RegisterClientVariable(objButton.Page, "min_text", Localization.GetString("Minimize"), true);

                            if (blnDefaultMin)
                            {
                                ClientAPI.RegisterClientVariable(objButton.Page, "__dnn_" + intModuleId + ":defminimized", "true", true);
                            }
                        }
                        break;
                }
            }

            if (GetMinMaxContentVisibile(objButton, intModuleId, blnDefaultMin, ePersistanceType))
            {
                if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
                {
                    AddStyleAttribute(objContent, "display", "");
                }
                else
                {
                    objContent.Visible = true;
                }

                // HACK : Modified to not error if object is null.
                //if (strMinIconLoc.Length > 0)
                if (!String.IsNullOrEmpty(strMaxIconLoc))
                {
                    SetMinMaxProperties(objButton, strMinIconLoc, Localization.GetString("Minimize"), Localization.GetString("Minimize"));
                }
            }
            else
            {
                if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
                {
                    AddStyleAttribute(objContent, "display", "none");
                }
                else
                {
                    objContent.Visible = false;
                }

                // HACK : Modified to not error if object is null.
                //if (strMaxIconLoc.Length > 0)
                if (!String.IsNullOrEmpty(strMaxIconLoc))
                {
                    SetMinMaxProperties(objButton, strMaxIconLoc, Localization.GetString("Maximize"), Localization.GetString("Maximize"));
                }
            }

            if (intAnimationFrames != 5)
            {
                AddAttribute(objButton, "animf", intAnimationFrames.ToString());
            }
        }

        public static void EnableMinMax( Control objButton, Control objContent, int intModuleId, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType )
        {
            EnableMinMax( objButton, objContent, intModuleId, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType, 5 );
        }

        public static void EnableMinMax( Control objButton, Control objContent, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType )
        {
            EnableMinMax( objButton, objContent, -1, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType );
        }

        public static void SetMinMaxContentVisibile( Control objButton, int intModuleId, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType, bool Value )
        {
            if (HttpContext.Current != null)
            {
                switch (ePersistanceType)
                {
                    case MinMaxPersistanceType.Page:

                        ClientAPI.RegisterClientVariable(objButton.Page, objButton.ClientID + ":exp", Convert.ToInt32(Value).ToString(), true);
                        break;
                    case MinMaxPersistanceType.Cookie:

                        HttpCookie objModuleVisible = new HttpCookie("_Module" + intModuleId + "_Visible");
                        if (objModuleVisible != null)
                        {
                            objModuleVisible.Value = Value.ToString().ToLower();
                            objModuleVisible.Expires = DateTime.MaxValue; // never expires
                            HttpContext.Current.Response.AppendCookie(objModuleVisible);
                        }
                        break;
                }
            }
        }

        public static void SetMinMaxContentVisibile( Control objButton, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType, bool Value )
        {
            SetMinMaxContentVisibile( objButton, -1, blnDefaultMin, ePersistanceType, Value );
        }

        private static void SetImageProperties( Control objControl, string strImage, string strToolTip, string strAltText )
        {
            
            
            if(  ( objControl is Image ) )
            {
                Image objImage = (Image)objControl;
                objImage.ImageUrl = strImage;
                objImage.AlternateText = strAltText;
                objImage.ToolTip = strToolTip;
                return;
            }
            else if(  ( objControl is ImageButton ) )
            {
                ImageButton objImage = (ImageButton)objControl;
                objImage.ImageUrl = strImage;
                objImage.AlternateText = strAltText;
                objImage.ToolTip = strToolTip;
                return;
            }
            else if( ( objControl is HtmlImage ) )
            {
                HtmlImage objImage = (HtmlImage)objControl;
                objImage.Src = strImage;
                objImage.Alt = strAltText;
                return;
            }
            else
            {
                HtmlImage htmlImage1 = ( (HtmlImage)objControl );
                htmlImage1.Src = strImage;
                htmlImage1.Alt = strAltText;
            }
        }

        private static void SetMinMaxProperties( Control objButton, string strImage, string strToolTip, string strAltText )
        {
            
            if ((objButton is LinkButton))
            {
                LinkButton objLB = (LinkButton)objButton;
                objLB.ToolTip = strToolTip;
                if (objLB.Controls.Count > 0)
                {
                    SetImageProperties(objLB.Controls[0], strImage, strToolTip, strAltText);
                }
                return;
            }
            else if ( (objButton is Image))
            {
                SetImageProperties(objButton, strImage, strToolTip, strAltText);
                return;
            }
            else if ( (objButton is ImageButton))
            {
                SetImageProperties(objButton, strImage, strToolTip, strAltText);
            }
            else
                return;
            
        }
    }
}