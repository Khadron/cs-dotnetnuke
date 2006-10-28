using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.UI.WebControls;
using Microsoft.VisualBasic;

namespace DotNetNuke.NavigationControl
{
    public class DNNDropDownNavigationProvider : NavigationProvider
    {
        private DropDownList m_objDropDown;
        private string m_strControlID;

        public DropDownList DropDown
        {
            get
            {
                return m_objDropDown;
            }
        }

        public override Control NavigationControl
        {
            get
            {
                return DropDown;
            }
        }

        public override string ControlID
        {
            get
            {
                return m_strControlID;
            }
            set
            {
                m_strControlID = value;
            }
        }

        public override bool SupportsPopulateOnDemand
        {
            get
            {
                return false;
            }
        }

        public override string CSSControl
        {
            get
            {
                return DropDown.CssClass;
            }
            set
            {
                DropDown.CssClass = value;
            }
        }

        public override void Initialize()
        {
            m_objDropDown = new DropDownList();
            DropDown.ID = m_strControlID;
            DropDown.SelectedIndexChanged += new EventHandler( DropDown_SelectedIndexChanged );
        }

        public override void Bind( DNNNodeCollection objNodes )
        {
            DNNNode objNode;
            string strLevelPrefix;

            foreach( DNNNode tempLoopVar_objNode in objNodes )
            {
                objNode = tempLoopVar_objNode;
                if( objNode.ClickAction == eClickAction.PostBack )
                {
                    DropDown.AutoPostBack = true; //its all or nothing...
                }
                strLevelPrefix = Strings.Space( objNode.Level ).Replace( " ", "_" );
                if( objNode.IsBreak )
                {
                    DropDown.Items.Add( "-------------------" );
                }
                else
                {
                    DropDown.Items.Add( new ListItem( strLevelPrefix + objNode.Text, objNode.ID ) );
                }
                Bind( objNode.DNNNodes );
            }
        }

        private void DropDown_SelectedIndexChanged( object source, EventArgs e )
        {
            if( DropDown.SelectedIndex > - 1 )
            {
                base.RaiseEvent_NodeClick( DropDown.SelectedItem.Value );
            }
        }
    }
}