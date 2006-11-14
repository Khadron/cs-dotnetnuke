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
using System.Diagnostics;
using DotNetNuke.Common;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.Skins.Controls
{
    public partial class Nav : NavObjectBase
    {
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                bool blnIndicateChildren = bool.Parse( GetValue( this.IndicateChildren, "True" ) ); //This setting determines if the submenu arrows will be used
                //Dim blnRootOnly As Boolean = Boolean.Parse(GetValue(RootOnly, "False"))				'This setting determines if the submenu will be shown
                string strRightArrow;
                string strDownArrow;

                //If blnRootOnly Then blnIndicateChildren = False

                SkinController objSkins = new SkinController();

                //image for right facing arrow
                if( IndicateChildImageSub != "" )
                {
                    strRightArrow = IndicateChildImageSub;
                }
                else
                {
                    strRightArrow = "[APPIMAGEPATH]breadcrumb.gif";
                }
                //image for down facing arrow
                if( IndicateChildImageRoot != "" )
                {
                    strDownArrow = IndicateChildImageRoot;
                }
                else
                {
                    strDownArrow = "[APPIMAGEPATH]menu_down.gif";
                }

                //Set correct image path for all separator images
                if( SeparatorHTML != "" )
                {
                    SeparatorHTML = FixImagePath( SeparatorHTML );
                }

                if( SeparatorLeftHTML != "" )
                {
                    SeparatorLeftHTML = FixImagePath( SeparatorLeftHTML );
                }
                if( SeparatorRightHTML != "" )
                {
                    SeparatorRightHTML = FixImagePath( SeparatorRightHTML );
                }
                if( SeparatorLeftHTMLBreadCrumb != "" )
                {
                    SeparatorLeftHTMLBreadCrumb = FixImagePath( SeparatorLeftHTMLBreadCrumb );
                }
                if( SeparatorRightHTMLBreadCrumb != "" )
                {
                    SeparatorRightHTMLBreadCrumb = FixImagePath( SeparatorRightHTMLBreadCrumb );
                }
                if( SeparatorLeftHTMLActive != "" )
                {
                    SeparatorLeftHTMLActive = FixImagePath( SeparatorLeftHTMLActive );
                }
                if( SeparatorRightHTMLActive != "" )
                {
                    SeparatorRightHTMLActive = FixImagePath( SeparatorRightHTMLActive );
                }

                if( NodeLeftHTMLBreadCrumbRoot != "" )
                {
                    NodeLeftHTMLBreadCrumbRoot = FixImagePath( NodeLeftHTMLBreadCrumbRoot );
                }
                if( NodeRightHTMLBreadCrumbRoot != "" )
                {
                    NodeRightHTMLBreadCrumbRoot = FixImagePath( NodeRightHTMLBreadCrumbRoot );
                }
                if( NodeLeftHTMLBreadCrumbSub != "" )
                {
                    NodeLeftHTMLBreadCrumbSub = FixImagePath( NodeLeftHTMLBreadCrumbSub );
                }
                if( NodeRightHTMLBreadCrumbSub != "" )
                {
                    NodeRightHTMLBreadCrumbSub = FixImagePath( NodeRightHTMLBreadCrumbSub );
                }
                if( NodeLeftHTMLRoot != "" )
                {
                    NodeLeftHTMLRoot = FixImagePath( NodeLeftHTMLRoot );
                }
                if( NodeRightHTMLRoot != "" )
                {
                    NodeRightHTMLRoot = FixImagePath( NodeRightHTMLRoot );
                }
                if( NodeLeftHTMLSub != "" )
                {
                    NodeLeftHTMLSub = FixImagePath( NodeLeftHTMLSub );
                }
                if( NodeRightHTMLSub != "" )
                {
                    NodeRightHTMLSub = FixImagePath( NodeRightHTMLSub );
                }

                if( this.PathImage.Length == 0 )
                {
                    this.PathImage = PortalSettings.HomeDirectory;
                }
                if( blnIndicateChildren )
                {
                    this.IndicateChildImageSub = strRightArrow;
                    //Me.IndicateChildren = True.ToString
                    if( this.ControlOrientation.ToLower() == "vertical" ) //NavigationProvider.NavigationProvider.Orientation.Vertical Then
                    {
                        this.IndicateChildImageRoot = strRightArrow;
                    }
                    else
                    {
                        this.IndicateChildImageRoot = strDownArrow;
                    }
                }
                else
                {
                    this.IndicateChildImageSub = "[APPIMAGEPATH]spacer.gif";
                }
                this.PathSystemScript = Globals.ApplicationPath + "/controls/SolpartMenu/";
                this.PathSystemImage = "[APPIMAGEPATH]";

                BuildNodes( null );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private string FixImagePath( string strPath )
        {
            if( strPath.IndexOf( "src=" ) != - 1 )
            {
                return strPath.Replace( "src=\"", "src=\"[SKINPATH]" );
            }
            else
            {
                return strPath;
            }
        }

        private void BuildNodes( DNNNode objNode )
        {
            DNNNodeCollection objNodes;
            objNodes = GetNavigationNodes( objNode );
            this.Control.ClearNodes(); //since we always bind we need to clear the nodes for providers that maintain their state
            this.Bind( objNodes );
        }

        protected override void OnInit( EventArgs e )
        {
            InitializeNavControl( this, "SolpartMenuNavigationProvider" );
            Control.NodeClick += new NavigationProvider.NodeClickEventHandler(Control_NodeClick);
            Control.PopulateOnDemand += new NavigationProvider.PopulateOnDemandEventHandler(Control_PopulateOnDemand);

            base.OnInit( e );
        }

        protected void Control_NodeClick( NavigationEventArgs args )
        {
            if( args.Node == null )
            {
                args.Node = Navigation.GetNavigationNode( args.ID, Control.ID );
            }
            Response.Redirect( Globals.ApplicationURL( int.Parse( args.Node.Key ) ), true );
        }

        protected void Control_PopulateOnDemand( NavigationEventArgs args )
        {
            if( args.Node == null )
            {
                args.Node = Navigation.GetNavigationNode( args.ID, Control.ID );
            }
            BuildNodes( args.Node );
        }
    }
}