#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.HttpModules.Config;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Modules.Admin.Host
{
    /// <summary>
    /// The FriendlyUrls PortalModuleBase is used to edit the friendly urls
    /// for the application.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/06/2006 Created
    /// </history>
    public partial class FriendlyUrls : PortalModuleBase
    {
        private RewriterRuleCollection _Rules;

        private void BindRules()
        {
            grdRules.DataSource = Rules;
            grdRules.DataBind();
        }

        /// <summary>
        /// Gets and sets the mode of the control
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <history>
        /// 	[cnurse]	7/06/2006  Created
        /// </history>
        private bool AddMode
        {
            get
            {
                bool _Mode = Null.NullBoolean;
                if( ViewState["Mode"] != null )
                {
                    _Mode = Convert.ToBoolean( ViewState["Mode"] );
                }
                return _Mode;
            }
            set
            {
                ViewState["Mode"] = value;
            }
        }

        /// <summary>
        /// Gets the collection of rewriter rules
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <history>
        /// 	[cnurse]	7/06/2006  Created
        /// </history>
        private RewriterRuleCollection Rules
        {
            get
            {
                if( _Rules == null )
                {
                    _Rules = RewriterConfiguration.GetConfig().Rules;
                }
                return _Rules;
            }
            set
            {
                _Rules = value;
            }
        }

        protected override void LoadViewState( object savedState )
        {
            object[] myState = (object[])savedState;
            if( ( myState[0] != null ) )
            {
                base.LoadViewState( myState[0] );
            }
            if( ( myState[1] != null ) )
            {
                RewriterConfiguration config = new RewriterConfiguration();

                // Deserialize into RewriterConfiguration
                config = (RewriterConfiguration)XmlUtils.Deserialize( myState[1].ToString(), config.GetType() );
                Rules = config.Rules;
            }
        }

        protected override object SaveViewState()
        {
            RewriterConfiguration config = new RewriterConfiguration();
            config.Rules = Rules;

            object baseState = base.SaveViewState();
            object[] allStates = new object[3];
            allStates[0] = baseState;
            allStates[1] = XmlUtils.Serialize( config );

            return allStates;
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	7/06/2006  Created
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            foreach( DataGridColumn column in grdRules.Columns )
            {
                if( column.GetType() == typeof( ImageCommandColumn ) )
                {
                    //Manage Delete Confirm JS
                    ImageCommandColumn imageColumn = (ImageCommandColumn)column;
                    if( imageColumn.CommandName == "Delete" )
                    {
                        imageColumn.OnClickJS = Localization.GetString( "DeleteItem" );
                    }

                    //Localize Image Column Text
                    if( imageColumn.CommandName != "" )
                    {
                        imageColumn.Text = Localization.GetString( imageColumn.CommandName, this.LocalResourceFile );
                    }
                }
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	7/06/2006  Created
        /// </history>
        protected void Page_Load( object sender, EventArgs e )
        {
            //Localize the Data Grid
            Localization.LocalizeDataGrid( ref grdRules, LocalResourceFile );

            //Bind the rules (as long as not postback)
            if( ! Page.IsPostBack )
            {
                BindRules();
            }
        }

        /// <summary>
        /// AddRule runs when the Add button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	7/06/2006  Created
        /// </history>
        protected void AddRule( object sender, EventArgs e )
        {
            //Add a new empty rule and set the editrow to the new row
            Rules.Add( new RewriterRule() );
            grdRules.EditItemIndex = Rules.Count - 1;

            //Set the AddMode to true
            AddMode = true;

            //Rebind the collection
            BindRules();
        }

        /// <summary>
        /// DeleteRule runs when a delete button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	7/06/2006  Created
        /// </history>
        protected void DeleteRule( object source, DataGridCommandEventArgs e )
        {
            //Get the index of the row to delete
            int index = e.Item.ItemIndex;

            //Remove the rule from the rules collection
            Rules.RemoveAt( index );

            //Save the new collection
            RewriterConfiguration.SaveConfig( Rules );

            //Rebind the collection
            BindRules();
        }

        protected void EditRule( object source, DataGridCommandEventArgs e )
        {
            //Set the AddMode to false
            AddMode = false;

            //Set the editrow
            grdRules.EditItemIndex = e.Item.ItemIndex;

            //Rebind the collection
            BindRules();
        }

        protected void SaveRule( object source, CommandEventArgs e )
        {
            //Get the index of the row to save
            int index = grdRules.EditItemIndex;

            RewriterRule rule = Rules[ index ];
            TextBox ctlMatch = (TextBox)grdRules.Items[ index ].Cells[ 2 ].FindControl( "txtMatch" );
            TextBox ctlReplace = (TextBox)grdRules.Items[ index ].Cells[ 2 ].FindControl( "txtReplace" );

            if( ctlMatch.Text != "" && ctlReplace.Text != "" )
            {
                rule.LookFor = ctlMatch.Text;
                rule.SendTo = ctlReplace.Text;
                //Save the modified collection
                RewriterConfiguration.SaveConfig( Rules );
            }
            else
            {
                if( AddMode )
                {
                    //Remove the temporary added row
                    Rules.RemoveAt( Rules.Count - 1 );
                    AddMode = false;
                }
            }

            //Reset Edit Index
            grdRules.EditItemIndex = - 1;
            BindRules();
        }

        protected void CancelEdit( object source, CommandEventArgs e )
        {
            if( AddMode )
            {
                //Remove the temporary added row
                Rules.RemoveAt( Rules.Count - 1 );
                AddMode = false;
            }

            //Clear editrow
            grdRules.EditItemIndex = - 1;

            //Rebind the collection
            BindRules();
        }
    }
}