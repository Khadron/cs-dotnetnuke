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