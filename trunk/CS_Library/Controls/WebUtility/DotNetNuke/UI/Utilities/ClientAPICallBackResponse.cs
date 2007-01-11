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
using System.Web.UI;

namespace DotNetNuke.UI.Utilities
{
    public class ClientAPICallBackResponse
    {
        public CallBackTypeCode CallBackType;
        private Page m_objPage;
        public string Response;
        public CallBackResponseStatusCode StatusCode;
        public string StatusDesc;

        public enum CallBackResponseStatusCode
        {
            OK = 200,
            GenericFailure = 400,
            ControlNotFound = 404,
            InterfaceNotSupported = 501,
        }

        public enum CallBackTypeCode
        {
            Simple = 0,
        }

        public enum TransportTypeCode
        {
            XMLHTTP = 0,
            IFRAMEPost = 1,
        }

        public ClientAPICallBackResponse( Page objPage, CallBackTypeCode eCallBackType )
        {
            this.Response = "";
            this.StatusDesc = "";
            this.m_objPage = objPage;
            this.CallBackType = eCallBackType;
        }

        public TransportTypeCode TransportType
        {
            get
            {
                if( this.m_objPage.Request.Form["ctx"].Length <= 0 )
                {
                    return TransportTypeCode.XMLHTTP;
                }
                else
                {
                    return TransportTypeCode.IFRAMEPost;
                }
            }
        }

        public void Write()
        {

            switch( this.TransportType )
            {
                case TransportTypeCode.IFRAMEPost:
                    {
                        string s = this.m_objPage.Request.Form["ctx"];
                        this.m_objPage.Response.Write( ( "<html><head></head><body onload=\"window.parent.dnn.xmlhttp.requests['" + s + "'].complete(window.parent.dnn.dom.getById('txt', document).value);\"><form>" ) );
                        this.m_objPage.Response.Write( ( "<input type=\"hidden\" id=\"__DNNCAPISCSI\" value=\"" + (int)this.StatusCode + "\">" ) );
                        this.m_objPage.Response.Write( ( "<input type=\"hidden\" id=\"__DNNCAPISCSDI\" value=\"" + this.StatusDesc + "\">" ) );
                        this.m_objPage.Response.Write( "<textarea id=\"txt\">" );
                        this.m_objPage.Response.Write( this.Response );
                        this.m_objPage.Response.Write( "</textarea></body></html>" );
                        return;
                    }
                case TransportTypeCode.XMLHTTP:
                    {
                        int statusCode = ( (int)this.StatusCode );
                        this.m_objPage.Response.AppendHeader( "__DNNCAPISCSI", statusCode.ToString() );
                        this.m_objPage.Response.AppendHeader( "__DNNCAPISCSDI", this.StatusDesc );
                        this.m_objPage.Response.Write( this.Response );
                        return;
                    }
            }
        }
    }
}