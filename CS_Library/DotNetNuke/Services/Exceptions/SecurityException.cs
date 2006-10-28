using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;
using System.Xml.Serialization;

namespace DotNetNuke.Services.Exceptions
{
    [Serializable()]
    public class SecurityException : BasePortalException
    {
        private string m_IP;
        private string m_Querystring;

        // default constructor
        public SecurityException()
        {
        }

        //constructor with exception message
        public SecurityException(string message)
            : base(message)
        {
            InitilizePrivateVariables();
        }

        // constructor with message and inner exception
        public SecurityException(string message, Exception inner)
            : base(message, inner)
        {
            InitilizePrivateVariables();
        }

        protected SecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitilizePrivateVariables();
            m_IP = info.GetString("m_IP");
            m_Querystring = info.GetString("m_Querystring");
        }

        private void InitilizePrivateVariables()
        {
            //Try and get the Portal settings from httpcontext
            try
            {
                if (HttpContext.Current.Request.UserHostAddress != null)
                {
                    m_IP = HttpContext.Current.Request.UserHostAddress;
                }
                m_Querystring = HttpContext.Current.Request.MapPath(Querystring, HttpContext.Current.Request.ApplicationPath, false);
            }
            catch (Exception)
            {
                m_IP = "";
                m_Querystring = "";
            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize this class' state and then call the base class GetObjectData
            //info.AddValue("m_IP", m_IP, GetType(String))
            //info.AddValue("m_Querystring", m_Querystring, GetType(String))
            info.AddValue("m_IP", m_IP, typeof(string));
            info.AddValue("m_Querystring", m_Querystring, typeof(string));
            base.GetObjectData(info, context);
        }

        [XmlElement("IP")]
        public string IP
        {
            get
            {
                return m_IP;
            }
        }

        [XmlElement("Querystring")]
        public string Querystring
        {
            get
            {
                return m_Querystring;
            }
        }
    }
}