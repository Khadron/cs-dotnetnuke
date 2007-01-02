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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;

namespace DotNetNuke.UI.WebControls
{
    public class DNNLabelEdit : Label, IClientAPICallbackEventHandler
    {
        public delegate void DNNLabelEditEventHandler( object source, DNNLabelEditEventArgs e );

        private DNNLabelEditEventHandler UpdateLabelEvent;

        public event DNNLabelEditEventHandler UpdateLabel
        {
            add
            {
                UpdateLabelEvent = (DNNLabelEditEventHandler)Delegate.Combine( UpdateLabelEvent, value );
            }
            remove
            {
                UpdateLabelEvent = (DNNLabelEditEventHandler)Delegate.Remove( UpdateLabelEvent, value );
            }
        }



        /// <summary>
        /// Returns whether the LabelEdit will render DownLevel or not
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Commented
        ///		[Jon Henning]	3/9/2005	Requiring XML support on client for uplevel
        /// </history>
        [Browsable( false )]
        public bool IsDownLevel
        {
            get
            {
                if( EditEnabled == false || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.DHTML ) == false || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XML ) == false )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [DefaultValue( true ), PersistenceMode( PersistenceMode.Attribute )]
        public bool EditEnabled
        {
            get
            {
                if( Strings.Len( ViewState["EditEnabled"] ) > 0 )
                {
                    return Convert.ToBoolean( ViewState["EditEnabled"] );
                }
                else
                {
                    return true;
                }
            }
            set
            {
                ViewState["EditEnabled"] = value;
            }
        }

        /// <summary>
        /// Location of ClientAPI js files
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        public string ClientAPIScriptPath
        {
            get
            {
                return ClientAPI.ScriptPath;
            }
            set
            {
                // Commented out because it was breaking the build.. (SM 26 Oct)
                // uncommented out, don't understand how this could have broke build - JH 22 Feb 2005
                ClientAPI.ScriptPath = value;
            }
        }

        /// <summary>
        /// Location of dnn.controls.DNNLabelEdit.js file
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Since 1.1 this path will be the same as the ClientAPI path.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        public string LabelEditScriptPath
        {
            get
            {
                if( Strings.Len( ViewState["LabelEditScriptPath"] ) == 0 )
                {
                    return ClientAPIScriptPath;
                }
                else
                {
                    return ViewState["LabelEditScriptPath"].ToString();
                }
            }
            set
            {
                ViewState["LabelEditScriptPath"] = value;
            }
        }

        /// <summary>
        /// Location of system images (i.e. spacer.gif)
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        [Description( "Directory to find the images for the LabelEdit.  Need to have spacer.gif here!" ), DefaultValue( "images/" )]
        public string SystemImagesPath
        {
            get
            {
                if( Strings.Len( ViewState["SystemImagesPath"] ) == 0 )
                {
                    return "images/";
                }
                else
                {
                    return ViewState["SystemImagesPath"].ToString();
                }
            }
            set
            {
                ViewState["SystemImagesPath"] = value;
            }
        }

        [Bindable( true ), DefaultValue( "onclick" ), PersistenceMode( PersistenceMode.Attribute )]
        public string EventName
        {
            get
            {
                if( Strings.Len( ViewState["EventName"] ) == 0 )
                {
                    return "onclick";
                }
                else
                {
                    return ViewState["EventName"].ToString();
                }
            }
            set
            {
                ViewState["EventName"] = value;
            }
        }

        [DefaultValue( false ), PersistenceMode( PersistenceMode.Attribute )]
        public bool MultiLine
        {
            get
            {
                if( Strings.Len( ViewState["MultiLine"] ) == 0 )
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean( ViewState["MultiLine"] );
                }
            }
            set
            {
                ViewState["MultiLine"] = value;
            }
        }

        [DefaultValue( false ), PersistenceMode( PersistenceMode.Attribute )]
        public bool RichTextEnabled
        {
            get
            {
                if( Strings.Len( ViewState["RichTextEnabled"] ) == 0 )
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean( ViewState["RichTextEnabled"] );
                }
            }
            set
            {
                ViewState["RichTextEnabled"] = value;
            }
        }

        /// <summary>
        /// If callbacks are supported/enabled, this javascript function will be invoked
        /// with each status change of the xmlhttp request.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The Javascript transport does not raise any events.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Created
        /// </history>
        [Bindable( true ), PersistenceMode( PersistenceMode.Attribute )]
        public string CallbackStatusFunction
        {
            get
            {
                return ( Convert.ToString( ViewState["CallbackStatusFunction"] ) );
            }
            set
            {
                ViewState["CallbackStatusFunction"] = value;
            }
        }

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string LabelEditCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["LabelEditCssClass"] ) );
            }
            set
            {
                ViewState["LabelEditCssClass"] = value;
            }
        }

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string WorkCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["WorkCssClass"] ) );
            }
            set
            {
                ViewState["WorkCssClass"] = value;
            }
        }

        [DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string MouseOverCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["MouseOverCssClass"] ) );
            }
            set
            {
                ViewState["MouseOverCssClass"] = value;
            }
        }

        protected override void AddAttributesToRender( HtmlTextWriter writer )
        {
            base.AddAttributesToRender( writer );
            //writer.AddAttribute(HtmlTextWriterAttribute.Class, Me.CssClass)
            if( this.EditEnabled )
            {
                writer.AddAttribute( "sysimgpath", this.SystemImagesPath );
                if( this.LabelEditCssClass.Length > 0 )
                {
                    writer.AddAttribute( "cssEdit", this.LabelEditCssClass );
                }
                if( this.WorkCssClass.Length > 0 )
                {
                    writer.AddAttribute( "cssWork", this.WorkCssClass );
                }

                if( this.MouseOverCssClass.Length > 0 )
                {
                    writer.AddAttribute( "cssOver", this.MouseOverCssClass );
                }

                if( this.EventName != "onclick" )
                {
                    writer.AddAttribute( "eventName", this.EventName );
                }

                if( this.MultiLine )
                {
                    writer.AddAttribute( "multiline", "1" );
                }

                if( this.RichTextEnabled )
                {
                    writer.AddAttribute( "richtext", "1" );
                }

                if( ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XMLHTTP ) )
                {
                    writer.AddAttribute( "callback", ClientAPI.GetCallbackEventReference( this, "'[TEXT]'", "this.callBackSuccess", "this", "this.callBackFail", "this.callBackStatus" ) );
                }

                if( this.CallbackStatusFunction.Length > 0 )
                {
                    writer.AddAttribute( "callbackSF", this.CallbackStatusFunction );
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	10/10/2005	Created
        /// </history>
        protected override object SaveViewState()
        {
            object _baseState = base.SaveViewState();
            object[] _newState = new object[1];
            _newState[0] = _baseState;
            return _newState;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	10/10/2005	Created
        /// </history>
        protected override void LoadViewState( object state )
        {
            if( !( state == null ) )
            {
                object[] _newState = (object[])state;
                if( !( _newState[0] == null ) )
                {
                    base.LoadViewState( _newState[0] );
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	10/10/2005	Created
        /// </history>
        protected override void TrackViewState()
        {
            base.TrackViewState();
            //CType(Root, IStateManager).TrackViewState()
        }

        public virtual void RaisePostBackEvent( string eventArgument )
        {
            if( UpdateLabelEvent != null )
            {
                UpdateLabelEvent( this, new DNNLabelEditEventArgs( eventArgument ) );
            }
        }

        public string RaiseClientAPICallbackEvent( string eventArgument )
        {
            if( UpdateLabelEvent != null )
            {
                UpdateLabelEvent( this, new DNNLabelEditEventArgs( eventArgument ) );
            }
            return eventArgument;
        }

        /// <summary>
        /// Determines if client supports an uplevel rendering, and if so it registers
        /// the appropriate scripts.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        public void RegisterClientScript()
        {
            if( IsDownLevel == false && this.EditEnabled )
            {
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_dom );
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_xml );
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_dom_positioning );
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_xmlhttp );

                if( !ClientAPI.IsClientScriptBlockRegistered( this.Page, "dnn.controls.dnnlabeledit.js" ) )
                {
                    ClientAPI.RegisterClientScriptBlock( this.Page, "dnn.controls.dnnlabeledit.js", "<script src=\"" + LabelEditScriptPath + "dnn.controls.dnnlabeledit.js\"></script>" );
                }
                ClientAPI.RegisterStartUpScript( Page, this.ClientID + "_startup", "<script>dnn.controls.initLabelEdit(dnn.dom.getById('" + this.ClientID + "'));</script>" ); //wrong place
            }
        }

        protected void DNNLabelEdit_PreRender( object sender, EventArgs e )
        {
            RegisterClientScript();
        }
    }
}