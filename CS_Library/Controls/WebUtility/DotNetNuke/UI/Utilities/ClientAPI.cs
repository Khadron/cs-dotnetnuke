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
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.Utilities
{
    /// Project	 : DotNetNuke
    /// Class	 : ClientAPI
    /// <summary>
    /// Library responsible for interacting with DNN Client API.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Jon Henning]	8/3/2004	Created
    /// </history>
    public class ClientAPI
    {
        #region Public Constants

        public const string SCRIPT_CALLBACKID = "__DNNCAPISCI";
        public const string SCRIPT_CALLBACKTYPE = "__DNNCAPISCT";
        public const string SCRIPT_CALLBACKPARAM = "__DNNCAPISCP";
        public const string SCRIPT_CALLBACKPAGEID = "__DNNCAPISCPAGEID";
        public const string SCRIPT_CALLBACKSTATUSID = "__DNNCAPISCSI";
        public const string SCRIPT_CALLBACKSTATUSDESCID = "__DNNCAPISCSDI";

        public const string DNNVARIABLE_CONTROLID = "__dnnVariable";

        #endregion

        #region Public Enums

        public enum ClientFunctionality : int
        {
            DHTML = 1,
            XML = 2,
            XSLT = 4,
            Positioning = 8,
            XMLJS = 16,
            XMLHTTP = 32,
            XMLHTTPJS = 64,
            SingleCharDelimiters = 128,
        }

        /// <summary>
        /// Enumerates each namespace with a seperate js file
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        public enum ClientNamespaceReferences : int
        {
            dnn,
            dnn_dom,
            dnn_dom_positioning,
            dnn_xml,
            dnn_xmlhttp
        }

        #endregion

        #region Private Shared Members

        /// <summary>Private variable holding location of client side js files.  Shared by entire application.</summary>
        private static string m_sScriptPath;

        private static string m_ClientAPIDisabled = string.Empty;

        #endregion

        #region Private Shared Properties

        /// <summary>
        /// Finds __dnnVariable control on page, if not found it attempts to add its own.
        /// </summary>
        /// <param name="objPage">Current page rendering content</param>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        //ORIGINAL LINE: Private Shared ReadOnly Property ClientVariableControl(ByVal objPage As Page) As HtmlInputHidden
        //INSTANT C# NOTE: C# does not support parameterized properties - the following property has been rewritten as a function:
        private static HtmlInputHidden GetClientVariableControl(Page objPage)
        {
            return RegisterDNNVariableControl(objPage);
        }

        /// <summary>
        /// Loop up parent controls to find form
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/2/2006	Commented
        /// </history>
        private static Control FindForm(Control oCtl)
        {
            while (!(oCtl is HtmlForm))
            {
                if (oCtl == null || oCtl is Page)
                {
                    return null;
                }
                oCtl = oCtl.Parent;
            }
            return oCtl;
        }

        /// <summary>
        /// Returns __dnnVariable control if present
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        private static HtmlInputHidden GetDNNVariableControl(Control objParent)
        {
            return (HtmlInputHidden)(Globals.FindControlRecursive(objParent.Page, DNNVARIABLE_CONTROLID));
        }

        #endregion

        #region Public Shared Properties

        /// <summary>Character used for delimiting name from value</summary>
        public static string COLUMN_DELIMITER
        {
            get
            {
                if (BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return ((char)18).ToString();
                }
                else
                {
                    return "~|~";
                }
            }
        }

        /// <summary>Character used for delimiting name from value</summary>
        public static string CUSTOM_COLUMN_DELIMITER
        {
            get
            {
                if (BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return ((char)16).ToString();
                }
                else
                {
                    return "~.~";
                }
            }
        }

        /// <summary>Character used for delimiting name/value pairs</summary>
        public static string CUSTOM_ROW_DELIMITER
        {
            get
            {
                if (BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return ((char)15).ToString();
                }
                else
                {
                    return "~,~";
                }
            }
        }

        /// <summary>In order to reduce payload, substitute out " with different char, since when put in a hidden control it uses &quot;</summary>
        public static string QUOTE_REPLACEMENT
        {
            get
            {
                if (BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return ((char)19).ToString();
                }
                else
                {
                    return "~!~";
                }
            }
        }

        /// <summary>Character used for delimiting name/value pairs</summary>
        public static string ROW_DELIMITER
        {
            get
            {
                if (BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return ((char)17).ToString();
                }
                else
                {
                    return "~`~";
                }
            }
        }

        /// <summary>
        /// Path where js files are placed
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/19/2004	Created
        /// </history>
        public static string ScriptPath
        {
            get
            {
                string script = "";
                // HACK : Modified to not error if object is null.
                //if (m_sScriptPath.Length > 0)
                if (!String.IsNullOrEmpty(m_sScriptPath))
                {
                    script = m_sScriptPath;
                }
                else if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request.ApplicationPath.EndsWith("/"))
                    {
                        script = HttpContext.Current.Request.ApplicationPath + "js/";
                    }
                    else
                    {
                        script = HttpContext.Current.Request.ApplicationPath + "/js/";
                    }
                }
                return script;
            }
            set
            {
                m_sScriptPath = value;
            }
        }

        #endregion

        #region Private Shared Methods

        private static void AddAttribute(Control objControl, string strName, string strValue)
        {
            if (objControl is HtmlControl)
            {
                ((HtmlControl)objControl).Attributes.Add(strName, strValue);
            }
            else if (objControl is WebControl)
            {
                ((WebControl)objControl).Attributes.Add(strName, strValue);
            }
        }

        /// <summary>
        /// Parses DNN Variable control contents and returns out the delimited name/value pair
        /// </summary>
        /// <param name="objPage">Current page rendering content</param>
        /// <param name="strVar">Name to retrieve</param>
        /// <returns>Delimited name/value pair string</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        private static string GetClientVariableNameValuePair(Page objPage, string strVar)
        {
            HtmlInputHidden ctlVar = GetClientVariableControl(objPage);
            string strValue = "";
            if (ctlVar != null)
            {
                strValue = ctlVar.Value;
            }
            if (strValue.Length == 0) //using request object in case we are loading before controls have values set
            {
                strValue = HttpContext.Current.Request[DNNVARIABLE_CONTROLID];
            }            
            // HACK : Modified to not error if object is null.
            //if (strValue.Length > 0)
            if (!String.IsNullOrEmpty(strValue))
            {
                strValue = strValue.Replace(QUOTE_REPLACEMENT, "\"");
                int intIndex = strValue.IndexOf(ROW_DELIMITER + strVar + COLUMN_DELIMITER);
                if (intIndex > -1)
                {
                    intIndex += COLUMN_DELIMITER.Length;
                    int intEndIndex = strValue.IndexOf(ROW_DELIMITER, intIndex);
                    if (intEndIndex > -1)
                    {
                        return strValue.Substring(intIndex, intEndIndex - intIndex);
                    }
                    else
                    {
                        return strValue.Substring(intIndex);
                    }
                }
            }           
            return "";
        }

        /// <summary>
        /// Returns javascript to call dnncore.js key handler logic
        /// </summary>
        /// <param name="intKeyAscii">ASCII value to trap</param>
        /// <param name="strJavascript">Javascript to execute</param>
        /// <returns>Javascript to handle key press</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/17/2005	Created
        /// </history>
        private static string GetKeyDownHandler(int intKeyAscii, string strJavascript)
        {
            return "return __dnn_KeyDown('" + intKeyAscii + "', '" + strJavascript.Replace("'", "%27") + "', event);";
        }

        #endregion

        #region Public Shared Methods

        /// <summary>
        /// Common way to handle confirmation prompts on client
        /// </summary>
        /// <param name="objButton">Button to trap click event</param>
        /// <param name="strText">Text to display in confirmation</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/17/2005	Created
        /// </history>
        public static void AddButtonConfirm(WebControl objButton, string strText)
        {
            objButton.Attributes.Add("onClick", "javascript:return confirm('" + GetSafeJSString(strText) + "');");
        }

        /// <summary>
        /// Determines of browser currently requesting page adaquately supports passed un client-side functionality
        /// </summary>
        /// <param name="eFunctionality">Desired Functionality</param>
        /// <returns>True when browser supports it</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        public static bool BrowserSupportsFunctionality(ClientFunctionality eFunctionality)
        {
            if (HttpContext.Current == null)
            {
                return true;
            }
            bool blnSupports = false;

            if (ClientAPIDisabled() == false)
            {
                BrowserCaps objCaps = BrowserCaps.GetBrowserCaps();
                if (objCaps != null)
                {
                    HttpRequest objRequest = HttpContext.Current.Request;
                    string strUserAgent = objRequest.UserAgent;
                    // HACK : Modified to not error if object is null.
                    //if (strUserAgent.Length > 0)
                    if (!String.IsNullOrEmpty(strUserAgent))
                    {
                        //First check whether we have checked this browser before
                        if (objCaps.FunctionalityDictionary.ContainsKey(strUserAgent) == false)
                        {
                            string strBrowser = objRequest.Browser.Browser;
                            double dblVersion = Convert.ToDouble(objRequest.Browser.MajorVersion + objRequest.Browser.MinorVersion);
                            int iBitValue = 0;
                            FunctionalityInfo objFuncInfo = null;
                            //loop through all functionalities for this UserAgent and determine the bitvalue 
                            foreach (ClientFunctionality eFunc in Enum.GetValues(typeof(ClientFunctionality)))
                            {
                                objFuncInfo = objCaps.Functionality[eFunc];
                                if (objFuncInfo.HasMatch(strUserAgent, strBrowser, dblVersion))
                                {
                                    iBitValue += (int)eFunc;
                                }
                            }
                            objCaps.FunctionalityDictionary[strUserAgent] = iBitValue;
                        }
                        blnSupports = (((int)(objCaps.FunctionalityDictionary[strUserAgent])) & (int)eFunctionality) != 0;
                    }
                }
            }

            return blnSupports;
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, null, "");
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, ClientAPICallBackResponse.CallBackTypeCode eCallbackType)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, null, null, eCallbackType);
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, Control objPostChildrenOf)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, null, objPostChildrenOf.ClientID, ClientAPICallBackResponse.CallBackTypeCode.Simple);
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, null, ClientAPICallBackResponse.CallBackTypeCode.Simple);
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack, ClientAPICallBackResponse.CallBackTypeCode eCallbackType)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, null, eCallbackType);
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack, Control objPostChildrenOf)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, objPostChildrenOf.ClientID, ClientAPICallBackResponse.CallBackTypeCode.Simple);
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack, string strPostChildrenOfId)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, strPostChildrenOfId, ClientAPICallBackResponse.CallBackTypeCode.Simple);
        }

        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack, string strPostChildrenOfId, ClientAPICallBackResponse.CallBackTypeCode eCallbackType)
        {
            string strCallbackType = Convert.ToInt32(eCallbackType).ToString();
            if (strArgument == null)
            {
                strArgument = "null";
            }
            if (strContext == null)
            {
                strContext = "null";
            }
            if (srtClientErrorCallBack == null)
            {
                srtClientErrorCallBack = "null";
            }
            if (strClientStatusCallBack == null)
            {
                strClientStatusCallBack = "null";
            }
            // HACK : Modified to not error if object is null.
            //if (strPostChildrenOfId.Length == 0)
            if (String.IsNullOrEmpty(strPostChildrenOfId))
            {
                strPostChildrenOfId = "null";
            }
            else if (strPostChildrenOfId.StartsWith("'") == false)
            {
                strPostChildrenOfId = "'" + strPostChildrenOfId + "'";
            }
            string strControlID = objControl.ID;
            if (BrowserSupportsFunctionality(ClientFunctionality.XMLHTTP) && BrowserSupportsFunctionality(ClientFunctionality.XML))
            {
                RegisterClientReference(objControl.Page, ClientNamespaceReferences.dnn_xml);
                RegisterClientReference(objControl.Page, ClientNamespaceReferences.dnn_xmlhttp);

                if ((objControl) is Page && strControlID.Length == 0) //page doesn't usually have an ID so we need to make one up
                {
                    strControlID = SCRIPT_CALLBACKPAGEID;
                }

                if ((objControl) is Page == false)
                {
                    strControlID = strControlID + " " + objControl.ClientID; //ID is not unique (obviously)
                }

                return string.Format("dnn.xmlhttp.doCallBack('{0}',{1},{2},{3},{4},{5},{6},{7},{8});", strControlID, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, "null", strPostChildrenOfId, strCallbackType);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Retrieves DNN Client Variable value
        /// </summary>
        /// <param name="objPage">Current page rendering content</param>
        /// <param name="strVar">Variable name to retrieve value for</param>
        /// <returns>Value of variable</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        public static string GetClientVariable(Page objPage, string strVar)
        {
            string s = GetClientVariableNameValuePair(objPage, strVar);
            if (s.IndexOf(COLUMN_DELIMITER) <= -1)
            {
                return "";
            }
            else
            {
                string[] splitter = { COLUMN_DELIMITER };
                return s.Split(splitter, StringSplitOptions.None)[1];
            }
        }

        /// <summary>
        /// Retrieves DNN Client Variable value
        /// </summary>
        /// <param name="objPage">Current page rendering content</param>
        /// <param name="strVar">Variable name to retrieve value for</param>
        /// <param name="strDefaultValue">Default value if variable not found</param>
        /// <returns>Value of variable</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        public static string GetClientVariable(Page objPage, string strVar, string strDefaultValue)
        {
            string s = GetClientVariableNameValuePair(objPage, strVar);
            if (s.IndexOf(COLUMN_DELIMITER) <= -1)
            {
                return strDefaultValue;
            }
            else
            {
                string[] splitter = { COLUMN_DELIMITER };
                return s.Split(splitter, StringSplitOptions.None)[1];
            }
        }

        /// <summary>
        /// Escapes string to be safely used in client side javascript.  
        /// </summary>
        /// <param name="strString">String to escape</param>
        /// <returns>Escaped string</returns>
        /// <remarks>
        /// Currently this only escapes out quotes and apostrophes
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/17/2005	Created
        /// </history>
        public static string GetSafeJSString(string strString)
        {
            // HACK : Modified to not error if object is null.
            //if (strString.Length > 0)
            if (!string.IsNullOrEmpty(strString))
            {
                //Return System.Text.RegularExpressions.Regex.Replace(strString, "(['""])", "\$1")
                return Regex.Replace(strString, "(['\"\\\\])", "\\$1");
            }
            else
            {
                return strString;
            }
        }

        public static bool IsInCallback(Page objPage)
        {
            // HACK : Modified to handle null value of the 
            // objPage.Request[SCRIPT_CALLBACKID] object.  
            // Original VB code did not require specific test 
            // for null value.
            //return objPage.Request[SCRIPT_CALLBACKID].Length > 0;
            return !String.IsNullOrEmpty(objPage.Request[SCRIPT_CALLBACKID]);
        }

        public static void HandleClientAPICallbackEvent(Page objPage)
        {
            ClientAPICallBackResponse.CallBackTypeCode eType = ClientAPICallBackResponse.CallBackTypeCode.Simple;
            // HACK : Modified to handle null value of the 
            // objPage.Request[SCRIPT_CALLBACKID] object.  
            // Original VB code did not require specific test 
            // for null value.
            //if(objPage.Request[SCRIPT_CALLBACKTYPE].Length > 0 )
            if (!String.IsNullOrEmpty(objPage.Request[SCRIPT_CALLBACKTYPE]))
            {
                eType = (ClientAPICallBackResponse.CallBackTypeCode)Enum.Parse(typeof(ClientAPICallBackResponse.CallBackTypeCode), objPage.Request[SCRIPT_CALLBACKTYPE]);
            }
            HandleClientAPICallbackEvent(objPage, eType);
        }

        public static void HandleClientAPICallbackEvent(Page objPage, ClientAPICallBackResponse.CallBackTypeCode eType)
        {
            if (IsInCallback(objPage))
            {
                if (eType != ClientAPICallBackResponse.CallBackTypeCode.ProcessPage)
                {
                    string[] arrIDs = objPage.Request[SCRIPT_CALLBACKID].Split(Convert.ToChar(" "));
                    string strControlID = arrIDs[0];
                    string strClientID = "";
                    if (arrIDs.Length > 1)
                    {
                        strClientID = arrIDs[1];
                    }

                    string strParam = objPage.Server.UrlDecode(objPage.Request[SCRIPT_CALLBACKPARAM]);
                    Control objControl = null;
                    IClientAPICallbackEventHandler objInterface = null;
                    ClientAPICallBackResponse objResponse = new ClientAPICallBackResponse(objPage, ClientAPICallBackResponse.CallBackTypeCode.Simple);

                    try
                    {
                        objPage.Response.Clear(); //clear response stream
                        if (strControlID == SCRIPT_CALLBACKPAGEID)
                        {
                            objControl = objPage;
                        }
                        else
                        {
                            objControl = Globals.FindControlRecursive(objPage, strControlID, strClientID);
                        }
                        if (objControl != null)
                        {
                            if ((objControl) is HtmlForm) //form doesn't implement interface, so use page instead
                            {
                                objInterface = (IClientAPICallbackEventHandler)objPage;
                            }
                            else
                            {
                                objInterface = (IClientAPICallbackEventHandler)objControl;
                            }

                            if (objInterface != null)
                            {
                                try
                                {
                                    objResponse.Response = objInterface.RaiseClientAPICallbackEvent(strParam);
                                    objResponse.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.OK;
                                }
                                catch (Exception ex)
                                {
                                    objResponse.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.GenericFailure;
                                    objResponse.StatusDesc = ex.Message;
                                }
                            }
                            else
                            {
                                objResponse.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.InterfaceNotSupported;
                                objResponse.StatusDesc = "Interface Not Supported";
                            }
                        }
                        else
                        {
                            objResponse.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.ControlNotFound;
                            objResponse.StatusDesc = "Control Not Found";
                        }
                    }
                    catch (Exception ex)
                    {
                        objResponse.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.GenericFailure;
                        objResponse.StatusDesc = ex.Message;
                    }
                    finally
                    {
                        objResponse.Write();
                        //objPage.Response.Flush()
                        objPage.Response.End();
                    }
                }
                else
                {
                    objPage.SetRenderMethodDelegate(new RenderMethod(CallbackRenderMethod));
                }
            }
        }

        private static void CallbackRenderMethod(HtmlTextWriter output, Control container)
        {
            Page objPage = (Page)container;
            HandleClientAPICallbackEvent(objPage, ClientAPICallBackResponse.CallBackTypeCode.Simple);
        }

        /// <summary>
        /// Determines if DNNVariable control is present in page's control collection
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        public static bool NeedsDNNVariable(Control objParent)
        {
            return GetDNNVariableControl(objParent) == null;
        }

        /// <summary>
        /// Responsible for registering client side js libraries and its dependecies.
        /// </summary>
        /// <param name="objPage">Current page rendering content</param>
        /// <param name="eRef">Enumerator of library to reference</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        public static void RegisterClientReference(Page objPage, ClientNamespaceReferences eRef)
        {
            switch (eRef)
            {
                case ClientNamespaceReferences.dnn:
                    if (!(IsClientScriptBlockRegistered(objPage, "dnn.js")))
                    {
                        RegisterClientScriptBlock(objPage, "dnn.js", "<script src=\"" + ScriptPath + "dnn.js\"></script>");
                        if (BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters) == false)
                        {
                            RegisterClientVariable(objPage, "__scdoff", "1", true); //SingleCharDelimiters Off!!!
                        }
                    }
                    break;
                case ClientNamespaceReferences.dnn_dom:
                    RegisterClientReference(objPage, ClientNamespaceReferences.dnn);
                    break;
                case ClientNamespaceReferences.dnn_dom_positioning:
                    RegisterClientReference(objPage, ClientNamespaceReferences.dnn);
                    if (!(IsClientScriptBlockRegistered(objPage, "dnn.positioning.js")))
                    {
                        RegisterClientScriptBlock(objPage, "dnn.positioning.js", "<script src=\"" + ScriptPath + "dnn.dom.positioning.js\"></script>");
                    }
                    break;
                case ClientNamespaceReferences.dnn_xml:
                    RegisterClientReference(objPage, ClientNamespaceReferences.dnn);

                    if (!(IsClientScriptBlockRegistered(objPage, "dnn.xml.js")))
                    {
                        string strScript = "<script src=\"" + ScriptPath + "dnn.xml.js\"></script>";
                        //only register the js parser if browsers needs it
                        if (BrowserSupportsFunctionality(ClientFunctionality.XMLJS)) //TODO: detect when using uplevel parser and only send this when necessary
                        {
                            strScript += "<script src=\"" + ScriptPath + "dnn.xml.jsparser.js\"></script>";
                        }
                        RegisterClientScriptBlock(objPage, "dnn.xml.js", strScript);
                    }
                    break;
                case ClientNamespaceReferences.dnn_xmlhttp:
                    RegisterClientReference(objPage, ClientNamespaceReferences.dnn);
                    if (!(IsClientScriptBlockRegistered(objPage, "dnn.xmlhttp.js")))
                    {
                        string strScript = "<script src=\"" + ScriptPath + "dnn.xmlhttp.js\"></script>";
                        if (BrowserSupportsFunctionality(ClientFunctionality.XMLHTTPJS))
                        {
                            strScript += "<script src=\"" + ScriptPath + "dnn.xmlhttp.jsxmlhttprequest.js\"></script>";
                        }
                        RegisterClientScriptBlock(objPage, "dnn.xmlhttp.js", strScript);
                    }
                    break;
            }
        }

        /// <summary>
        /// Registers a client side variable (name/value) pair
        /// </summary>
        /// <param name="objPage">Current page rendering content</param>
        /// <param name="strVar">Variable name</param>
        /// <param name="strValue">Value</param>
        /// <param name="blnOverwrite">Determins if a replace or append is applied when variable already exists</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	8/3/2004	Created
        /// </history>
        public static void RegisterClientVariable(Page objPage, string strVar, string strValue, bool blnOverwrite)
        {
            //only add once
            HtmlInputHidden ctlVar = GetClientVariableControl(objPage);
            string strPair = GetClientVariableNameValuePair(objPage, strVar);
            // HACK : Modified to not error if object is null.
            //if (strPair.Length > 0)
            if (!String.IsNullOrEmpty(strPair))
            {
                strPair = strPair.Replace("\"", QUOTE_REPLACEMENT); //since we are searching for existing string we need it in its posted format (without quotes)
                if (blnOverwrite)
                {
                    ctlVar.Value = ctlVar.Value.Replace(ROW_DELIMITER + strPair, ROW_DELIMITER + strVar + COLUMN_DELIMITER + strValue);
                }
                else
                {
                    //appending value
                    string strOrig = GetClientVariable(objPage, strVar);
                    ctlVar.Value = ctlVar.Value.Replace(ROW_DELIMITER + strPair, ROW_DELIMITER + strVar + COLUMN_DELIMITER + strOrig + strValue);
                }
            }
            else
            {
                ctlVar.Value += ROW_DELIMITER + strVar + COLUMN_DELIMITER + strValue;
            }
            ctlVar.Value = ctlVar.Value.Replace("\"", QUOTE_REPLACEMENT); //reduce payload of &quot;
        }

        /// <summary>
        /// Responsible for inputting the hidden field necessary for the ClientAPI to pass variables back in forth
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        public static HtmlInputHidden RegisterDNNVariableControl(Control objParent)
        {
            HtmlInputHidden ctlVar = GetDNNVariableControl(objParent);
            if (ctlVar == null)
            {
                Control oForm = FindForm(objParent);
                if (oForm != null)
                {
                    ctlVar = new HtmlInputHidden();
                    ctlVar.ID = DNNVARIABLE_CONTROLID;
                    //oForm.Controls.AddAt(0, ctlVar)
                    oForm.Controls.Add(ctlVar);
                }
            }
            return ctlVar;
        }

        /// <summary>
        /// Traps client side keydown event looking for passed in key press (ASCII) and hooks it up with server side postback handler
        /// </summary>
        /// <param name="objControl">Control that should trap the keydown</param>
        /// <param name="objPostbackControl">Server-side control that has its onclick event handled server-side</param>
        /// <param name="intKeyAscii">ASCII value of key to trap</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/17/2005	Commented
        /// </history>
        public static void RegisterKeyCapture(Control objControl, Control objPostbackControl, int intKeyAscii)
        {
            Globals.SetAttribute(objControl, "onkeydown", GetKeyDownHandler(intKeyAscii, GetPostBackClientHyperlink(objPostbackControl, "")));
        }

        /// <summary>
        /// Traps client side keydown event looking for passed in key press (ASCII) and hooks it up with client-side javascript
        /// </summary>
        /// <param name="objControl">Control that should trap the keydown</param>
        /// <param name="strJavascript">Javascript to execute when event fires</param>
        /// <param name="intKeyAscii">ASCII value of key to trap</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/17/2005	Commented
        /// </history>
        public static void RegisterKeyCapture(Control objControl, string strJavascript, int intKeyAscii)
        {
            Globals.SetAttribute(objControl, "onkeydown", GetKeyDownHandler(intKeyAscii, strJavascript));
        }

        /// <summary>
        /// Allows a listener to be associated to a client side post back
        /// </summary>
        /// <param name="objParent">The current control on the page or the page itself.  Depending on where the page is in its lifecycle it may not be possible to add a control directly to the page object, therefore we will use the current control being rendered to append the postback control.</param>
        /// <param name="strEventName">Name of the event to sync.  If a page contains more than a single client side event only the events associated with the passed in name will be raised.</param>
        /// <param name="objDelegate">Server side AddressOf the function to handle the event</param>
        /// <param name="blnMultipleHandlers">Boolean flag to determine if multiple event handlers can be associated to an event.</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	9/15/2004	Created
        /// </history>
        public static void RegisterPostBackEventHandler(Control objParent, string strEventName, ClientAPIPostBackControl.PostBackEvent objDelegate, bool blnMultipleHandlers)
        {
            const string CLIENTAPI_POSTBACKCTL_ID = "ClientAPIPostBackCtl";
            Control objCtl = Globals.FindControlRecursive(objParent.Page, CLIENTAPI_POSTBACKCTL_ID); //DotNetNuke.Globals.FindControlRecursive(objParent, CLIENTAPI_POSTBACKCTL_ID)
            if (objCtl == null)
            {
                objCtl = new ClientAPIPostBackControl(objParent.Page, strEventName, objDelegate);
                objCtl.ID = CLIENTAPI_POSTBACKCTL_ID;
                objParent.Controls.Add(objCtl);
                RegisterClientVariable(objParent.Page, "__dnn_postBack", GetPostBackClientHyperlink(objCtl, "[DATA]"), true);
            }
            else if (blnMultipleHandlers)
            {
                ((ClientAPIPostBackControl)objCtl).AddEventHandler(strEventName, objDelegate);
            }
        }

        /// <summary>
        /// Registers a button inside a table for the ability to perform client-side reordering
        /// </summary>
        /// <param name="objButton">Button responsible for moving the row up or down.</param>
        /// <param name="objPage">Page the table belongs to.  Can't just use objButton.Page because inside ItemCreated event of grid the button has no page yet.</param>
        /// <param name="blnUp">Determines if the button is responsible for moving the row up or down</param>
        /// <param name="strKey">Unique key for the table/grid to be used to obtain the new order on postback.  Needed when calling GetClientSideReOrder</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	3/10/2006	Created
        /// </history>
        public static void EnableClientSideReorder(Control objButton, Page objPage, bool blnUp, string strKey)
        {
            if (BrowserSupportsFunctionality(ClientFunctionality.DHTML))
            {
                RegisterClientReference(objPage, ClientNamespaceReferences.dnn_dom);
                if (!(IsClientScriptBlockRegistered(objPage, "dnn.util.tablereorder.js")))
                {
                    RegisterClientScriptBlock(objPage, "dnn.util.tablereorder.js", "<script src=\"" + ScriptPath + "dnn.util.tablereorder.js\"></script>");
                }

                AddAttribute(objButton, "onclick", "if (dnn.util.tableReorderMove(this," + Convert.ToInt32(blnUp) + ",'" + strKey + "')) return false;");
                Control objParent = objButton.Parent;
                while (objParent != null)
                {
                    if (objParent is TableRow)
                    {
                        AddAttribute(objParent, "origidx", "-1"); //mark row as one that we care about, it will be numbered correctly on client
                    }
                    objParent = objParent.Parent;
                }
            }
        }

        /// <summary>
        /// Retrieves an array of the new order for the rows
        /// </summary>
        /// <param name="strKey">Unique key for the table/grid to be used to obtain the new order on postback.  Needed when calling GetClientSideReOrder</param>
        /// <param name="objPage">Page the table belongs to.  Can't just use objButton.Page because inside ItemCreated event of grid the button has no page yet.</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	3/10/2006	Created
        /// </history>
        public static string[] GetClientSideReorder(string strKey, Page objPage)
        {
            // HACK : Modified to not error if object is null.
            //if (GetClientVariable(objPage, strKey).Length > 0)
            if (!String.IsNullOrEmpty(GetClientVariable(objPage, strKey)))
            {
                return GetClientVariable(objPage, strKey).Split(',');
            }
            else
            {
                return new string[] { };
            }
        }

        public static bool ClientAPIDisabled()
        {
            if (m_ClientAPIDisabled == string.Empty)
            {
                if (ConfigurationManager.AppSettings["ClientAPI"] == null)
                {
                    m_ClientAPIDisabled = "1";
                }
                else
                {
                    m_ClientAPIDisabled = ConfigurationManager.AppSettings["ClientAPI"];
                }
            }
            return m_ClientAPIDisabled == "0";
        }

        public static string GetPostBackClientEvent(Page objPage, Control objControl, string arg)
        {
            return objPage.ClientScript.GetPostBackEventReference(objControl, arg);
        }

        public static string GetPostBackClientHyperlink(Control objControl, string strArgument)
        {
            return "javascript:" + GetPostBackEventReference(objControl, strArgument);
        }

        public static string GetPostBackEventReference(Control objControl, string strArgument)
        {
            return objControl.Page.ClientScript.GetPostBackEventReference(objControl, strArgument);
        }

        public static bool IsClientScriptBlockRegistered(Page objPage, string key)
        {
            return objPage.ClientScript.IsClientScriptBlockRegistered(objPage.GetType(), key);
        }

        public static void RegisterClientScriptBlock(Page objPage, string key, string strScript)
        {
            objPage.ClientScript.RegisterClientScriptBlock(objPage.GetType(), key, strScript);
        }

        public static void RegisterStartUpScript(Page objPage, string key, string script)
        {
            objPage.ClientScript.RegisterStartupScript(objPage.GetType(), key, script);
        }

        #endregion
    }
}