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
using System.Web.UI;

namespace DotNetNuke.UI.Utilities
{
    /// <Summary>Control used to register post-back events</Summary>
    public class ClientAPIPostBackControl : Control, IPostBackEventHandler
    {
        private Hashtable m_oEventHandlers;

        public delegate void PostBackEvent( ClientAPIPostBackEventArgs Args );

        public ClientAPIPostBackControl( System.Web.UI.Page objPage, string strEventName, PostBackEvent objDelegate )
        {
            this.m_oEventHandlers = new Hashtable();
            string string1 = ClientAPI.GetPostBackClientEvent( objPage, ( (Control)this ), "" );
            this.AddEventHandler( strEventName, objDelegate );
        }

        public ClientAPIPostBackControl()
        {
            this.m_oEventHandlers = new Hashtable();
        }

        public PostBackEvent get_EventHandlers( string strEventName )
        {
            if( ! this.m_oEventHandlers.Contains( strEventName ) )
            {
                return null;
            }
            else
            {
                return ( (PostBackEvent)this.m_oEventHandlers[strEventName] );
            }
        }

        public void AddEventHandler( string strEventName, PostBackEvent objDelegate )
        {
            if( ! this.m_oEventHandlers.Contains( strEventName ) )
            {
                this.m_oEventHandlers.Add( strEventName, objDelegate );
                return;
            }
            this.m_oEventHandlers[strEventName] = ( (PostBackEvent)Delegate.Combine( ( (Delegate)( (PostBackEvent)this.m_oEventHandlers[strEventName] ) ), ( (Delegate)objDelegate ) ) );
        }

        /// <Summary>
        /// Function implementing IPostBackEventHandler which allows the ASP.NET page to invoke
        /// the control's events
        /// </Summary>
        public virtual void RaisePostBackEvent( string strEventArgument )
        {
            ClientAPIPostBackEventArgs clientAPIPostBackEventArgs1 = new ClientAPIPostBackEventArgs( strEventArgument );
            if( this.get_EventHandlers( clientAPIPostBackEventArgs1.EventName ) == null )
            {
                return;
            }
            this.get_EventHandlers( clientAPIPostBackEventArgs1.EventName )( clientAPIPostBackEventArgs1 );
        }
    }
}