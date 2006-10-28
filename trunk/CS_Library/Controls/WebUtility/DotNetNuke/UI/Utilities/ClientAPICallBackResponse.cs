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
            string string1;
            int i1;
            switch( this.TransportType )
            {
                case TransportTypeCode.IFRAMEPost:
                    {
                        string1 = this.m_objPage.Request.Form["ctx"];
                        this.m_objPage.Response.Write( ( "<html><head></head><body onload=\"window.parent.dnn.xmlhttp.requests[\'" + string1 + "\'].complete(window.parent.dnn.dom.getById(\'txt\', document).value);\"><form>" ) );
                        this.m_objPage.Response.Write( ( "<input type=\"hidden\" id=\"__DNNCAPISCSI\" value=\"" + ( i1 = ( (int)this.StatusCode ) ).ToString() + "\">" ) );
                        this.m_objPage.Response.Write( ( "<input type=\"hidden\" id=\"__DNNCAPISCSDI\" value=\"" + this.StatusDesc + "\">" ) );
                        this.m_objPage.Response.Write( "<textarea id=\"txt\">" );
                        this.m_objPage.Response.Write( this.Response );
                        this.m_objPage.Response.Write( "</textarea></body></html>" );
                        return;
                    }
                case TransportTypeCode.XMLHTTP:
                    {
                        i1 = ( (int)this.StatusCode );
                        this.m_objPage.Response.AppendHeader( "__DNNCAPISCSI", i1.ToString() );
                        this.m_objPage.Response.AppendHeader( "__DNNCAPISCSDI", this.StatusDesc );
                        this.m_objPage.Response.Write( this.Response );
                        return;
                    }
            }
        }
    }
}