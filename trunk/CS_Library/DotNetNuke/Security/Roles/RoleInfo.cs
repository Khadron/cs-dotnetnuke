using System.Xml.Serialization;

namespace DotNetNuke.Security.Roles
{
    /// <summary>
    /// The RoleInfo class provides the Entity Layer Role object
    /// </summary>
    [XmlRoot("role", IsNullable = false)]
    public class RoleInfo
    {
        private int _RoleID;
        private int _PortalID;
        private int _RoleGroupID;
        private string _RoleName;
        private string _Description;
        private float _ServiceFee;
        private string _BillingFrequency;
        private int _TrialPeriod;
        private string _TrialFrequency;
        private int _BillingPeriod;
        private float _TrialFee;
        private bool _IsPublic;
        private bool _AutoAssignment;
        private string _RSVPCode;
        private string _IconFile;

        /// <summary>
        /// Gets and sets the Role Id
        /// </summary>
        /// <value>An Integer representing the Id of the Role</value>
        [XmlIgnore()]
        public int RoleID
        {
            get
            {
                return _RoleID;
            }
            set
            {
                _RoleID = value;
            }
        }

        /// <summary>
        /// Gets and sets the Portal Id for the Role
        /// </summary>
        /// <value>An Integer representing the Id of the Portal</value>
        [XmlIgnore()]
        public int PortalID
        {
            get
            {
                return _PortalID;
            }
            set
            {
                _PortalID = value;
            }
        }

        /// <summary>
        /// Gets and sets the RoleGroup Id
        /// </summary>
        /// <value>An Integer representing the Id of the RoleGroup</value>
        [XmlIgnore()]
        public int RoleGroupID
        {
            get
            {
                return _RoleGroupID;
            }
            set
            {
                _RoleGroupID = value;
            }
        }

        /// <summary>
        /// Gets and sets the Role Name
        /// </summary>
        /// <value>A string representing the name of the role</value>
        [XmlElement("rolename")]
        public string RoleName
        {
            get
            {
                return _RoleName;
            }
            set
            {
                _RoleName = value;
            }
        }

        /// <summary>
        /// Gets an sets the Description of the Role
        /// </summary>
        /// <value>A string representing the description of the role</value>
        [XmlElement("description")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        /// <summary>
        /// Gets and sets the Billing Frequency for the role
        /// </summary>
        /// <value>A String representing the Billing Frequency of the Role<br/>
        /// <ul>
        /// <list>N - None</list>
        /// <list>O - One time fee</list>
        /// <list>D - Daily</list>
        /// <list>W - Weekly</list>
        /// <list>M - Monthly</list>
        /// <list>Y - Yearly</list>
        /// </ul>
        /// </value>
        [XmlElement("billingfrequency")]
        public string BillingFrequency
        {
            get
            {
                return _BillingFrequency;
            }
            set
            {
                _BillingFrequency = value;
            }
        }

        /// <summary>
        /// Gets and sets the fee for the role
        /// </summary>
        /// <value>A single number representing the fee for the role</value>
        [XmlElement("servicefee")]
        public float ServiceFee
        {
            get
            {
                return _ServiceFee;
            }
            set
            {
                _ServiceFee = value;
            }
        }

        /// <summary>
        /// Gets and sets the Trial Frequency for the role
        /// </summary>
        /// <value>A String representing the Trial Frequency of the Role<br/>
        /// <ul>
        /// <list>N - None</list>
        /// <list>O - One time fee</list>
        /// <list>D - Daily</list>
        /// <list>W - Weekly</list>
        /// <list>M - Monthly</list>
        /// <list>Y - Yearly</list>
        /// </ul>
        /// </value>
        [XmlElement("trialfrequency")]
        public string TrialFrequency
        {
            get
            {
                return _TrialFrequency;
            }
            set
            {
                _TrialFrequency = value;
            }
        }

        /// <summary>
        /// Gets and sets the length of the trial period
        /// </summary>
        /// <value>An integer representing the length of the trial period</value>
        [XmlElement("trialperiod")]
        public int TrialPeriod
        {
            get
            {
                return _TrialPeriod;
            }
            set
            {
                _TrialPeriod = value;
            }
        }

        /// <summary>
        /// Gets and sets the length of the billing period
        /// </summary>
        /// <value>An integer representing the length of the billing period</value>
        [XmlElement("billingperiod")]
        public int BillingPeriod
        {
            get
            {
                return _BillingPeriod;
            }
            set
            {
                _BillingPeriod = value;
            }
        }

        /// <summary>
        /// Gets and sets the trial fee for the role
        /// </summary>
        /// <value>A single number representing the trial fee for the role</value>
        [XmlElement("trialfee")]
        public float TrialFee
        {
            get
            {
                return _TrialFee;
            }
            set
            {
                _TrialFee = value;
            }
        }

        /// <summary>
        /// Gets and sets whether the role is public
        /// </summary>
        /// <value>A boolean (True/False)</value>
        [XmlElement("ispublic")]
        public bool IsPublic
        {
            get
            {
                return _IsPublic;
            }
            set
            {
                _IsPublic = value;
            }
        }

        /// <summary>
        /// Gets and sets whether users are automatically assigned to the role
        /// </summary>
        /// <value>A boolean (True/False)</value>
        [XmlElement("autoassignment")]
        public bool AutoAssignment
        {
            get
            {
                return _AutoAssignment;
            }
            set
            {
                _AutoAssignment = value;
            }
        }

        /// <summary>
        /// Gets and sets the RSVP Code for the role
        /// </summary>
        /// <value>A string representing the RSVP Code for the role</value>
        [XmlElement("rsvpcode")]
        public string RSVPCode
        {
            get
            {
                return _RSVPCode;
            }
            set
            {
                _RSVPCode = value;
            }
        }

        /// <summary>
        /// Gets and sets the Icon File for the role
        /// </summary>
        /// <value>A string representing the Icon File for the role</value>
        [XmlElement("iconfile")]
        public string IconFile
        {
            get
            {
                return _IconFile;
            }
            set
            {
                _IconFile = value;
            }
        }
    }
}