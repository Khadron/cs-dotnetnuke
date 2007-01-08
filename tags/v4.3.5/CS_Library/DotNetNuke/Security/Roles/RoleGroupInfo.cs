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