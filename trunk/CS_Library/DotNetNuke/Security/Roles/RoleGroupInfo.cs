namespace DotNetNuke.Security.Roles
{
    /// <summary>
    /// The RoleGroupInfo class provides the Entity Layer RoleGroup object
    /// </summary>
    public class RoleGroupInfo
    {
        private string _Description;
        private int _PortalID;
        private int _RoleGroupID;
        private string _RoleGroupName;

        /// <summary>
        /// Gets an sets the Description of the RoleGroup
        /// </summary>
        /// <value>A string representing the description of the RoleGroup</value>
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
        /// Gets and sets the Portal Id for the RoleGroup
        /// </summary>
        /// <value>An Integer representing the Id of the Portal</value>
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
        /// Gets and sets the RoleGroup Name
        /// </summary>
        /// <value>A string representing the Name of the RoleGroup</value>
        public string RoleGroupName
        {
            get
            {
                return _RoleGroupName;
            }
            set
            {
                _RoleGroupName = value;
            }
        }
    }
}