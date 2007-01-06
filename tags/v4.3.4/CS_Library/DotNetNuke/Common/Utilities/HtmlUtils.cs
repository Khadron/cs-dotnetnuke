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
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Services.Upgrade;
using Microsoft.VisualBasic;

namespace DotNetNuke.Common.Utilities
{

    /// <Summary>
    /// HtmlUtils is a Utility class that provides Html Utility methods
    /// </Summary>
    public class HtmlUtils
    {
        /// <summary>
        /// Clean removes any HTML Tags, Entities (and optionally any punctuation) from
        /// a string
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The Html to clean</param>
        /// <param name="RemovePunctuation">A flag indicating whether to remove punctuation</param>
        /// <returns>The cleaned up string</returns>
        public static string Clean(string HTML, bool RemovePunctuation)
        {
            //First remove any HTML entities (&nbsp; &lt; etc)
            HTML = StripEntities(HTML, true);

            //Next remove any HTML Tags ("<....>")
            HTML = StripTags(HTML, true);

            //Finally remove any punctuation
            if (RemovePunctuation)
            {
                HTML = StripPunctuation(HTML, true);
            }

            return HTML;
        }

        /// <summary>
        /// Formats an Email address
        /// </summary>
        /// <param name="Email">The email address to format</param>
        /// <returns>The formatted email address</returns>
        public static string FormatEmail(string Email)
        {
            string returnValue;

            returnValue = "";

            if (Email.Length != 0)
            {
                if (Email.Trim() != "")
                {
                    if (Email.IndexOf("@") != -1)
                    {
                        returnValue = "<a href=\"mailto:" + Email + "\">" + Email + "</a>";
                    }
                    else
                    {
                        returnValue = Email;
                    }
                }
            }

            return Globals.CloakText(returnValue);
        }

        /// <summary>
        /// FormatText replaces <br/> tags by LineFeed characters
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <returns>The cleaned up string</returns>
        public static string FormatText(string HTML, bool RetainSpace)
        {
            //Match all variants of <br> tag (<br>, <BR>, <br/>, including embedded space
            string brMatch = "\\s*<\\s*[bB][rR]\\s*/\\s*>\\s*";

            //Set up Replacement String
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }

            //Replace Tags by replacement String and return mofified string
            return Regex.Replace(HTML, brMatch, "\n");
        }

        /// <summary>
        /// Format a domain name including link
        /// </summary>
        /// <param name="Website">The domain name to format</param>
        /// <returns>The formatted domain name</returns>
        public static string FormatWebsite(object Website)
        {
            string returnValue;

            returnValue = "";

            if (!Information.IsDBNull(Website))
            {
                if (Website.ToString().Trim() != "")
                {
                    if (Convert.ToBoolean(Strings.InStr(1, Website.ToString(), ".", 0)))
                    {
                        returnValue = "<a href=\"" + ((Convert.ToBoolean(Strings.InStr(1, Website.ToString(), "://", 0))) ? "" : "http://").ToString() + Website.ToString() + "\">" + Website.ToString() + "</a>";
                    }
                    else
                    {
                        returnValue = Website.ToString();
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Shorten returns the first (x) characters of a string
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="txt">The text to reduces</param>
        /// <param name="length">The max number of characters to return</param>
        /// <param name="suffix">An optional suffic to append to the shortened string</param>
        /// <returns>The shortened string</returns>
        public static string Shorten(string txt, int length, string suffix)
        {
            string results;
            if (txt.Length > length)
            {
                results = txt.Substring(0, length) + suffix;
            }
            else
            {
                results = txt;
            }
            return results;
        }

        /// <summary>
        /// StripEntities removes the HTML Entities from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the Entity by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripEntities(string HTML, bool RetainSpace)
        {
            //Set up Replacement String
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }

            //Replace Entities by replacement String and return mofified string
            return Regex.Replace(HTML, "&[^;]*;", RepString);
        }

        /// <summary>
        /// StripNonWord removes any Non-Word Character from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the Non-Word Character by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripNonWord(string HTML, bool RetainSpace)
        {
            //Set up Replacement String
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }

            //Replace Tags by replacement String and return mofified string
            if (HTML == null)
            {
                return HTML;
            }
            else
            {
                return Regex.Replace(HTML, "\\W*", RepString);
            }
        }

        /// <summary>
        /// StripPunctuation removes the Punctuation from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the Punctuation by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripPunctuation(string HTML, bool RetainSpace)
        {
            //Create Regular Expression objects
            string punctuationMatch = "[~!#\\$%\\^&*\\(\\)-+=\\{\\[\\}\\]\\|;:\\x22\'<,>\\.\\?\\\\\\t\\r\\v\\f\\n]";
            Regex afterRegEx = new Regex(punctuationMatch + "\\s");
            Regex beforeRegEx = new Regex("\\s" + punctuationMatch);

            //Define return string
            string retHTML = HTML + " "; //Make sure any punctuation at the end of the String is removed

            //Set up Replacement String
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }

            while (beforeRegEx.IsMatch(retHTML))
            {
                //Strip punctuation from beginning of word
                retHTML = beforeRegEx.Replace(retHTML, RepString);
            }

            while (afterRegEx.IsMatch(retHTML))
            {
                //Strip punctuation from end of word
                retHTML = afterRegEx.Replace(retHTML, RepString);
            }

            // Return modified string
            return retHTML;
        }

        /// <summary>
        /// StripTags removes the HTML Tags from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the Tag by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripTags(string HTML, bool RetainSpace)
        {
            //Set up Replacement String
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }

            //Replace Tags by replacement String and return mofified string
            return Regex.Replace(HTML, "<[^>]*>", RepString);
        }

        /// <summary>
        /// StripWhiteSpace removes the WhiteSpace from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the WhiteSpace by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripWhiteSpace(string HTML, bool RetainSpace)
        {
            //Set up Replacement String
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }

            //Replace Tags by replacement String and return mofified string
            return Regex.Replace(HTML, "\\s+", RepString);
        }

        /// <summary>
        /// WriteError outputs an Error Message during Install/Upgrade etc
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="response">The ASP.Net Response object</param>
        /// <param name="file">The filename where the Error Occurred</param>
        /// <param name="message">The error message</param>
        public static void WriteError(HttpResponse response, string file, string message)
        {
            response.Write("<h2>Error Details</h2>");
            response.Write("<table cellspacing=0 cellpadding=0 border=0>");
            response.Write("<tr><td><b>File</b></td><td><b>" + file + "</b></td></tr>");
            response.Write("<tr><td><b>Error</b>&nbsp;&nbsp;</td><td><b>" + message + "</b></td></tr>");
            response.Write("</table>");
            response.Write("<br><br>");
            response.Flush();
        }

        /// <summary>
        /// WriteFeedback outputs a Feedback Line during Install/Upgrade etc
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="response">The ASP.Net Response object</param>
        /// <param name="message">The feedback message</param>
        public static void WriteFeedback(HttpResponse response, int indent, string message)
        {
            bool showInstallationMessages = true;
            if (Config.GetSetting("ShowInstallationMessages") != null)
            {
                showInstallationMessages = bool.Parse(Convert.ToString(Config.GetSetting("ShowInstallationMessages")));
            }
            if (showInstallationMessages)
            {
                //Get the time of the feedback
                TimeSpan timeElapsed = Upgrade.RunTime;

                string strMessage = timeElapsed.ToString().Substring(0, timeElapsed.ToString().LastIndexOf(".") + 4) + " -";
                for (int i = 0; i <= indent; i++)
                {
                    strMessage += "&nbsp;";
                }
                strMessage += message;
                HttpContext.Current.Response.Write(strMessage);
                HttpContext.Current.Response.Flush();
            }
        }

        /// <summary>
        /// WriteFooter outputs the Footer during Install/Upgrade etc
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="response">The ASP.Net Response object</param>
        public static void WriteFooter(HttpResponse response)
        {
            response.Write("</body>");
            response.Write("</html>");
            response.Flush();
        }

        /// <summary>
        /// WriteHeader outputs the Header during Install/Upgrade etc
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="response">The ASP.Net Response object</param>
        /// <param name="mode">The mode Install/Upgrade etc</param>
        public static void WriteHeader(HttpResponse response, string mode)
        {
            //Set Response buffer to False
            response.Buffer = false;

            // create an install page if it does not exist already
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Install/Install.htm")))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Install/Install.template.htm")))
                {
                    File.Copy(HttpContext.Current.Server.MapPath("~/Install/Install.template.htm"), HttpContext.Current.Server.MapPath("~/Install/Install.htm"));
                }
            }

            // read install page and insert into response stream
            if (File.Exists(HttpContext.Current.Server.MapPath("~/Install/Install.htm")))
            {
                StreamReader objStreamReader;
                objStreamReader = File.OpenText(HttpContext.Current.Server.MapPath("~/Install/Install.htm"));
                string strHTML = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                response.Write(strHTML);
            }

            switch (mode)
            {
                case "install":

                    response.Write("<h1>Installing DotNetNuke</h1>");
                    break;
                case "upgrade":

                    response.Write("<h1>Upgrading DotNetNuke</h1>");
                    break;
                case "addPortal":

                    response.Write("<h1>Adding New Portal</h1>");
                    break;
                case "installResources":

                    response.Write("<h1>Installing Resources</h1>");
                    break;
                case "executeScripts":

                    response.Write("<h1>Executing Scripts</h1>");
                    break;
                case "none":

                    response.Write("<h1>Nothing To Install At This Time</h1>");
                    break;
                case "noDBVersion":

                    response.Write("<h1>New DotNetNuke Database</h1>");
                    break;
                case "error":

                    response.Write("<h1>Error Installing DotNetNuke</h1>");
                    break;
                default:

                    response.Write("<h1>" + mode + "</h1>");
                    break;
            }
            response.Flush();
        }
    }
}