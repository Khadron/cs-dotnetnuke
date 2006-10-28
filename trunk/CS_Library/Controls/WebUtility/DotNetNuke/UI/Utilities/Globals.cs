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
            return Globals.FindControlRecursive( objParent, strControlName, "" );
        }

        public static Control FindControlRecursive( Control objParent, string strControlName, string strClientID )
        {
            Control control3 = objParent.FindControl( strControlName );
            if( control3 != null )
            {
                return control3;
            }
            foreach( Control control2 in objParent.Controls )
            {
                if( control2.HasControls() )
                {
                    control3 = Globals.FindControlRecursive( control2, strControlName, strClientID );
                }
                if( ( ( control3 != null ) && ( strClientID.Length > 0 ) ) && ( String.Compare( control3.ClientID, strClientID, false ) != 0 ) )
                {
                    control3 = null;
                }
                if( control3 != null )
                {
                    return control3;
                }
            }
            return control3;
        }

        public static string GetAttribute( Control objControl, string strAttr )
        {
            bool b1 = true;
            if( b1 == ( objControl is WebControl ) )
            {
                return ( (WebControl)objControl ).Attributes[strAttr];
            }
            if( b1 != ( objControl is HtmlControl ) )
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
            WebControl webControl1;
            string string1 = Globals.GetAttribute( objControl, strAttr );
            if( string1.Length > 0 )
            {
                strValue = ( string1 + strValue );
            }
            bool b1 = true;
            if( b1 == ( objControl is WebControl ) )
            {
                webControl1 = ( (WebControl)objControl );
                if( webControl1.Attributes[strAttr] == null )
                {
                    webControl1.Attributes.Add( strAttr, strValue );
                    return;
                }
                webControl1.Attributes[strAttr] = strValue;
                return;
            }
            if( b1 != ( objControl is HtmlControl ) )
            {
                return;
            }
            HtmlControl htmlControl1 = ( (HtmlControl)objControl );
            if( htmlControl1.Attributes[strAttr] == null )
            {
                htmlControl1.Attributes.Add( strAttr, strValue );
                return;
            }
            htmlControl1.Attributes[strAttr] = strValue;
        }
    }
}