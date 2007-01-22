#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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

using System.Globalization;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.Common.Utilities
{
    public class Calendar
    {
        /// <summary>
        /// Opens a popup Calendar
        /// </summary>
        /// <param name="Field">TextBox to return the date value</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static string InvokePopupCal( TextBox Field )
        {
            // Define character array to trim from language strings
            char[] TrimChars = new char[] {',', ' '};

            // Get culture array of month names and convert to string for
            // passing to the popup calendar
            string MonthNameString = "";
            foreach( string Month in DateTimeFormatInfo.CurrentInfo.MonthNames )
            {
                MonthNameString += Month + ",";
            }
            MonthNameString = MonthNameString.TrimEnd( TrimChars );

            // Get culture array of day names and convert to string for
            // passing to the popup calendar
            string DayNameString = "";
            foreach( string Day in DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames )
            {
                DayNameString += Day + ",";
            }
            DayNameString = DayNameString.TrimEnd( TrimChars );

            // Get the short date pattern for the culture
            string FormatString = DateTimeFormatInfo.CurrentInfo.ShortDatePattern.ToString();
            if( !ClientAPI.IsClientScriptBlockRegistered( Field.Page, "PopupCalendar.js" ) )
            {
                ClientAPI.RegisterClientScriptBlock( Field.Page, "PopupCalendar.js", "<script src=\"" + ClientAPI.ScriptPath + "PopupCalendar.js\"></script>" );
            }

            string strToday = ClientAPI.GetSafeJSString( Localization.GetString( "Today" ) );
            string strClose = ClientAPI.GetSafeJSString( Localization.GetString( "Close" ) );
            string strCalendar = ClientAPI.GetSafeJSString( Localization.GetString( "Calendar" ) );
            return "javascript:popupCal('Cal','" + Field.ClientID + "','" + FormatString + "','" + MonthNameString + "','" + DayNameString + "','" + strToday + "','" + strClose + "','" + strCalendar + "'," + DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek + ");";
        }
    }
}