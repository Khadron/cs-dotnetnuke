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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Containers
{
    /// <summary>
    /// Handles the events for collapsing and expanding modules,
    /// Showing or hiding admin controls when preview is checked
    /// if personalization of the module container and title is allowed for that module.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[sun1]	    2/1/2004	Created
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Visibility : SkinObjectBase
    {
        // private members
        private string _borderWidth;
        private string _minIcon;
        private string _maxIcon;
        private int _animationFrames = 5;

        private PortalModuleBase m_objPortalModule;
        private Panel m_pnlModuleContent;

        public string BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
            }
        }

        public string minIcon
        {
            get
            {
                return _minIcon;
            }
            set
            {
                _minIcon = value;
            }
        }

        public string MaxIcon
        {
            get
            {
                return _maxIcon;
            }
            set
            {
                _maxIcon = value;
            }
        }

        public int AnimationFrames
        {
            get
            {
                return _animationFrames;
            }
            set
            {
                _animationFrames = value;
            }
        }

        public string ResourceFile
        {
            get
            {
                return Localization.GetResourceFile( this, "Visibility.ascx" );
            }
        }

        private PortalModuleBase PortalModule
        {
            get
            {
                if( m_objPortalModule == null )
                {
                    m_objPortalModule = Container.GetPortalModuleBase( this );
                }
                return m_objPortalModule;
            }
        }

        private Panel ModuleContent
        {
            get
            {
                if( m_pnlModuleContent == null )
                {
                    Control objCtl = this.Parent.FindControl( "ModuleContent" );
                    if( objCtl != null )
                    {
                        m_pnlModuleContent = (Panel)objCtl;
                    }
                }
                return m_pnlModuleContent;
            }
        }

        public bool ContentVisible
        {
            get
            {
                if( PortalModule.ModuleConfiguration.Visibility == VisibilityState.Maximized )
                {
                    return DNNClientAPI.GetMinMaxContentVisibile( cmdVisibility, PortalModule.ModuleId, PortalModule.ModuleConfiguration.Visibility == VisibilityState.Minimized, DNNClientAPI.MinMaxPersistanceType.Cookie );
                }
                else if( PortalModule.ModuleConfiguration.Visibility == VisibilityState.Minimized )
                {
                    return DNNClientAPI.GetMinMaxContentVisibile( cmdVisibility, PortalModule.ModuleId, PortalModule.ModuleConfiguration.Visibility == VisibilityState.Minimized, DNNClientAPI.MinMaxPersistanceType.Cookie );
                }
                return true;
            }
            set
            {                                
                DNNClientAPI.SetMinMaxContentVisibile( cmdVisibility, PortalModule.ModuleId, PortalModule.ModuleConfiguration.Visibility == VisibilityState.Minimized, DNNClientAPI.MinMaxPersistanceType.Cookie, value);
            }
        }

        private bool IsAdminControlOrTab
        {
            get
            {
                return ( Globals.IsAdminControl() || PortalModule.PortalSettings.ActiveTab.IsAdminTab );
            }
        }

        private string ModulePath
        {
            get
            {
                return PortalModule.ModuleConfiguration.ContainerPath.Substring( 0, PortalModule.ModuleConfiguration.ContainerPath.LastIndexOf( "/" ) + 1 );
            }
        }

        private string MinIconLoc
        {
            get
            {
                if( !String.IsNullOrEmpty(minIcon) )
                {
                    return ModulePath + minIcon;
                }
                else
                {
                    //Return "~/images/min.gif"
                    return Globals.ApplicationPath + "/images/min.gif"; //is ~/ the same as ApplicationPath in all cases?
                }
            }
        }

        private string MaxIconLoc
        {
            get
            {
                if( !String.IsNullOrEmpty(MaxIcon) )
                {
                    return ModulePath + MaxIcon;
                }
                else
                {
                    //Return "~/images/max.gif"
                    return Globals.ApplicationPath + "/images/max.gif"; //is ~/ the same as ApplicationPath in all cases?
                }
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    // public attributes
                    if( !String.IsNullOrEmpty(BorderWidth) )
                    {
                        cmdVisibility.BorderWidth = Unit.Parse( BorderWidth );
                    }

                    if( PortalModule.ModuleConfiguration != null )
                    {
                        // check if Personalization is allowed
                        if( PortalModule.ModuleConfiguration.Visibility == VisibilityState.None )
                        {
                            cmdVisibility.Enabled = false;
                            cmdVisibility.Visible = false;
                        }

                        if( PortalModule.ModuleConfiguration.Visibility == VisibilityState.Minimized )
                        {
                            //if visibility is set to minimized, then the client needs to set the cookie for maximized only and delete the cookie for minimized,
                            //instead of the opposite.  We need to notify the client of this
                            ClientAPI.RegisterClientVariable( this.Page, "__dnn_" + PortalModule.ModuleId + ":defminimized", "true", true );
                        }

                        if( IsAdminControlOrTab == false )
                        {
                            if( cmdVisibility.Enabled )
                            {
                                if( ModuleContent != null )
                                {
                                    //EnableMinMax now done in prerender
                                }
                                else
                                {
                                    this.Visible = false;
                                }
                            }
                        }
                        else
                        {
                            this.Visible = false;
                        }
                    }
                    else
                    {
                        this.Visible = false;
                    }
                }
                else
                {
                    //since we disabled viewstate on the cmdVisibility control we need to check to see if we need hide this on postbacks as well
                    if( PortalModule.ModuleConfiguration != null )
                    {
                        if( PortalModule.ModuleConfiguration.Visibility == VisibilityState.None )
                        {
                            cmdVisibility.Enabled = false;
                            cmdVisibility.Visible = false;
                        }
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void Page_PreRender( object sender, EventArgs e )
        {
            if( ModuleContent != null && PortalModule != null && IsAdminControlOrTab == false )
            {
                switch( PortalModule.ModuleConfiguration.Visibility )
                {
                    case VisibilityState.Maximized:
                        DNNClientAPI.EnableMinMax( cmdVisibility, ModuleContent, PortalModule.ModuleId, PortalModule.ModuleConfiguration.Visibility == VisibilityState.Minimized, MinIconLoc, MaxIconLoc, DNNClientAPI.MinMaxPersistanceType.Cookie, this.AnimationFrames );
                        break;

                    case VisibilityState.Minimized:

                        DNNClientAPI.EnableMinMax( cmdVisibility, ModuleContent, PortalModule.ModuleId, PortalModule.ModuleConfiguration.Visibility == VisibilityState.Minimized, MinIconLoc, MaxIconLoc, DNNClientAPI.MinMaxPersistanceType.Cookie, this.AnimationFrames );
                        break;
                }
            }
        }

        protected void cmdVisibility_Click( Object sender, EventArgs e )
        {
            try
            {
                if( ModuleContent != null )
                {
                    if( ModuleContent.Visible  )
                    {
                        this.ContentVisible = false;
                    }
                    else
                    {
                        this.ContentVisible = true;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}