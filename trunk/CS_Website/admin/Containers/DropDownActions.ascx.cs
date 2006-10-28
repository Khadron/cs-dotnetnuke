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

        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

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
            this.cmdGo.Attributes.Add( "onclick", "if (cmdGo_OnClick(dnn.dom.getById(\'" + Control.NavigationControl.ClientID + "\')) == false) return false;" );
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
            DNNNodeCollection objNodes;
            objNodes = Navigation.GetActionNodes( m_menuActionRoot, this, Control.NavigationControl );
            DNNNode objNode;
            foreach( DNNNode tempLoopVar_objNode in objNodes )
            {
                objNode = tempLoopVar_objNode;
                ProcessNodes( objNode );
            }
            Control.Bind( objNodes );

            if( objNodes != null && objNodes.Count > 0 && objNodes[0].DNNNodes.Count > 0 && m_tabPreview == false )
            {
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }

        private void ProcessNodes( DNNNode objParent )
        {
            if( objParent.JSFunction.Length > 0 )
            {
                ClientAPI.RegisterClientVariable( this.Page, "__dnn_CSAction_" + this.Control.NavigationControl.ClientID + "_" + objParent.ID, objParent.JSFunction, true );
            }

            objParent.ClickAction = eClickAction.None; //since GO button is handling actions dont allow selected index change fire postback

            DNNNode objNode;
            foreach( DNNNode tempLoopVar_objNode in objParent.DNNNodes )
            {
                objNode = tempLoopVar_objNode;
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