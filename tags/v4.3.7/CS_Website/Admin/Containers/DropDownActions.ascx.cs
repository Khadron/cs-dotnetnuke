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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.Containers
{
    public partial class DropDownActions : ActionBase
    {
        private NavigationProvider m_objControl;
        private string m_strProviderName = "DNNDropDownNavigationProvider";


        public string ProviderName
        {
            get
            {
                return m_strProviderName;
            }
            set
            {
                //m_strProviderName = Value 'always is dropdown!
            }
        }

        public NavigationProvider Control
        {
            get
            {
                return m_objControl;
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            //Put user code to initialize the page here
            this.cmdGo.Attributes.Add( "onclick", "if (cmdGo_OnClick(dnn.dom.getById('" + Control.NavigationControl.ClientID + "')) == false) return false;" );
        }

        protected void Page_PreRender( object sender, EventArgs e )
        {
            try
            {
                BindDropDown();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void cmdGo_Click( object sender, ImageClickEventArgs e )
        {
            try
            {
                DropDownList cboActions = (DropDownList)Control.NavigationControl;
                if( cboActions.SelectedIndex != - 1 )
                {
                    ProcessAction( cboActions.SelectedItem.Value );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public void BindDropDown()
        {
            DNNNodeCollection objNodes = Navigation.GetActionNodes( m_menuActionRoot, this, Control.NavigationControl );
            foreach( DNNNode objNode in objNodes )
            {
                ProcessNodes( objNode );
            }
            Control.Bind( objNodes );

            this.Visible = DisplayControl(objNodes);
        }

        private void ProcessNodes( DNNNode objParent )
        {
            if( objParent.JSFunction.Length > 0 )
            {
                ClientAPI.RegisterClientVariable( this.Page, "__dnn_CSAction_" + this.Control.NavigationControl.ClientID + "_" + objParent.ID, objParent.JSFunction, true );
            }

            objParent.ClickAction = eClickAction.None; //since GO button is handling actions dont allow selected index change fire postback

            foreach( DNNNode objNode in objParent.DNNNodes )
            {
                ProcessNodes( objNode );
            }
        }

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            m_objControl = NavigationProvider.Instance( this.ProviderName );
            Control.ControlID = "ctl" + this.ID;
            Control.Initialize();
            spActions.Controls.Add( Control.NavigationControl );
        }
    }
}