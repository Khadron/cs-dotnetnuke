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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace DotNetNuke.UI.Utilities
{
    public class Globals
    {
        /// <Summary>
        /// Searches control hierarchy from top down to find a control matching the passed in name
        /// </Summary>
        /// <Param name="objParent">Root control to begin searching</Param>
        /// <Param name="strControlName">Name of control to look for</Param>
        public static Control FindControlRecursive( Control objParent, string strControlName )
        {
            return FindControlRecursive( objParent, strControlName, "" );
        }

        public static Control FindControlRecursive( Control objParent, string strControlName, string strClientID )
        {
            Control control = objParent.FindControl( strControlName );
            if( control != null )
            {
                return control;
            }
            foreach( Control parent in objParent.Controls )
            {
                if( parent.HasControls() )
                {
                    control = FindControlRecursive( parent, strControlName, strClientID );
                }
                if( ( ( control != null ) && ( strClientID.Length > 0 ) ) && ( String.Compare( control.ClientID, strClientID, false ) != 0 ) )
                {
                    control = null;
                }
                if( control != null )
                {
                    return control;
                }
            }
            return control;
        }

        public static string GetAttribute( Control objControl, string strAttr )
        {            
            if(  objControl is WebControl ) 
            {
                return ( (WebControl)objControl ).Attributes[strAttr];
            }
            if( !( objControl is HtmlControl ) )
            {
                return null;
            }
            else
            {
                return ( (HtmlControl)objControl ).Attributes[strAttr];
            }
        }

        public static void SetAttribute( Control objControl, string strAttr, string strValue )
        {
            string s = GetAttribute( objControl, strAttr );
            if( s != null ) if( s.Length > 0 )
            {
                strValue = ( s + strValue );
            }            
            if(  objControl is WebControl ) 
            {
                WebControl webControl = ( (WebControl)objControl );
                if( webControl.Attributes[strAttr] == null )
                {
                    webControl.Attributes.Add( strAttr, strValue );
                    return;
                }
                webControl.Attributes[strAttr] = strValue;
                return;
            }
            if( ! ( objControl is HtmlControl ) )
            {
                return;
            }
            HtmlControl htmlControl = ( (HtmlControl)objControl );
            if( htmlControl.Attributes[strAttr] == null )
            {
                htmlControl.Attributes.Add( strAttr, strValue );
                return;
            }
            htmlControl.Attributes[strAttr] = strValue;
        }
    }
}