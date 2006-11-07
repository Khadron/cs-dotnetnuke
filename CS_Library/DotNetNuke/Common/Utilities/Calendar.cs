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
        public static string InvokePopupCal(TextBox Field)
        {
            // Define character array to trim from language strings
            char[] TrimChars = new char[] { ',', ' ' };

            // Get culture array of month names and convert to string for
            // passing to the popup calendar
            string MonthNameString = "";
            string Month;
            foreach (string tempLoopVar_Month in DateTimeFormatInfo.CurrentInfo.MonthNames)
            {
                Month = tempLoopVar_Month;
                MonthNameString += Month + ",";
            }
            MonthNameString = MonthNameString.TrimEnd(TrimChars);

            // Get culture array of day names and convert to string for
            // passing to the popup calendar
            string DayNameString = "";
            string Day;
            foreach (string tempLoopVar_Day in DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames)
            {
                Day = tempLoopVar_Day;
                DayNameString += Day + ",";
            }
            DayNameString = DayNameString.TrimEnd(TrimChars);

            // Get the short date pattern for the culture
            string FormatString = DateTimeFormatInfo.CurrentInfo.ShortDatePattern.ToString();
            if (!ClientAPI.IsClientScriptBlockRegistered(Field.Page, "PopupCalendar.js"))
            {
                ClientAPI.RegisterClientScriptBlock(Field.Page, "PopupCalendar.js", "<script src=\"" + ClientAPI.ScriptPath + "PopupCalendar.js\"></script>");
            }

            return "javascript:popupCal(\'Cal\',\'" + Field.ClientID + "\',\'" + FormatString + "\',\'" + MonthNameString + "\',\'" + DayNameString + "\',\'" + Localization.GetString("Today") + "\',\'" + Localization.GetString("Close") + "\',\'" + Localization.GetString("Calendar") + "\'," + DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek + ");";
        }
    }
}