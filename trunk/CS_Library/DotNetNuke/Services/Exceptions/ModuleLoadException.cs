using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Services.Exceptions
{
    [Serializable()]
    public class ModuleLoadException : BasePortalException
    {
        private string m_FriendlyName;
        private DotNetNuke.Entities.Modules.ModuleInfo m_ModuleConfiguration;
        private string m_ModuleControlSource;
        private int m_ModuleDefId;
        private int m_ModuleId;

        [XmlElement("FriendlyName")]
        public string FriendlyName
        {
            get
            {
                return m_FriendlyName;
            }
        }

        [XmlElement("ModuleControlSource")]
        public string ModuleControlSource
        {
            get
            {
                return m_ModuleControlSource;
            }
        }

        [XmlElement("ModuleDefId")]
        public int ModuleDefId
        {
            get
            {
                return m_ModuleDefId;
            }
        }

        [XmlElement("ModuleID")]
        public int ModuleId
        {
            get
            {
                return m_ModuleId;
            }
        }

        // default constructor
        public ModuleLoadException()
        {
        }

        //constructor with exception message
        public ModuleLoadException(string message)
            : base(message)
        {
            InitilizePrivateVariables();
        }

        //constructor with exception message
        public ModuleLoadException(string message, Exception inner, ModuleInfo ModuleConfiguration)
            : base(message, inner)
        {
            m_ModuleConfiguration = ModuleConfiguration;
            InitilizePrivateVariables();
        }

        // constructor with message and inner exception
        public ModuleLoadException(string message, Exception inner)
            : base(message, inner)
        {
            InitilizePrivateVariables();
        }

        protected ModuleLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitilizePrivateVariables();
            m_ModuleId = info.GetInt32("m_ModuleId");
            m_ModuleDefId = info.GetInt32("m_ModuleDefId");
            m_FriendlyName = info.GetString("m_FriendlyName");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize this class' state and then call the base class GetObjectData
            info.AddValue("m_ModuleId", m_ModuleId, typeof(int));
            info.AddValue("m_ModuleDefId", m_ModuleDefId, typeof(int));
            info.AddValue("m_FriendlyName", m_FriendlyName, typeof(string));
            info.AddValue("m_ModuleControlSource", m_ModuleControlSource, typeof(string));

            base.GetObjectData(info, context);
        }

        private void InitilizePrivateVariables()
        {
            //Try and get the Portal settings from context
            //If an error occurs getting the context then set the variables to -1
            try
            {
                m_ModuleId = m_ModuleConfiguration.ModuleID;
                m_ModuleDefId = m_ModuleConfiguration.ModuleDefID;
                m_FriendlyName = m_ModuleConfiguration.ModuleTitle;
                m_ModuleControlSource = m_ModuleConfiguration.ControlSrc;
            }
            catch (Exception)
            {
                m_ModuleId = -1;
                m_ModuleDefId = -1;
                m_FriendlyName = "";
            }
        }
    }
}