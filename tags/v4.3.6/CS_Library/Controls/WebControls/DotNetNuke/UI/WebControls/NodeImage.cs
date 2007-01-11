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

namespace DotNetNuke.UI.WebControls
{
    public class NodeImage : IStateManager
    {
        private bool _marked = false;
        private StateBag _state;

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public NodeImage()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="NewImageUrl"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public NodeImage( string NewImageUrl )
        {
            if( ImageUrl == null )
            {
                throw ( new ArgumentNullException() );
            }
            this.TrackViewState();
            ImageUrl = NewImageUrl;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public bool IsTrackingViewState
        {
            get
            {
                return _marked;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public void TrackViewState()
        {
            _marked = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public object SaveViewState()
        {
            // save _state state
            object _stateState = null;
            if( !( _state == null ) )
            {
                _stateState = ( (IStateManager)_state ).SaveViewState();
            }
            return _stateState;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public void LoadViewState( object state )
        {
            if( !( state == null ) )
            {
                ( (IStateManager)ViewState ).LoadViewState( state );
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        ///     [cnurse]    11/3/2004   Fixed to work under Option Strict On
        /// </history>
        public string ImageUrl
        {
            get
            {
                string _imageUrl = string.Empty;
                if( ( ViewState["ImageUrl"] == null ) == false )
                {
                    _imageUrl = Convert.ToString( ViewState["ImageUrl"] );
                }
                return _imageUrl;
            }
            set
            {
                ViewState["ImageUrl"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        protected StateBag ViewState
        {
            get
            {
                if( _state == null )
                {
                    _state = new StateBag( true );
                    if( IsTrackingViewState )
                    {
                        ( (IStateManager)_state ).TrackViewState();
                    }
                }
                return _state;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        internal void SetDirty()
        {
            if( _state != null )
            {
                string key;
                foreach( string tempLoopVar_key in _state.Keys )
                {
                    key = tempLoopVar_key;
                    _state.SetItemDirty( key, true );
                }
            }
        }
    }
}