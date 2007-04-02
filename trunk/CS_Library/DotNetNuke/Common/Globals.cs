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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Url.FriendlyUrl;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;
using DataCache = DotNetNuke.Common.Utilities.DataCache;
using TabInfo = DotNetNuke.Entities.Tabs.TabInfo;

namespace DotNetNuke.Common
{
    /// <Summary>
    /// This module contains global utility functions, constants, and enumerations.
    /// </Summary>
    public sealed class Globals
    {
        public enum PerformanceSettings
        {
            //The values of the enum are used to calculate
            //cache settings throughout the portal.
            //Calculating based on these numbers keeps
            //the scaling linear for all caching.
            NoCaching = 0,
            LightCaching = 1,
            ModerateCaching = 3,
            HeavyCaching = 6
        }

        public enum PortalRegistrationType
        {
            NoRegistration = 0,
            PrivateRegistration = 1,
            PublicRegistration = 2,
            VerifiedRegistration = 3
        }

        public enum UpgradeStatus
        {
            Upgrade,
            Install,
            None,
            Error
        }

        public const string glbAboutPage = "about.htm";
        public const string glbAppCompany = "DotNetNuke Corporation";
        public const string glbAppDescription = "Open Source Web Application Framework";
        public const string glbAppTitle = "DotNetNuke";
        public const string glbAppUrl = "http://www.dotnetnuke.com";
        public const string glbUpgradeUrl = "http://update.dotnetnuke.com";
        // HACK : Bug fix Id: 2
        public const string glbAppVersion = "04.04.00";
        public const string glbConfigFolder = "\\Config\\";
        public const string glbDefaultAdminContainer = "Image Header - Color Background.ascx";
        public const string glbDefaultAdminSkin = "Horizontal Menu - Fixed Width.ascx";
        public const string glbDefaultContainer = "Image Header - Color Background.ascx";
        public const string glbDefaultContainerFolder = "/DNN-Blue/";
        public const string glbDefaultControlPanel = "Admin/ControlPanel/IconBar.ascx";

        public const string glbDefaultPage = "Default.aspx";
        public const string glbDefaultPane = "ContentPane";
        public const string glbDefaultSkin = "Horizontal Menu - Fixed Width.ascx";
        public const string glbDefaultSkinFolder = "/DNN-Blue/";
        public const string glbHelpUrl = "http://www.dotnetnuke.com/default.aspx?tabid=787";
        public const string glbHostSkinFolder = "_default";
        public const string glbImageFileTypes = "jpg,jpeg,jpe,gif,bmp,png,swf";
        public const string glbLegalCopyright = "DotNetNuke® is copyright 2002-YYYY by DotNetNuke Corporation";

        public const string glbProtectedExtension = ".resources";

        public const string glbRoleAllUsers = "-1";

        public const string glbRoleAllUsersName = "All Users";
        public const string glbRoleSuperUser = "-2";
        public const string glbRoleSuperUserName = "Superuser";
        public const string glbRoleUnauthUser = "-3";
        public const string glbRoleUnauthUserName = "Unauthenticated Users";

        public const int glbSuperUserAppName = -1;
        public const string glbTrademark = "DotNetNuke,DNN";
        private static string _ApplicationMapPath;

        // global constants for the life of the application ( set in Application_Start )
        private static string _ApplicationPath;
        private static string _AssemblyPath;
        private static string _HostMapPath;
        private static string _HostPath;
        private static Hashtable _HostSettings;
        private static PerformanceSettings _PerformanceSetting;
        private static string _ServerName;
        private static string strWebFarmEnabled;

        public static string ApplicationMapPath
        {
            get
            {
                return _ApplicationMapPath;
            }
            set
            {
                _ApplicationMapPath = value;
            }
        }

        public static string ApplicationPath
        {
            get
            {
                return _ApplicationPath;
            }
            set
            {
                _ApplicationPath = value;
            }
        }

        public static string AssemblyPath
        {
            get
            {
                return _AssemblyPath;
            }
            set
            {
                _AssemblyPath = value;
            }
        }

        public static string HostMapPath
        {
            get
            {
                return _HostMapPath;
            }
            set
            {
                _HostMapPath = value;
            }
        }

        public static string HostPath
        {
            get
            {
                return _HostPath;
            }
            set
            {
                _HostPath = value;
            }
        }

        public static Hashtable HostSettings
        {
            get
            {
                return Entities.Host.HostSettings.GetHostSettings();
            }
        }

        public static PerformanceSettings PerformanceSetting
        {
            get
            {
                return ((PerformanceSettings)Convert.ToInt32((HostSettings["PerformanceSetting"] == null) ? 3 : (HostSettings["PerformanceSetting"])));
            }
        }

        public static string ServerName
        {
            get
            {
                return _ServerName;
            }
            set
            {
                _ServerName = value;
            }
        }

        public static bool WebFarmEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(strWebFarmEnabled))
                {
                    if (Config.GetSetting("EnableWebFarmSupport") == null)
                    {
                        strWebFarmEnabled = "false";
                    }
                    else
                    {
                        strWebFarmEnabled = Config.GetSetting("EnableWebFarmSupport").ToLower();
                    }
                }
                return bool.Parse(strWebFarmEnabled);
            }
        }



        public static string AccessDeniedURL()
        {
            return AccessDeniedURL("");
        }

        public static string AccessDeniedURL(string Message)
        {
            string strURL;

            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            if (HttpContext.Current.Request.IsAuthenticated)
            {
                if (Message == "")
                {
                    // redirect to access denied page
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Access Denied");
                }
                else
                {
                    // redirect to access denied page with custom message
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Access Denied", "message=" + HttpUtility.UrlEncode(Message));
                }
            }
            else
            {
                if (_portalSettings.LoginTabId != -1)
                {
                    // redirect to portal login page specified
                    strURL = NavigateURL(_portalSettings.LoginTabId, "", "returnurl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl));
                }
                else
                {
                    // redirect to current page
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login", "returnurl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl));
                }
            }

            return strURL;
        }

        /// <summary>
        /// Adds HTTP to URL if no other protocol specified
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="strURL">The url</param>
        /// <returns>The formatted url</returns>
        public static string AddHTTP(string strURL)
        {
            if (!String.IsNullOrEmpty(strURL))
            {
                if (Strings.InStr(1, strURL, "mailto:", 0) == 0 && Strings.InStr(1, strURL, "://", 0) == 0 && Strings.InStr(1, strURL, "~", 0) == 0 && Strings.InStr(1, strURL, "\\\\", 0) == 0)
                {
                    if (HttpContext.Current.Request.IsSecureConnection)
                    {
                        strURL = "https://" + strURL;
                    }
                    else
                    {
                        strURL = "http://" + strURL;
                    }
                }
            }
            return strURL;
        }

        /// <summary>
        /// Generates the Application root url (including the tab/page)
        /// </summary>
        /// <remarks>
        /// This overload assumes the current page
        /// </remarks>
        /// <returns>The formatted root url</returns>
        public static string ApplicationURL()
        {
            // Obtain PortalSettings from Current Context
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            if (_portalSettings != null)
            {
                return (ApplicationURL(_portalSettings.ActiveTab.TabID));
            }
            else
            {
                return (ApplicationURL(-1));
            }
        }

        /// <summary>
        /// Generates the Application root url (including the tab/page)
        /// </summary>
        /// <remarks>
        /// This overload takes the tabid (page id) as a parameter
        /// </remarks>
        /// <param name="TabID">The id of the tab/page</param>
        /// <returns>The formatted root url</returns>
        public static string ApplicationURL(int TabID)
        {
            string strURL = "~/" + glbDefaultPage;

            if (TabID != -1)
            {
                strURL += "?tabid=" + TabID;
            }

            return strURL;
        }

        public DataSet BuildCrossTabDataSet(string DataSetName, IDataReader result, string FixedColumns, string VariableColumns, string KeyColumn, string FieldColumn, string FieldTypeColumn, string StringValueColumn, string NumericValueColumn)
        {
            return BuildCrossTabDataSet(DataSetName, result, FixedColumns, VariableColumns, KeyColumn, FieldColumn, FieldTypeColumn, StringValueColumn, NumericValueColumn, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// converts a data reader with serialized fields into a typed data set
        /// </summary>
        /// <param name="DataSetName">Name of the dataset to be created</param>
        /// <param name="result">Data reader that contains all field values serialized</param>
        /// <param name="FixedColumns">List of fixed columns, delimited by commas. Columns must be contained in DataReader</param>
        /// <param name="VariableColumns">List of variable columns, delimited by commas. Columns must be contained in DataReader</param>
        /// <param name="KeyColumn">Name of the column, that contains the row ID. Column must be contained in DataReader</param>
        /// <param name="FieldColumn">Name of the column, that contains the field name. Column must be contained in DataReader</param>
        /// <param name="FieldTypeColumn">Name of the column, that contains the field type name. Column must be contained in DataReader</param>
        /// <param name="StringValueColumn">Name of the column, that contains the field value, if stored as string. Column must be contained in DataReader</param>
        /// <param name="NumericValueColumn">Name of the column, that contains the field value, if stored as number. Column must be contained in DataReader</param>
        /// <param name="Culture">culture of the field values in data reader's string value column</param>
        /// <returns>The generated DataSet</returns>
        /// <history>
        /// 	[sleupold]     08/24/2006	Created temporary clone of core function and added support for culture based parsing of numeric values
        /// </history>
        /// -----------------------------------------------------------------------------
        public DataSet BuildCrossTabDataSet(string DataSetName, IDataReader result, string FixedColumns, string VariableColumns, string KeyColumn, string FieldColumn, string FieldTypeColumn, string StringValueColumn, string NumericValueColumn, System.Globalization.CultureInfo Culture)
        {
            string[] arrFixedColumns;
            string[] arrVariableColumns = null;
            string[] arrField;
            int intColumn;
            int intKeyColumn;

            // create dataset
            DataSet crosstab = new DataSet(DataSetName);
            crosstab.Namespace = "NetFrameWork";

            // create table
            DataTable tab = new DataTable(DataSetName);

            // split fixed columns
            arrFixedColumns = FixedColumns.Split(",".ToCharArray());

            // add fixed columns to table
            for (intColumn = arrFixedColumns.GetLowerBound(0); intColumn <= arrFixedColumns.GetUpperBound(0); intColumn++)
            {
                arrField = arrFixedColumns[intColumn].Split("|".ToCharArray());
                DataColumn col = new DataColumn(arrField[0], Type.GetType("System." + arrField[1]));
                tab.Columns.Add(col);
            }

            // split variable columns
            if (VariableColumns != "")
            {
                arrVariableColumns = VariableColumns.Split(",".ToCharArray());

                // add varible columns to table
                for (intColumn = arrVariableColumns.GetLowerBound(0); intColumn <= arrVariableColumns.GetUpperBound(0); intColumn++)
                {
                    arrField = arrVariableColumns[intColumn].Split("|".ToCharArray());
                    DataColumn col = new DataColumn(arrField[0], Type.GetType("System." + arrField[1]));
                    col.AllowDBNull = true;
                    tab.Columns.Add(col);
                }
            }

            // add table to dataset
            crosstab.Tables.Add(tab);

            // add rows to table
            intKeyColumn = -1;
            DataRow row = null;
            while (result.Read())
            {
                // loop using KeyColumn as control break
                if (Convert.ToInt32(result[KeyColumn]) != intKeyColumn)
                {
                    // add row
                    if (intKeyColumn != -1)
                    {
                        tab.Rows.Add(row);
                    }

                    // create new row
                    row = tab.NewRow();

                    // assign fixed column values
                    for (intColumn = arrFixedColumns.GetLowerBound(0); intColumn <= arrFixedColumns.GetUpperBound(0); intColumn++)
                    {
                        arrField = arrFixedColumns[intColumn].Split("|".ToCharArray());
                        row[arrField[0]] = result[arrField[0]];
                    }

                    // initialize variable column values
                    if (VariableColumns != "")
                    {
                        if (arrVariableColumns != null)
                        {
                            for (intColumn = arrVariableColumns.GetLowerBound(0); intColumn <= arrVariableColumns.GetUpperBound(0); intColumn++)
                            {
                                arrField = arrVariableColumns[intColumn].Split("|".ToCharArray());
                                switch (arrField[1])
                                {
                                    case "Decimal":
                                        row[arrField[0]] = 0;
                                        break;
                                    case "String":
                                        row[arrField[0]] = "";
                                        break;
                                }
                            }
                        }
                    }

                    intKeyColumn = Convert.ToInt32(result[KeyColumn]);
                }

                // assign pivot column value
                string FieldType;
                if (FieldTypeColumn != "")
                {
                    FieldType = result[FieldTypeColumn].ToString();
                }
                else
                {
                    FieldType = "String";
                }
                if (row != null)
                {
                    switch (FieldType)
                    {
                        case "Decimal": // decimal
                            row[Convert.ToInt32(result[FieldColumn])] = result[NumericValueColumn];
                            break;
                        case "String": // string
                            if (Culture == CultureInfo.CurrentCulture)
                            {
                                row[result[FieldColumn].ToString()] = result[StringValueColumn];
                            }
                            else
                            {
                                switch (tab.Columns[result[FieldColumn].ToString()].DataType.ToString())
                                {
                                    case "System.Decimal":
                                    case "System.Currency":
                                        row[result[FieldColumn].ToString()] = decimal.Parse(result[StringValueColumn].ToString(), Culture);
                                        break;
                                    case "System.Int32":
                                        row[result[FieldColumn].ToString()] = Int32.Parse(result[StringValueColumn].ToString(), Culture);
                                        break;
                                    default:
                                        row[result[FieldColumn].ToString()] = result[StringValueColumn];
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            result.Close();

            // add row
            if (intKeyColumn != -1)
            {
                tab.Rows.Add(row);
            }

            // finalize dataset
            crosstab.AcceptChanges();

            // return the dataset
            return crosstab;

        }

        public static string CleanFileName(string FileName)
        {
            return CleanFileName(FileName, "", "");
        }

        public static string CleanFileName(string FileName, string BadChars)
        {
            return CleanFileName(FileName, BadChars, "");
        }

        public static string CleanFileName(string FileName, string BadChars, string ReplaceChar)
        {
            string strFileName = FileName;

            if (BadChars == "")
            {
                BadChars = ":/\\?*|" + '\u0022' + '\u0027' + '\t';
            }

            if (ReplaceChar == "")
            {
                ReplaceChar = "_";
            }

            int intCounter;

            for (intCounter = 0; intCounter <= BadChars.Length - 1; intCounter++)
            {
                strFileName = strFileName.Replace(BadChars.Substring(intCounter, 1), ReplaceChar);
            }

            return strFileName;
        }

        // obfuscate sensitive data to prevent collection by robots and spiders and crawlers
        public static string CloakText(string PersonalInfo)
        {
            if (PersonalInfo != null)
            {
                StringBuilder sb = new StringBuilder();

                // convert to ASCII character codes
                sb.Remove(0, sb.Length);
                int StringLength = PersonalInfo.Length - 1;
                for (int i = 0; i <= StringLength; i++)
                {
                    sb.Append(Strings.Asc(PersonalInfo.Substring(i, 1)).ToString());
                    if (i < StringLength)
                    {
                        sb.Append(",");
                    }
                }

                // build script block
                StringBuilder sbScript = new StringBuilder();

                sbScript.Append("\r\n" + "<script language=\"javascript\">" + "\r\n");
                sbScript.Append("<!-- " + "\r\n");
                sbScript.Append("   document.write(String.fromCharCode(" + sb + "))" + "\r\n");
                sbScript.Append("// -->" + "\r\n");
                sbScript.Append("</script>" + "\r\n");

                return sbScript.ToString();
            }
            else
            {
                return Null.NullString;
            }
        }

        // convert datareader to dataset
        public static DataSet ConvertDataReaderToDataSet(IDataReader reader)
        {
            // add datatable to dataset
            DataSet objDataSet = new DataSet();
            objDataSet.Tables.Add(ConvertDataReaderToDataTable(reader));

            return objDataSet;
        }

        // convert datareader to dataset
        public static DataTable ConvertDataReaderToDataTable(IDataReader reader)
        {
            // create datatable from datareader
            DataTable objDataTable = new DataTable();
            int intFieldCount = reader.FieldCount;
            int intCounter;
            for (intCounter = 0; intCounter <= intFieldCount - 1; intCounter++)
            {
                objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter));
            }

            // populate datatable
            objDataTable.BeginLoadData();
            object[] objValues = new object[intFieldCount - 1 + 1];
            while (reader.Read())
            {
                reader.GetValues(objValues);
                objDataTable.LoadDataRow(objValues, true);
            }
            reader.Close();
            objDataTable.EndLoadData();

            return objDataTable;
        }

        public static string CreateValidID(string strID)
        {
            string strBadCharacters = " ./-\\";

            int intIndex;
            for (intIndex = 0; intIndex <= strBadCharacters.Length - 1; intIndex++)
            {
                strID = strID.Replace(strBadCharacters.Substring(intIndex, 1), "_");
            }

            return strID;
        }

        public static string DateToString(DateTime DateValue)
        {
            try
            {
                if (!Null.IsNull(DateValue))
                {
                    return DateValue.ToString("s");
                }
                else
                {
                    return Null.NullString;
                }
            }
            catch (Exception)
            {
                return Null.NullString;
            }
        }

        /// <summary>
        /// DeserializeHashTableBase64 deserializes a Hashtable using Binary Formatting
        /// </summary>
        /// <remarks>
        /// While this method of serializing is no longer supported (due to Medium Trust
        /// issue, it is still required for upgrade purposes.
        /// </remarks>
        /// <param name="Source">The String Source to deserialize</param>
        /// <returns>The deserialized Hashtable</returns>
        public static Hashtable DeserializeHashTableBase64(string Source)
        {
            Hashtable objHashTable;
            if (!String.IsNullOrEmpty(Source))
            {
                byte[] bits = Convert.FromBase64String(Source);
                MemoryStream mem = new MemoryStream(bits);
                BinaryFormatter bin = new BinaryFormatter();
                try
                {
                    objHashTable = (Hashtable)bin.Deserialize(mem);
                }
                catch (Exception)
                {
                    objHashTable = new Hashtable();
                }
                mem.Close();
            }
            else
            {
                objHashTable = new Hashtable();
            }
            return objHashTable;
        }

        /// <summary>
        /// DeserializeHashTableXml deserializes a Hashtable using Xml Serialization
        /// </summary>
        /// <remarks>
        /// This is the preferred method of serialization under Medium Trust
        /// </remarks>
        /// <param name="Source">The String Source to deserialize</param>
        /// <returns>The deserialized Hashtable</returns>
        public static Hashtable DeserializeHashTableXml(string Source)
        {
            Hashtable objHashTable;
            if (!String.IsNullOrEmpty(Source))
            {
                objHashTable = new Hashtable();

                XmlDocument xmlProfile = new XmlDocument();
                xmlProfile.LoadXml(Source);

                foreach (XmlElement xmlItem in xmlProfile.SelectNodes("profile/item"))
                {
                    string key = xmlItem.GetAttribute("key");
                    string typeName = xmlItem.GetAttribute("type");

                    //Create the XmlSerializer
                    XmlSerializer xser = new XmlSerializer(Type.GetType(typeName));

                    //A reader is needed to read the XML document.
                    XmlTextReader reader = new XmlTextReader(new StringReader(xmlItem.InnerXml));

                    // Use the Deserialize method to restore the object's state, and store it
                    // in the Hashtable
                    objHashTable.Add(key, xser.Deserialize(reader));
                }
            }
            else
            {
                objHashTable = new Hashtable();
            }
            return objHashTable;
        }

        public static string EncodeReservedCharacters(string QueryString)
        {
            QueryString = QueryString.Replace("$", "%24");
            QueryString = QueryString.Replace("&", "%26");
            QueryString = QueryString.Replace("+", "%2B");
            QueryString = QueryString.Replace(",", "%2C");
            QueryString = QueryString.Replace("/", "%2F");
            QueryString = QueryString.Replace(":", "%3A");
            QueryString = QueryString.Replace(";", "%3B");
            QueryString = QueryString.Replace("=", "%3D");
            QueryString = QueryString.Replace("?", "%3F");
            QueryString = QueryString.Replace("@", "%40");

            return QueryString;
        }

        // uses recursion to search the control hierarchy for a specific control based on controlname
        public static Control FindControlRecursive(Control objControl, string strControlName)
        {
            if (objControl.Parent == null)
            {
                return null;
            }
            else
            {
                if (objControl.Parent.FindControl(strControlName) != null)
                {
                    return objControl.Parent.FindControl(strControlName);
                }
                else
                {
                    return FindControlRecursive(objControl.Parent, strControlName);
                }
            }
        }

        /// <summary>
        /// Searches control hierarchy from top down to find a control matching the passed in name
        /// </summary>
        /// <param name="objParent">Root control to begin searching</param>
        /// <param name="strControlName">Name of control to look for</param>
        /// <returns></returns>
        /// <remarks>
        /// This differs from FindControlRecursive in that it looks down the control hierarchy, whereas, the
        /// FindControlRecursive starts at the passed in control and walks the tree up.  Therefore, this function is
        /// more a expensive task.
        /// </remarks>
        public static Control FindControlRecursiveDown(Control objParent, string strControlName)
        {
            Control objCtl;
            objCtl = objParent.FindControl(strControlName);
            if (objCtl == null)
            {
                foreach (Control tempLoopVar_objChild in objParent.Controls)
                {
                    Control objChild = tempLoopVar_objChild;
                    objCtl = FindControlRecursiveDown(objChild, strControlName);
                    if (objCtl != null)
                    {
                        goto endOfForLoop;
                    }
                }
            endOfForLoop:
                1.GetHashCode(); //nop
            }
            return objCtl;
        }

        // format an address on a single line ( ie. Unit, Street, City, Region, Country, PostalCode )
        public static string FormatAddress(object Unit, object Street, object City, object Region, object Country, object PostalCode)
        {
            string returnValue;

            string strAddress = "";

            if (Unit != null)
            {
                if (Unit.ToString().Trim() != "")
                {
                    strAddress += ", " + Unit;
                }
            }
            if (Street != null)
            {
                if (Street.ToString().Trim() != "")
                {
                    strAddress += ", " + Street;
                }
            }
            if (City != null)
            {
                if (City.ToString().Trim() != "")
                {
                    strAddress += ", " + City;
                }
            }
            if (Region != null)
            {
                if (Region.ToString().Trim() != "")
                {
                    strAddress += ", " + Region;
                }
            }
            if (Country != null)
            {
                if (Country.ToString().Trim() != "")
                {
                    strAddress += ", " + Country;
                }
            }
            if (PostalCode != null)
            {
                if (PostalCode.ToString().Trim() != "")
                {
                    strAddress += ", " + PostalCode;
                }
            }
            if (strAddress.Trim() != "")
            {
                strAddress = strAddress.Substring(2);
            }

            returnValue = strAddress;

            return returnValue;
        }

        [Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.HtmlUtils.FormatEmail")]
        public static string FormatEmail(string Email)
        {
            return HtmlUtils.FormatEmail(Email);
        }

        public static string FormatHelpUrl(string HelpUrl, PortalSettings objPortalSettings, string Name)
        {
            return FormatHelpUrl(HelpUrl, objPortalSettings, Name, "");
        }

        public static string FormatHelpUrl(string HelpUrl, PortalSettings objPortalSettings, string Name, string Version)
        {
            string strURL = HelpUrl;

            if (strURL.IndexOf("?") != -1)
            {
                strURL += "&helpculture=";
            }
            else
            {
                strURL += "?helpculture=";
            }

            if (Thread.CurrentThread.CurrentCulture.ToString().ToLower() != "")
            {
                strURL += Thread.CurrentThread.CurrentCulture.ToString().ToLower();
            }
            else
            {
                strURL += objPortalSettings.DefaultLanguage.ToLower();
            }

            if (!String.IsNullOrEmpty(Name))
            {
                strURL += "&helpmodule=" + HttpUtility.UrlEncode(Name);
            }

            if (!String.IsNullOrEmpty(Version))
            {
                strURL += "&helpversion=" + HttpUtility.UrlEncode(Version);
            }

            return AddHTTP(strURL);
        }

        [Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.HtmlUtils.FormatWebsite")]
        public static string FormatWebsite(object Website)
        {
            return HtmlUtils.FormatWebsite(Website);
        }

        /// <summary>
        /// Generates the correctly formatted friendly url.
        /// </summary>
        /// <remarks>
        /// Assumes Default.aspx, and that portalsettings are saved to Context
        /// </remarks>
        /// <param name="tab">The current tab</param>
        /// <param name="path">The path to format.</param>
        /// <returns>The formatted (friendly) url</returns>
        public static string FriendlyUrl(TabInfo tab, string path)
        {
            return FriendlyUrl(tab, path, glbDefaultPage);
        }

        /// <summary>
        /// Generates the correctly formatted friendly url
        /// </summary>
        /// <remarks>
        /// This overload includes an optional page to include in the url.
        /// </remarks>
        /// <param name="tab">The current tab</param>
        /// <param name="path">The path to format.</param>
        /// <param name="pageName">The page to include in the url.</param>
        /// <returns>The formatted (friendly) url</returns>
        public static string FriendlyUrl(TabInfo tab, string path, string pageName)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            return FriendlyUrl(tab, path, pageName, _portalSettings);
        }

        /// <summary>
        /// Generates the correctly formatted friendly url
        /// </summary>
        /// <remarks>
        /// This overload includes the portal settings for the site
        /// </remarks>
        /// <param name="tab">The current tab</param>
        /// <param name="path">The path to format.</param>
        /// <param name="settings">The portal Settings</param>
        /// <returns>The formatted (friendly) url</returns>
        public static string FriendlyUrl(TabInfo tab, string path, PortalSettings settings)
        {
            return FriendlyUrl(tab, path, glbDefaultPage, settings);
        }

        /// <summary>
        /// Generates the correctly formatted friendly url
        /// </summary>
        /// <remarks>
        /// This overload includes an optional page to include in the url, and the portal
        /// settings for the site
        /// </remarks>
        /// <param name="tab">The current tab</param>
        /// <param name="path">The path to format.</param>
        /// <param name="pageName">The page to include in the url.</param>
        /// <param name="settings">The portal Settings</param>
        /// <returns>The formatted (friendly) url</returns>
        public static string FriendlyUrl(TabInfo tab, string path, string pageName, PortalSettings settings)
        {
            return FriendlyUrlProvider.Instance().FriendlyUrl(tab, path, pageName, settings);
        }

        /// <summary>
        /// Generates the correctly formatted friendly url
        /// </summary>
        /// <remarks>
        /// This overload includes an optional page to include in the url, and the portal
        /// settings for the site
        /// </remarks>
        /// <param name="tab">The current tab</param>
        /// <param name="path">The path to format.</param>
        /// <param name="pageName">The page to include in the url.</param>
        /// <param name="portalAlias">The portal Alias for the site</param>
        /// <returns>The formatted (friendly) url</returns>
        public static string FriendlyUrl(TabInfo tab, string path, string pageName, string portalAlias)
        {
            return FriendlyUrlProvider.Instance().FriendlyUrl(tab, path, pageName, portalAlias);
        }

        /// <summary>
        /// GenerateTabPath generates the TabPath used in Friendly URLS
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ParentId">The Id of the Parent Tab</param>
        /// <param name="TabName">The Name of the current Tab</param>
        /// <returns>The TabPath</returns>
        public static string GenerateTabPath(int ParentId, string TabName)
        {
            string strTabPath = "";
            TabController objTabs = new TabController();
            TabInfo objTab;
            objTab = objTabs.GetTab(ParentId, Null.NullInteger, false);
            while (objTab != null)
            {
                string strTabName = HtmlUtils.StripNonWord(objTab.TabName, false);
                strTabPath = "//" + strTabName + strTabPath;
                if (Null.IsNull(objTab.ParentId))
                {
                    objTab = null;
                }
                else
                {
                    objTab = objTabs.GetTab(objTab.ParentId, objTab.PortalID, false);
                }
            }

            strTabPath = strTabPath + "//" + HtmlUtils.StripNonWord(TabName, false);
            return strTabPath;
        }

        // returns the absolute server path to the root ( ie. D:\Inetpub\wwwroot\directory\ )
        public static string GetAbsoluteServerPath(HttpRequest Request)
        {
            string returnValue;
            string strServerPath;

            strServerPath = Request.MapPath(Request.ApplicationPath);
            if (!strServerPath.EndsWith("\\"))
            {
                strServerPath += "\\";
            }

            returnValue = strServerPath;
            return returnValue;
        }

        /// <summary>
        /// Gets the ApplicationName for the MemberRole API.
        /// </summary>
        /// <remarks>
        /// This overload is used to get the current ApplcationName.  The Application
        /// Name is in the form Prefix_Id, where Prefix is the object qualifier
        /// for this instance of DotNetNuke, and Id is the current PortalId for normal
        /// users or glbSuperUserAppName for SuperUsers.
        /// </remarks>
        public static string GetApplicationName()
        {
            string appName;

            if (Convert.ToString(HttpContext.Current.Items["ApplicationName"]) == "")
            {
                //No Application Name saved
                PortalSettings _PortalSettings = PortalController.GetCurrentPortalSettings();
                if (_PortalSettings == null)
                {
                    //No Name is defined and no portal is current so return "/"
                    appName = "/";
                }
                else
                {
                    //Get the "default' Application Name based on the PortalId
                    appName = GetApplicationName(_PortalSettings.PortalId);
                }
            }
            else
            {
                appName = Convert.ToString(HttpContext.Current.Items["ApplicationName"]);
            }

            return appName;
        }

        /// <summary>
        /// Gets the ApplicationName for the MemberRole API.
        /// </summary>
        /// <remarks>
        /// This overload is used to build the Application Name from the Portal Id
        /// </remarks>
        public static string GetApplicationName(int portalID)
        {
            string appName;

            //Get the Data Provider Configuration
            ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration("data");

            // Read the configuration specific information for the current Provider
            Provider objProvider = (Provider)providerConfiguration.Providers[providerConfiguration.DefaultProvider];

            //Get the Object Qualifier frm the Provider Configuration
            string qualifier = objProvider.Attributes["objectQualifier"];
            if (!String.IsNullOrEmpty(qualifier) && qualifier.EndsWith("_") == false)
            {
                qualifier += "_";
            }

            appName = qualifier + Convert.ToString(portalID);

            return appName;
        }

        public static XmlNode GetContent(string Content, string ContentType)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Content);
            if (ContentType == "")
            {
                return xmlDoc.DocumentElement;
            }
            else
            {
                return xmlDoc.SelectSingleNode(ContentType);
            }
        }

        /// <summary>
        /// GetDatabaseVersion - gets the current version in the DB
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>The DB Version</returns>
        public static string GetDatabaseVersion()
        {
            string strDatabaseVersion = "";

            try
            {
                IDataReader dr = PortalSettings.GetDatabaseVersion();
                if (dr.Read())
                {
                    strDatabaseVersion = Strings.Format(dr["Major"], "00") + Strings.Format(dr["Minor"], "00") + Strings.Format(dr["Build"], "00");
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                strDatabaseVersion = "ERROR:" + ex.Message;
            }

            return strDatabaseVersion;
        }

        // returns the database connection string
        [Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.Config.GetConnectionString")]
        public static string GetDBConnectionString()
        {
            return Config.GetConnectionString();
        }

        /// <summary>
        /// GetDesktopModuleByName is a Utility function that retrieves the Desktop Module
        /// defeined by the name provided.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="name">The Name of the Module</param>
        /// <returns>The Desktop Module</returns>
        public static DesktopModuleInfo GetDesktopModuleByName(string name)
        {
            DesktopModuleController objDesktopModules = new DesktopModuleController();

            //First attempt to retrieve the module based on the Module Name
            DesktopModuleInfo objDesktopModule = objDesktopModules.GetDesktopModuleByModuleName(name);

            if (objDesktopModule == null)
            {
                //Next attempt to retrieve the module based on the Friendly Name
                objDesktopModule = objDesktopModules.GetDesktopModuleByFriendlyName(name);
            }

            return objDesktopModule;
        }

        public static string GetDomainName(HttpRequest Request)
        {
            return GetDomainName(Request, false);
        }

        // returns the domain name of the current request ( ie. www.domain.com or 207.132.12.123 or www.domain.com/directory if subhost )
        // Actually, more to the point, returns the URL for the portal of this request.url.
        //   USE:
        //       Returns URI appropriate for checking against Portal Aliases.
        //   ASSUMPTIONS:
        //       portal access is always centric thru *.aspx or *.axd file;
        //       DotNetNuke application directory names are special (and not part of portal alias);
        //           so only specific DNN top level directory names are examined
        public static string GetDomainName(HttpRequest Request, bool ParsePortNumber)
        {
            StringBuilder DomainName = new StringBuilder();
            string[] URL;
            string URI; // holds Request.Url, less the "?" parameters
            int intURL;

            // split both URL separater, and parameter separator
            // We trim right of '?' so test for filename extensions can occur at END of URL-componenet.
            // Test:   'www.aspxforum.net'  should be returned as a valid domain name.
            // just consider left of '?' in URI
            // Binary, else '?' isn't taken literally; only interested in one (left) string
            URI = Request.Url.ToString();
            string hostHeader = Config.GetSetting("HostHeader");
            // HACK : Modified to use IsNullOrEmpty method instead of two if statements.          
            if (!String.IsNullOrEmpty(hostHeader))
            {
                URI = URI.ToLower().Replace(hostHeader.ToLower(), "");
            }
            intURL = (URI.IndexOf("?", 0) + 1);
            if (intURL > 0)
            {
                URI = URI.Substring(0, intURL - 1);
            }

            URL = URI.Split('/');

            for (intURL = 2; intURL <= URL.GetUpperBound(0); intURL++)
            {
                switch (URL[intURL].ToLower())
                {
                    case "admin":
                    case "controls":
                    case "desktopmodules":
                    case "mobilemodules":
                    case "premiummodules":
                    case "providers":
                        goto ExitLabel1;
                    default:
                        // exclude filenames ENDing in ".aspx" or ".axd" --- 
                        //   we'll use reverse match,
                        //   - but that means we are checking position of left end of the match;
                        //   - and to do that, we need to ensure the string we test against is long enough;
                        if (URL[intURL].Length >= ".aspx".Length) //long enough for both tests
                        {
                            if ((URL[intURL].LastIndexOf(".aspx") + 1) == (URL[intURL].Length - (".aspx".Length - 1)) | (URL[intURL].LastIndexOf(".axd") + 1) == (URL[intURL].Length - (".axd".Length - 1)))
                            {
                                goto ExitLabel1;
                            }
                        }
                        // non of the exclusionary names found
                        DomainName.Append(((DomainName.ToString() != "") ? "/" : "") + URL[intURL]);
                        break;
                }
            }
        ExitLabel1:

            // handle port specification
            if (ParsePortNumber)
            {
                if (DomainName.ToString().IndexOf(":") != -1)
                {
                    bool usePortNumber = (!Request.IsLocal);
                    if (Config.GetSetting("UsePortNumber") != null)
                    {
                        usePortNumber = bool.Parse(Config.GetSetting("UsePortNumber"));
                    }
                    if (usePortNumber == false)
                    {
                        DomainName = DomainName.Replace(":" + Request.Url.Port, "");
                    }
                }
            }

            return DomainName.ToString();

        }

        public static HttpWebRequest GetExternalRequest(string Address)
        {
            // Obtain PortalSettings from Current Context

            // Create the request object
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Address);

            // Set a time out to the request ... 10 seconds
            if (HostSettings["WebRequestTimeout"] != null)
            {
                objRequest.Timeout = int.Parse(Convert.ToString(HostSettings["WebRequestTimeout"]));
            }
            else
            {
                objRequest.Timeout = 10000;
            }

            // Attach a User Agent to the request
            objRequest.UserAgent = "DotNetNuke";

            // If there is Proxy info, apply it to the Request
            if (Convert.ToString(HostSettings["ProxyServer"]) != "")
            {
                // Create a new Proxy
                WebProxy Proxy;

                // Create a new Network Credentials item

                // Fill Proxy info from host settings
                Proxy = new WebProxy(Convert.ToString(HostSettings["ProxyServer"]), Convert.ToInt32(HostSettings["ProxyPort"]));

                if (Convert.ToString(HostSettings["ProxyUsername"]) != "")
                {
                    // Fill the credential info from host settings
                    NetworkCredential proxyCredentials = new NetworkCredential(Convert.ToString(HostSettings["ProxyUsername"]), Convert.ToString(HostSettings["ProxyPassword"]));

                    //Apply credentials to proxy
                    Proxy.Credentials = proxyCredentials;
                }

                // Apply Proxy to Request
                objRequest.Proxy = Proxy;
            }
            return objRequest;
        }

        public static HttpWebRequest GetExternalRequest(string Address, NetworkCredential Credentials)
        {
            // Obtain PortalSettings from Current Context
            //            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();

            // Create the request object
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Address);

            // Set a time out to the request ... 10 seconds
            if (HostSettings["WebRequestTimeout"] != null)
            {
                objRequest.Timeout = int.Parse(Convert.ToString(HostSettings["WebRequestTimeout"]));
            }
            else
            {
                objRequest.Timeout = 10000;
            }

            // Attach a User Agent to the request
            objRequest.UserAgent = "DotNetNuke";

            // Attach supplied credentials
            if (Credentials.UserName != null)
            {
                objRequest.Credentials = Credentials;
            }

            // If there is Proxy info, apply it to the Request
            if (Convert.ToString(HostSettings["ProxyServer"]) != "")
            {
                // Create a new Proxy
                WebProxy Proxy;

                // Create a new Network Credentials item

                // Fill Proxy info from host settings
                Proxy = new WebProxy(Convert.ToString(HostSettings["ProxyServer"]), Convert.ToInt32(HostSettings["ProxyPort"]));

                if (Convert.ToString(HostSettings["ProxyUsername"]) != "")
                {
                    // Fill the credential info from host settings
                    NetworkCredential proxyCredentials = new NetworkCredential(Convert.ToString(HostSettings["ProxyUsername"]), Convert.ToString(HostSettings["ProxyPassword"]));

                    //Apply credentials to proxy
                    Proxy.Credentials = proxyCredentials;
                }

                // Apply Proxy to Request
                objRequest.Proxy = Proxy;
            }
            return objRequest;
        }

        // get list of files from folder matching criteria
        public static ArrayList GetFileList()
        {
            return GetFileList(-1, "", true, "", false);
        }

        public static ArrayList GetFileList(int PortalId)
        {
            return GetFileList(PortalId, "", true, "", false);
        }

        public static ArrayList GetFileList(int PortalId, string strExtensions)
        {
            return GetFileList(PortalId, strExtensions, true, "", false);
        }

        public static ArrayList GetFileList(int PortalId, string strExtensions, bool NoneSpecified)
        {
            return GetFileList(PortalId, strExtensions, NoneSpecified, "", false);
        }

        public static ArrayList GetFileList(int PortalId, string strExtensions, bool NoneSpecified, string Folder)
        {
            return GetFileList(PortalId, strExtensions, NoneSpecified, Folder, false);
        }

        public static ArrayList GetFileList(int PortalId, string strExtensions, bool NoneSpecified, string Folder, bool includeHidden)
        {
            ArrayList arrFileList = new ArrayList();

            if (NoneSpecified)
            {
                arrFileList.Add(new FileItem("", "<" + Localization.GetString("None_Specified") + ">"));
            }

            string portalRoot = null;
            if (PortalId == Null.NullInteger)
            {
                portalRoot = HostMapPath;
            }
            else
            {
                PortalController objPortals = new PortalController();
                PortalInfo objPortal = objPortals.GetPortal(PortalId);
                portalRoot = objPortal.HomeDirectoryMapPath;
            }

            FolderInfo objFolder = FileSystemUtils.GetFolder(PortalId, Folder);
            if (objFolder != null)
            {
                FileController objFiles = new FileController();
                IDataReader dr = objFiles.GetFiles(PortalId, objFolder.FolderID);
                while (dr.Read())
                {
                    if ((strExtensions.ToUpper().IndexOf(dr["Extension"].ToString().ToUpper(), 0) + 1) != 0 | strExtensions == "")
                    {
                        string filePath = (portalRoot + dr["Folder"].ToString() + dr["fileName"].ToString()).Replace("/", "\\");
                        int StorageLocation = 0;

                        if (dr["StorageLocation"] != null)
                        {
                            StorageLocation = System.Convert.ToInt32(dr["StorageLocation"]);
                            switch (StorageLocation)
                            {
                                case 1: // Secure File System
                                    filePath = filePath + glbProtectedExtension;
                                    break;
                                case 2: // Secure Database
                                    break;
                                default: // Insecure File System
                                    break;
                            }
                        }

                        // check if file exists - as the database may not be synchronized with the file system
                        // Make sure its not a file stored in the db, if it is we don't worry about seeing if it exists
                        if (!(StorageLocation == 2))
                        {
                            if (File.Exists(filePath))
                            {
                                // check if file is hidden
                                if (includeHidden)
                                {
                                    arrFileList.Add(new FileItem(dr["FileID"].ToString(), dr["FileName"].ToString()));
                                }
                                else
                                {
                                    System.IO.FileAttributes attributes = File.GetAttributes(filePath);
                                    if (!((attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                                    {
                                        arrFileList.Add(new FileItem(dr["FileID"].ToString(), dr["FileName"].ToString()));
                                    }
                                }
                            }
                        }
                        else
                        {
                            // File is stored in DB - Just add to arraylist
                            arrFileList.Add(new FileItem(dr["FileID"].ToString(), dr["FileName"].ToString()));
                        }
                        //END Change
                    }
                }
                dr.Close();
            }

            return arrFileList;
        }

        public static string GetHashValue(object HashObject, string DefaultValue)
        {
            if (HashObject != null)
            {
                if (Convert.ToString(HashObject) != "")
                {
                    return Convert.ToString(HashObject);
                }
                else
                {
                    return DefaultValue;
                }
            }
            else
            {
                return DefaultValue;
            }
        }

        public static PortalSettings GetHostPortalSettings()
        {
            int tabId = -1;
            int portalId = -1;

            PortalAliasInfo objPortalAliasInfo = null;

            // if the portal alias exists
            if (Convert.ToString(HostSettings["HostPortalId"]) != "")
            {
                // use the host portal
                objPortalAliasInfo = new PortalAliasInfo();
                objPortalAliasInfo.PortalID = int.Parse(Convert.ToString(HostSettings["HostPortalId"]));

                portalId = objPortalAliasInfo.PortalID;
            }

            // load the PortalSettings into current context
            return new PortalSettings(tabId, objPortalAliasInfo);
        }

        // returns a SQL Server compatible date
        public static string GetMediumDate(string mediumDate)
        {
            if (!String.IsNullOrEmpty(mediumDate))
            {
                DateTime datDate = Convert.ToDateTime(mediumDate);

                string strYear = DateAndTime.Year(datDate).ToString();
                string strMonth = DateAndTime.MonthName(DateAndTime.Month(datDate), true);
                string strDay = DateAndTime.Day(datDate).ToString();

                mediumDate = strDay + "-" + strMonth + "-" + strYear;
            }

            return mediumDate;
        }

        public static string GetOnLineHelp(string HelpUrl, ModuleInfo moduleConfig)
        {
            bool isAdminModule = moduleConfig.IsAdmin;
            string showOnlineHelp = Convert.ToString(HostSettings["EnableModuleOnLineHelp"]);
            string ctlString = Convert.ToString(HttpContext.Current.Request.QueryString["ctl"]);

            if ((showOnlineHelp == "Y" && !isAdminModule) || (isAdminModule))
            {
                if ((isAdminModule) || (IsAdminControl() && ctlString == "Module") || (IsAdminControl() && ctlString == "Tab"))
                {
                    string hostHelpUrl = Convert.ToString(HostSettings["HelpURL"]);
                    HelpUrl = hostHelpUrl;
                }
            }
            else
            {
                HelpUrl = "";
            }

            return HelpUrl;
        }

        // retrieves the domain name of the portal ( ie. http://www.domain.com " )
        public static string GetPortalDomainName(string strPortalAlias, HttpRequest Request, bool blnAddHTTP)
        {
            string returnValue;

            string strDomainName = "";

            string strURL = "";
            string[] arrPortalAlias;
            int intAlias;

            if (Request != null)
            {
                strURL = GetDomainName(Request);
            }

            arrPortalAlias = strPortalAlias.Split(',');
            for (intAlias = 0; intAlias <= arrPortalAlias.Length - 1; intAlias++)
            {
                if (arrPortalAlias[intAlias] == strURL)
                {
                    strDomainName = arrPortalAlias[intAlias];
                }
            }
            if (strDomainName == "")
            {
                strDomainName = arrPortalAlias[0];
            }

            if (blnAddHTTP)
            {
                strDomainName = AddHTTP(strDomainName);
            }

            returnValue = strDomainName;

            return returnValue;
        }

        public static PortalSettings GetPortalSettings()
        {
            PortalSettings portalSettings = null;

            //Try getting the settings from the Context
            if (HttpContext.Current != null)
            {
                portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }

            // If nothing then try getting the Host Settings
            if (portalSettings == null)
            {
                portalSettings = GetHostPortalSettings();
            }

            return portalSettings;
        }

        public static ArrayList GetPortalTabs(int intPortalId, bool blnNoneSpecified, bool blnHidden, bool blnDeleted, bool blnURL, bool bCheckAuthorised)
        {

            TabController objTabs = new TabController();
            ArrayList arrTabs = null;

            // Obtain current PortalSettings from Current Context
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            //Get the portal's tabs
            if (intPortalId == _portalSettings.PortalId)
            {
                //Load current Portals tabs into arrTabs
                arrTabs = _portalSettings.DesktopTabs;
            }
            else
            {
                //We are editing a different portal (as host) so get the portal's tabs
                arrTabs = objTabs.GetTabs(intPortalId);
            }
            return GetPortalTabs(arrTabs, -1, blnNoneSpecified, blnHidden, blnDeleted, blnURL, bCheckAuthorised);

        }

        public static ArrayList GetPortalTabs(ArrayList objDesktopTabs, bool blnNoneSpecified, bool blnHidden)
        {
            return GetPortalTabs(objDesktopTabs, -1, blnNoneSpecified, blnHidden, false, false, false);
        }

        public static ArrayList GetPortalTabs(ArrayList objDesktopTabs, bool blnNoneSpecified, bool blnHidden, bool blnDeleted, bool blnURL)
        {
            return GetPortalTabs(objDesktopTabs, -1, blnNoneSpecified, blnHidden, blnDeleted, blnURL, false);
        }

        public static ArrayList GetPortalTabs(ArrayList objDesktopTabs, int currentTab, bool blnNoneSpecified, bool blnHidden, bool blnDeleted, bool blnURL, bool bCheckAuthorised)
        {
            ArrayList arrPortalTabs = new ArrayList();
            TabInfo objTab;

            if (blnNoneSpecified)
            {
                objTab = new TabInfo();
                objTab.TabID = -1;
                objTab.TabName = "<" + Localization.GetString("None_Specified") + ">";
                objTab.TabOrder = 0;
                objTab.ParentId = -2;
                arrPortalTabs.Add(objTab);
            }

            foreach (TabInfo tempLoopVar_objTab in objDesktopTabs)
            {
                objTab = tempLoopVar_objTab;
                if ((currentTab < 0) || (objTab.TabID != currentTab))
                {
                    if (objTab.IsAdminTab == false && (objTab.IsVisible || blnHidden) && (objTab.IsDeleted == false || blnDeleted) && (objTab.TabType == TabType.Normal || blnURL))
                    {
                        TabInfo tabTemp = objTab.Clone();
                        string strIndent = "";

                        for (int intCounter = 1; intCounter <= tabTemp.Level; intCounter++)
                        {
                            strIndent += "...";
                        }
                        tabTemp.TabName = strIndent + tabTemp.TabName;
                        if (bCheckAuthorised)
                        {
                            //Check if User has Administrator rights to this tab
                            if (PortalSecurity.IsInRoles(tabTemp.AdministratorRoles))
                            {
                                arrPortalTabs.Add(tabTemp);
                            }
                        }
                        else
                        {
                            arrPortalTabs.Add(tabTemp);
                        }
                    }
                }
            }

            return arrPortalTabs;
        }

        public static string GetRoleName(int RoleID)
        {
            if (Convert.ToString(RoleID) == glbRoleAllUsers)
            {
                return "All Users";
            }
            else if (Convert.ToString(RoleID) == glbRoleUnauthUser)
            {
                return "Unauthenticated Users";
            }

            Hashtable htRoles = null;
            if (PerformanceSetting != PerformanceSettings.NoCaching)
            {
                htRoles = (Hashtable)DataCache.GetCache("GetRoles");
            }

            if (htRoles == null)
            {
                RoleController objRoleController = new RoleController();
                ArrayList arrRoles;
                arrRoles = objRoleController.GetRoles();
                htRoles = new Hashtable();
                int i;
                for (i = 0; i <= arrRoles.Count - 1; i++)
                {
                    RoleInfo objRole;
                    objRole = (RoleInfo)arrRoles[i];
                    htRoles.Add(objRole.RoleID, objRole.RoleName);
                }
                if (PerformanceSetting != PerformanceSettings.NoCaching)
                {
                    DataCache.SetCache("GetRoles", htRoles);
                }
            }
            return Convert.ToString(htRoles[RoleID]);
        }

        // returns a SQL Server compatible date
        public static string GetShortDate(string strDate)
        {
            if (!String.IsNullOrEmpty(strDate))
            {
                DateTime datDate = Convert.ToDateTime(strDate);

                string strYear = DateAndTime.Year(datDate).ToString();
                string strMonth = DateAndTime.Month(datDate).ToString();
                string strDay = DateAndTime.Day(datDate).ToString();

                strDate = strMonth + "/" + strDay + "/" + strYear;
            }

            return strDate;
        }

        /// <summary>
        /// Returns the folder path under the root for the portal
        /// </summary>
        /// <param name="strFileNamePath">The folder the absolute path</param>
        /// <remarks>
        /// </remarks>
        public static string GetSubFolderPath(string strFileNamePath)
        {
            string returnValue;
            // Obtain PortalSettings from Current Context
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string ParentFolderName;
            if (_portalSettings.ActiveTab.ParentId == _portalSettings.SuperTabId)
            {
                ParentFolderName = HostMapPath.Replace("/", "\\");
            }
            else
            {
                ParentFolderName = _portalSettings.HomeDirectoryMapPath.Replace("/", "\\");
            }
            string strFolderpath = strFileNamePath.Substring(0, strFileNamePath.LastIndexOf("\\") + 1);

            returnValue = strFolderpath.Substring(ParentFolderName.Length).Replace("\\", "/");

            return returnValue;
        }

        /// <summary>
        /// The GetTotalRecords method gets the number of Records returned.
        /// </summary>
        /// <param name="dr">An <see cref="IDataReader"/> containing the Total no of records</param>
        /// <returns>An Integer</returns>
        public static int GetTotalRecords(ref IDataReader dr)
        {
            int total = 0;

            if (dr.Read())
            {
                try
                {
                    total = Convert.ToInt32(dr["TotalRecords"]);
                }
                catch (Exception)
                {
                    total = -1;
                }
            }

            return total;
        }

        /// <summary>
        /// GetUpgradeStatus - determines whether an upgrade/install is required
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>An UpgradeStatus enum Upgrade/Install/None</returns>
        public static UpgradeStatus GetUpgradeStatus()
        {
            string strAssemblyVersion = glbAppVersion.Replace(".", "");
            string strDatabaseVersion;
            string strConfirmVersion = "";
            XmlDocument xmlConfig = new XmlDocument();

            UpgradeStatus status = UpgradeStatus.None;

            // first call GetProviderPath - this insures that the Database is Initialised correctly
            //and also generates the appropriate error message if it cannot be initialised correctly
            string strProviderPath = PortalSettings.GetProviderPath();

            // get current database version from DB
            if (!strProviderPath.StartsWith("ERROR:"))
            {
                strDatabaseVersion = GetDatabaseVersion();
            }
            else
            {
                strDatabaseVersion = strProviderPath;
            }

            if (strDatabaseVersion.StartsWith("ERROR"))
            {
                status = UpgradeStatus.Error;
            }
            else if (strDatabaseVersion == "")
            {
                //No Db Version so Install
                status = UpgradeStatus.Install;
            }
            else
            {
                if (strDatabaseVersion != strAssemblyVersion)
                {
                    //Upgrade Required - confirm by checking the DB Version in the DB
                    status = UpgradeStatus.Upgrade;
                }
            }

            return status;
        }

        /// <summary>
        /// Returns the type of URl (T=other tab, F=file, U=URL, N=normal)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="URL">The url</param>
        /// <returns>The url type</returns>
        public static TabType GetURLType(string URL)
        {
            if (URL == "")
            {
                return TabType.Normal; // normal tab
            }
            else
            {
                if (URL.ToLower().StartsWith("mailto:") == false && URL.IndexOf("://") == -1 && URL.StartsWith("~") == false && URL.StartsWith("\\\\") == false && URL.StartsWith("/") == false)
                {
                    if (Information.IsNumeric(URL))
                    {
                        return TabType.Tab; // internal tab ( ie. 23 = tabid )
                    }
                    else
                    {
                        if (URL.ToLower().StartsWith("userid="))
                        {
                            return TabType.Member; // userid=
                        }
                        else
                        {
                            return TabType.File; // internal file ( ie. folder/file.ext or fileid= )
                        }
                    }
                }
                else
                {
                    return TabType.Url; // external url ( eg. http://www.domain.com )
                }
            }
        }

        // encodes a URL for posting to an external site
        public static string HTTPPOSTEncode(string strPost)
        {
            string returnValue;
            strPost = strPost.Replace("\\", "");
            strPost = HttpUtility.UrlEncode(strPost);
            strPost = strPost.Replace("%2f", "/");
            returnValue = strPost;
            return returnValue;
        }

        public static string ImportUrl(int ModuleId, string url)
        {
            string strUrl = url;

            if (GetURLType(url) == TabType.File)
            {
                ModuleController objModuleController = new ModuleController();
                FileController objFileController = new FileController();
                ModuleInfo objModule = objModuleController.GetModule(ModuleId, Null.NullInteger, true);

                strUrl = "FileID=" + objFileController.ConvertFilePathToFileId(url, objModule.PortalID);
            }

            return strUrl;
        }

        // returns a boolean value whether the control is an admin control
        public static bool IsAdminControl()
        {
            // This is needed to avoid an exception if there is no Context.  This will occur if code is called from the Scheduler
            if (HttpContext.Current == null)
            {
                return false;
            }
            return (Information.IsNothing(HttpContext.Current.Request.QueryString["mid"]) == false) || (Information.IsNothing(HttpContext.Current.Request.QueryString["ctl"]) == false);
        }

        // returns a boolean value whether the page should display an admin skin
        public static bool IsAdminSkin(bool IsAdminTab)
        {
            string AdminKeys = "tab,module,importmodule,exportmodule,help";

            string ControlKey = "";
            if (HttpContext.Current.Request.QueryString["ctl"] != null)
            {
                ControlKey = HttpContext.Current.Request.QueryString["ctl"].ToLower();
            }

            int ModuleID = -1;
            if (HttpContext.Current.Request.QueryString["mid"] != null)
            {
                ModuleID = int.Parse(HttpContext.Current.Request.QueryString["mid"]);
            }

            return IsAdminTab || (!String.IsNullOrEmpty(ControlKey) && ControlKey != "view" && ModuleID != -1) || (!String.IsNullOrEmpty(ControlKey) && AdminKeys.IndexOf(ControlKey) != -1 && ModuleID == -1);
        }

        /// <summary>
        /// Returns whether the currnet tab is in LayoutMode
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static bool IsLayoutMode()
        {
            bool blnReturn = false;
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            if ((PortalSecurity.IsInRole(_portalSettings.AdministratorRoleName.ToString()) || PortalSecurity.IsInRoles(_portalSettings.ActiveTab.AdministratorRoles.ToString())) && IsTabPreview() == false)
            {
                if (_portalSettings.ActiveTab.IsAdminTab == false)
                {
                    blnReturn = true;
                }
            }
            return blnReturn;
        }

        /// <summary>
        /// Returns whether the tab being displayed is in preview mode
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static bool IsTabPreview()
        {
            bool blnReturn = false;
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            if (HttpContext.Current.Request != null && HttpContext.Current.Request.Cookies["_Tab_Admin_Preview" + _portalSettings.PortalId] != null)
            {
                blnReturn = bool.Parse(HttpContext.Current.Request.Cookies["_Tab_Admin_Preview" + _portalSettings.PortalId].Value);
            }
            return blnReturn;
        }

        public static string LinkClick(string Link, int TabID, int ModuleID)
        {
            return LinkClick(Link, TabID, ModuleID, true, "");
        }

        public static string LinkClick(string Link, int TabID, int ModuleID, bool TrackClicks)
        {
            return LinkClick(Link, TabID, ModuleID, TrackClicks, "");
        }

        public static string LinkClick(string Link, int TabID, int ModuleID, bool TrackClicks, string ContentType)
        {
            return LinkClick(Link, TabID, ModuleID, TrackClicks, !String.IsNullOrEmpty(ContentType));
        }

        public static string LinkClick(string Link, int TabID, int ModuleID, bool TrackClicks, bool ForceDownload)
        {
            string strLink = "";

            TabType UrlType = GetURLType(Link);

            if (TrackClicks || ForceDownload || UrlType == TabType.File)
            {
                // format LinkClick wrapper
                if (Link.ToLower().StartsWith("fileid="))
                {
                    strLink = ApplicationPath + "/LinkClick.aspx?fileticket=" + UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(Link));
                }
                if (Link.ToLower().StartsWith("userid="))
                {
                    strLink = ApplicationPath + "/LinkClick.aspx?userticket=" + UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(Link));
                }
                if (strLink == "")
                {
                    strLink = ApplicationPath + "/LinkClick.aspx?link=" + HttpUtility.UrlEncode(Link);
                }

                // tabid is required to identify the portal where the click originated
                if (TabID != Null.NullInteger)
                {
                    strLink += "&tabid=" + TabID;
                }

                // moduleid is used to identify the module where the url is stored
                if (ModuleID != -1)
                {
                    strLink += "&mid=" + ModuleID;
                }

                // force a download dialog
                if (ForceDownload)
                {
                    strLink += "&forcedownload=true";
                }
            }
            else
            {
                switch (UrlType)
                {
                    case TabType.Tab:

                        strLink = NavigateURL(int.Parse(Link));
                        break;
                    case TabType.Member:

                        PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                        strLink = NavigateURL(_portalSettings.ActiveTab.TabID, "ViewProfile", "userticket=" + UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(Link)));
                        break;
                    default:

                        strLink = Link;
                        break;
                }
            }

            return strLink;
        }

        [Obsolete("This function has been obsoleted: Use Common.Globals.LinkClick() for proper handling of URLs")]
        public static string LinkClickURL(string Link)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return LinkClick(Link, _portalSettings.ActiveTab.TabID, -1, false);
        }

        // injects the upload directory into raw HTML for src and background tags
        public static string ManageUploadDirectory(string strHTML, string strUploadDirectory)
        {
            string returnValue = "";

            if (!String.IsNullOrEmpty(strHTML))
            {
                int para = Strings.InStr(1, strHTML.ToLower(), "src=\"", 0);
                while (para != 0)
                {
                    returnValue = returnValue + strHTML.Substring(0, para + 4);

                    strHTML = strHTML.Substring(para + 5 - 1);

                    // add uploaddirectory if we are linking internally
                    if (Strings.InStr(1, strHTML, "://", 0) == 0)
                    {
                        strHTML = strUploadDirectory + strHTML;
                    }

                    para = Strings.InStr(1, strHTML.ToLower(), "src=\"", 0);
                }
                returnValue = returnValue + strHTML;

                strHTML = returnValue;
                returnValue = "";

                para = Strings.InStr(1, strHTML.ToLower(), "background=\"", 0);
                while (para != 0)
                {
                    returnValue = returnValue + strHTML.Substring(0, para + 11);

                    strHTML = strHTML.Substring(para + 12 - 1);

                    // add uploaddirectory if we are linking internally
                    if (Strings.InStr(1, strHTML, "://", 0) == 0)
                    {
                        strHTML = strUploadDirectory + strHTML;
                    }

                    para = Strings.InStr(1, strHTML.ToLower(), "background=\"", 0);
                }
            }

            returnValue = returnValue + strHTML;

            return returnValue;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            return NavigateURL(_portalSettings.ActiveTab.TabID, Null.NullString);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID)
        {
            return NavigateURL(TabID, Null.NullString);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, bool IsSuperTab)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            return NavigateURL(TabID, IsSuperTab, _portalSettings, Null.NullString, null);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(string ControlKey)
        {
            if (ControlKey == "Access Denied")
            {
                return AccessDeniedURL();
            }
            else
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                return NavigateURL(_portalSettings.ActiveTab.TabID, ControlKey);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, string ControlKey)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            return NavigateURL(TabID, _portalSettings, ControlKey, null);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, string ControlKey, params string[] AdditionalParameters)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            return NavigateURL(TabID, _portalSettings, ControlKey, AdditionalParameters);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, PortalSettings settings, string ControlKey, params string[] AdditionalParameters)
        {
            bool isSuperTab = false;
            if (!(settings == null))
            {
                if (settings.ActiveTab.IsSuperTab)
                {
                    isSuperTab = true;
                }
            }

            return NavigateURL(TabID, isSuperTab, settings, ControlKey, AdditionalParameters);
        }

        public static string NavigateURL(int TabID, bool IsSuperTab, PortalSettings settings, string ControlKey, params string[] AdditionalParameters)
        {
            string strURL;

            if (TabID == Null.NullInteger)
            {
                strURL = ApplicationURL();
            }
            else
            {
                strURL = ApplicationURL(TabID);
            }

            if (!String.IsNullOrEmpty(ControlKey))
            {
                strURL += "&ctl=" + ControlKey;
            }

            if (!(AdditionalParameters == null))
            {
                foreach (string parameter in AdditionalParameters)
                {
                    strURL += "&" + parameter;
                }
            }

            if (IsSuperTab)
            {
                strURL += "&portalid=" + settings.PortalId;
            }

            if (Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") == "Y")
            {
                foreach (TabInfo objTab in settings.DesktopTabs)
                {
                    if (objTab.TabID == TabID)
                    {
                        return FriendlyUrl(objTab, strURL, settings);
                    }
                }

                return FriendlyUrl(null, strURL, settings);
            }
            else
            {
                return ResolveUrl(strURL);
            }
        }

        // Deprecated PreventSQLInjection Function to consolidate Security Filter functions in the PortalSecurity class.
        public static string PreventSQLInjection(string strSQL)
        {
            return (new PortalSecurity()).InputFilter(strSQL, PortalSecurity.FilterFlag.NoSQL);
        }

        public static string QueryStringDecode(string QueryString)
        {
            QueryString = HttpUtility.UrlDecode(QueryString);
            string fullPath;
            try
            {
                fullPath = HttpContext.Current.Request.MapPath(QueryString, HttpContext.Current.Request.ApplicationPath, false);
            }
            catch (HttpException)
            {
                //attempted cross application mapping
                throw (new HttpException(404, "Not Found"));
            }
            string strDoubleDecodeURL = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Server.UrlDecode(QueryString));
            if (QueryString.IndexOf("..") != -1 || strDoubleDecodeURL.IndexOf("..") != -1)
            {
                //attempted parent path traversal
                throw (new HttpException(404, "Not Found"));
            }
            return QueryString;
        }

        public static string QueryStringEncode(string QueryString)
        {
            QueryString = HttpUtility.UrlEncode(QueryString);

            return QueryString;
        }

        /// <summary>
        /// Generates the correctly formatted url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="url">The url to format.</param>
        /// <returns>The formatted (resolved) url</returns>
        public static string ResolveUrl(string url)
        {
            // String is Empty, just return Url
            if (url.Length == 0)
            {
                return url;
            }

            // String does not contain a ~, so just return Url
            if (url.StartsWith("~") == false)
            {
                return url;
            }

            // There is just the ~ in the Url, return the appPath
            if (url.Length == 1)
            {
                return ApplicationPath;
            }

            if ((url.ToCharArray()[1] == '/' || url.ToCharArray()[1] == '\\'))
            {
                // Url looks like ~/ or ~\
                if (ApplicationPath.Length > 1)
                {
                    return ApplicationPath + "/" + url.Substring(2);
                }
                else
                {
                    return "/" + url.Substring(2);
                }
            }
            else
            {
                // Url look like ~something
                if (ApplicationPath.Length > 1)
                {
                    return ApplicationPath + "/" + url.Substring(1);
                }
                else
                {
                    return ApplicationPath + url.Substring(1);
                }
            }
        }

        [Obsolete("This function has been replaced by DotNetNuke.Services.Mail.Mail.SendMail")]
        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding, string Body, string Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword)
        {
            return Mail.SendMail(MailFrom, MailTo, Cc, Bcc, Priority, Subject, BodyFormat, BodyEncoding, Body, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword);
        }

        //These functions have been replaced by the Mail class in
        //DotNetNuke.Services.Mail.  They have been retained here
        //for backwards compatabily, but flagged as obsolete to
        //discourage use

        [Obsolete("This function has been replaced by DotNetNuke.Services.Mail.Mail.SendMail")]
        public static string SendNotification(string MailFrom, string MailTo, string Bcc, string Subject, string Body)
        {
            return Mail.SendMail(MailFrom, MailTo, Bcc, Subject, Body, "", "", "", "", "", "");
        }

        [Obsolete("This function has been replaced by DotNetNuke.Services.Mail.Mail.SendMail")]
        public static string SendNotification(string MailFrom, string MailTo, string Bcc, string Subject, string Body, string Attachment)
        {
            return Mail.SendMail(MailFrom, MailTo, Bcc, Subject, Body, Attachment, "", "", "", "", "");
        }

        [Obsolete("This function has been replaced by DotNetNuke.Services.Mail.Mail.SendMail")]
        public static string SendNotification(string MailFrom, string MailTo, string Bcc, string Subject, string Body, string Attachment, string BodyType)
        {
            return Mail.SendMail(MailFrom, MailTo, Bcc, Subject, Body, Attachment, BodyType, "", "", "", "");
        }

        [Obsolete("This function has been replaced by DotNetNuke.Services.Mail.Mail.SendMail")]
        public static string SendNotification(string MailFrom, string MailTo, string Bcc, string Subject, string Body, string Attachment, string BodyType, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword)
        {
            return Mail.SendMail(MailFrom, MailTo, Bcc, Subject, Body, Attachment, BodyType, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword);
        }

        [Obsolete("This function has been replaced by DotNetNuke.Services.Mail.Mail.SendMail")]
        public static string SendNotification(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding, string Body, string Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword)
        {
            return Mail.SendMail(MailFrom, MailTo, Cc, Bcc, Priority, Subject, BodyFormat, BodyEncoding, Body, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword);
        }

        /// <summary>
        /// SerializeHashTableBase64 serializes a Hashtable using Binary Formatting
        /// </summary>
        /// <remarks>
        /// While this method of serializing is no longer supported (due to Medium Trust
        /// issue, it is still required for upgrade purposes.
        /// </remarks>
        /// <param name="Source">The Hashtable to serialize</param>
        /// <returns>The serialized String</returns>
        public static string SerializeHashTableBase64(Hashtable Source)
        {
            string strString;
            if (Source.Count != 0)
            {
                BinaryFormatter bin = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                try
                {
                    bin.Serialize(mem, Source);
                    strString = Convert.ToBase64String(mem.GetBuffer(), 0, Convert.ToInt32(mem.Length));
                }
                catch (Exception)
                {
                    strString = "";
                }
                finally
                {
                    mem.Close();
                }
            }
            else
            {
                strString = "";
            }
            return strString;
        }

        /// <summary>
        /// SerializeHashTableXml serializes a Hashtable using Xml Serialization
        /// </summary>
        /// <remarks>
        /// This is the preferred method of serialization under Medium Trust
        /// </remarks>
        /// <param name="Source">The Hashtable to serialize</param>
        /// <returns>The serialized String</returns>
        public static string SerializeHashTableXml(Hashtable Source)
        {
            string strString;
            if (Source.Count != 0)
            {
                XmlDocument xmlProfile = new XmlDocument();
                XmlElement xmlRoot = xmlProfile.CreateElement("profile");
                xmlProfile.AppendChild(xmlRoot);

                foreach (string key in Source.Keys)
                {
                    //Create the item Node
                    XmlElement xmlItem = xmlProfile.CreateElement("item");

                    //Save the key name and the object type
                    xmlItem.SetAttribute("key", key);
                    xmlItem.SetAttribute("type", Source[key].GetType().AssemblyQualifiedName.ToString());

                    //Serialize the object
                    XmlDocument xmlObject = new XmlDocument();
                    XmlSerializer xser = new XmlSerializer(Source[key].GetType());
                    StringWriter sw = new StringWriter();
                    xser.Serialize(sw, Source[key]);
                    xmlObject.LoadXml(sw.ToString());

                    //import and append the node to the root of the profile
                    xmlItem.AppendChild(xmlProfile.ImportNode(xmlObject.DocumentElement, true));
                    xmlRoot.AppendChild(xmlItem);
                }

                //Return the OuterXml of the profile
                strString = xmlProfile.OuterXml;
            }
            else
            {
                strString = "";
            }
            return strString;
        }

        [Obsolete("This method has been deprecated. Replaced by same method in FileSystemUtils class.")]
        public static string UploadFile(string RootPath, HttpPostedFile objHtmlInputFile, bool Unzip)
        {
            return FileSystemUtils.UploadFile(RootPath, objHtmlInputFile, Unzip);
        }

        [Obsolete("This function has been replaced by DotNetNuke.Common.Utilities.XmlUtils.XMLEncode")]
        public static string XMLEncode(string HTML)
        {
            return XmlUtils.XMLEncode(HTML);
        }

        [Obsolete("This method has been deprecated.")]
        public static void AddFile(string strFileName, string strExtension, string FolderPath, string strContentType, int Length, int imageWidth, int imageHeight)
        {
            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            int portalId;

            FileController objFiles = new FileController();
            if (portalSettings.ActiveTab.ParentId == portalSettings.SuperTabId)
            {
                portalId = Null.NullInteger;
            }
            else
            {
                portalId = portalSettings.PortalId;
            }

            objFiles.AddFile(portalId, strFileName, strExtension, Length, imageWidth, imageHeight, strContentType, FolderPath);
        }

        [Obsolete("This method has been deprecated. ")]
        public static ArrayList GetFileList(DirectoryInfo CurrentDirectory, string strExtensions)
        {
            return GetFileList(CurrentDirectory, strExtensions, true);
        }

        [Obsolete("This method has been deprecated. ")]
        public static ArrayList GetFileList(DirectoryInfo CurrentDirectory)
        {
            return GetFileList(CurrentDirectory, "", true);
        }

        /// <summary>
        /// get list of files from folder matching criteria
        /// </summary>
        [Obsolete("This method has been deprecated. ")]
        public static ArrayList GetFileList(DirectoryInfo currentDirectory, string strExtensions, bool noneSpecified)
        {
            ArrayList arrFileList = new ArrayList();
            string strExtension = "";

            if (noneSpecified)
            {

                arrFileList.Add(new FileItem("", "<" + Localization.GetString("None_Specified") + ">"));
            }

            string[] Files = Directory.GetFiles(currentDirectory.FullName);
            foreach (string File in Files)
            {
                if (Convert.ToBoolean((File.IndexOf(".", 0) + 1)))
                {
                    strExtension = File.Substring((File.LastIndexOf(".") + 1));
                }
                string FileName = File.Substring(currentDirectory.FullName.Length);
                if ((strExtensions.ToUpper().IndexOf(strExtension.ToUpper(), 0) + 1) != 0 | strExtensions == "")
                {
                    arrFileList.Add(new FileItem(FileName, FileName));
                }
            }

            return arrFileList;
        }

        // creates RRS files
        public static void CreateRSS(IDataReader dr, string TitleField, string URLField, string CreatedDateField, string SyndicateField, string DomainName, string FileName)
        {
            // Obtain PortalSettings from Current Context
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            // create RSS file
            string strRSS = "";

            string strRelativePath = DomainName + FileName.Substring(Strings.InStr(1, FileName, "\\Portals", 0) - 1).Replace("\\", "/");
            strRelativePath = strRelativePath.Substring(0, Strings.InStrRev(strRelativePath, "/", -1, 0));

            while (dr.Read())
            {
                if (Convert.ToBoolean(dr[SyndicateField]))
                {
                    strRSS += "      <item>" + "\r\n";
                    strRSS += "         <title>" + dr[TitleField] + "</title>" + "\r\n";
                    if (Strings.InStr(1, dr["URL"].ToString(), "://", 0) == 0)
                    {
                        if (Information.IsNumeric(dr["URL"].ToString()))
                        {
                            strRSS += "         <link>" + DomainName + "/" + glbDefaultPage + "?tabid=" + dr[URLField] + "</link>" + "\r\n";
                        }
                        else
                        {
                            strRSS += "         <link>" + strRelativePath + dr[URLField] + "</link>" + "\r\n";
                        }
                    }
                    else
                    {
                        strRSS += "         <link>" + dr[URLField] + "</link>" + "\r\n";
                    }
                    strRSS += "         <description>" + _portalSettings.PortalName + " " + GetMediumDate(dr[CreatedDateField].ToString()) + "</description>" + "\r\n";
                    strRSS += "     </item>" + "\r\n";
                }
            }
            dr.Close();

            if (!String.IsNullOrEmpty(strRSS))
            {
                strRSS = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" + "\r\n" + "<rss version=\"0.91\">" + "\r\n" + "  <channel>" + "\r\n" + "     <title>" + _portalSettings.PortalName + "</title>" + "\r\n" + "     <link>" + DomainName + "</link>" + "\r\n" + "     <description>" + _portalSettings.PortalName + "</description>" + "\r\n" + "     <language>en-us</language>" + "\r\n" + "     <copyright>" + _portalSettings.FooterText + "</copyright>" + "\r\n" + "     <webMaster>" + _portalSettings.Email + "</webMaster>" + "\r\n" + strRSS + "   </channel>" + "\r\n" + "</rss>";

                StreamWriter objStream;
                objStream = File.CreateText(FileName);
                objStream.WriteLine(strRSS);
                objStream.Close();
            }
            else
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }
            }
        }

        public static void DeleteFilesRecursive(string strRoot, string filter)
        {
            if (!String.IsNullOrEmpty(strRoot))
            {
                if (Directory.Exists(strRoot))
                {
                    foreach (string tempLoopVar_strFolder in Directory.GetDirectories(strRoot))
                    {
                        string strFolder = tempLoopVar_strFolder;
                        DeleteFilesRecursive(strFolder, filter);
                    }

                    foreach (string tempLoopVar_strFile in Directory.GetFiles(strRoot, "*" + filter))
                    {
                        string strFile = tempLoopVar_strFile;
                        try
                        {
                            File.SetAttributes(strFile, FileAttributes.Normal);
                            File.Delete(strFile);
                        }
                        catch
                        {
                            // error deleting file
                        }
                    }
                }
            }
        }

        public static void DeleteFolderRecursive(string strRoot)
        {
            if (!String.IsNullOrEmpty(strRoot))
            {
                if (Directory.Exists(strRoot))
                {
                    foreach (string tempLoopVar_strFolder in Directory.GetDirectories(strRoot))
                    {
                        string strFolder = tempLoopVar_strFolder;
                        DeleteFolderRecursive(strFolder);
                    }
                    foreach (string tempLoopVar_strFile in Directory.GetFiles(strRoot))
                    {
                        string strFile = tempLoopVar_strFile;
                        try
                        {
                            File.SetAttributes(strFile, FileAttributes.Normal);
                            File.Delete(strFile);
                        }
                        catch
                        {
                            // error deleting file
                        }
                    }
                    try
                    {
                        Directory.Delete(strRoot);
                    }
                    catch
                    {
                        // error deleting folder
                    }
                }
            }
        }

        /// <summary>
        /// Sets the ApplicationName for the MemberRole API
        /// </summary>
        /// <remarks>
        /// This overload takes a the PortalId
        /// </remarks>
        ///	<param name="PortalID">The Portal Id</param>
        public static void SetApplicationName(int PortalID)
        {
            HttpContext.Current.Items["ApplicationName"] = GetApplicationName(PortalID);
        }

        /// <summary>
        /// Sets the ApplicationName for the MemberRole API
        /// </summary>
        /// <remarks>
        /// This overload takes a the PortalId
        /// </remarks>
        ///	<param name="ApplicationName">The Application Name to set</param>
        public static void SetApplicationName(string ApplicationName)
        {
            HttpContext.Current.Items["ApplicationName"] = ApplicationName;
        }

        //set focus to any control
        public static void SetFormFocus(Control control)
        {
            if (control.Page != null && control.Visible)
            {
                if (control.Page.Request.Browser.EcmaScriptVersion.Major >= 1)
                {
                    //JH dnn.js mod
                    if (ClientAPI.ClientAPIDisabled() == false)
                    {
                        ClientAPI.RegisterClientReference(control.Page, ClientAPI.ClientNamespaceReferences.dnn);
                        DNNClientAPI.AddBodyOnloadEventHandler(control.Page, "__dnn_SetInitialFocus('" + control.ClientID + "');");
                    }
                    else
                    {
                        // Create JavaScript
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<SCRIPT LANGUAGE='JavaScript'>");
                        sb.Append("<!--");
                        sb.Append(ControlChars.Lf);
                        sb.Append("function SetInitialFocus() {");
                        sb.Append(ControlChars.Lf);
                        sb.Append(" document.");

                        // Find the Form
                        Control objParent = control.Parent;
                        while (!(objParent is HtmlForm))
                        {
                            objParent = objParent.Parent;
                        }
                        sb.Append(objParent.ClientID);
                        sb.Append("['");
                        sb.Append(control.UniqueID);
                        sb.Append("'].focus(); }");
                        sb.Append("window.onload = SetInitialFocus;");
                        sb.Append(ControlChars.Lf);
                        sb.Append("// -->");
                        sb.Append(ControlChars.Lf);
                        sb.Append("</SCRIPT>");

                        // Register Client Script
                        ClientAPI.RegisterClientScriptBlock(control.Page, "InitialFocus", sb.ToString());
                    }
                }
            }
        }
    }
}