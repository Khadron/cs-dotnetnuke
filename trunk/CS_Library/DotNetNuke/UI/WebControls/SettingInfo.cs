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

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The SettingInfo class provides a helper class for the Settings Editor
    /// </Summary>
    public class SettingInfo
    {
        private string _Editor;
        private string _Name;
        private Type _Type;
        private object _Value;

        /// <Summary>Gets and sets the Editor to use for the Setting</Summary>
        public string Editor
        {
            get
            {
                return this._Editor;
            }
            set
            {
                this._Editor = value;
            }
        }

        /// <Summary>Gets and sets the Setting Name</Summary>
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        /// <Summary>Gets and sets the Setting Type</Summary>
        public Type Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        /// <Summary>Gets and sets the Setting Value</Summary>
        public object Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }

        /// <Summary>Constructs a new SettingInfo obect</Summary>
        /// <Param name="name">The name of the setting</Param>
        /// <Param name="value">The value of the setting</Param>
        public SettingInfo( object name, object value )
        {
            _Name = Convert.ToString(name);
            _Value = value;
            _Type = value.GetType();
            _Editor = EditorInfo.GetEditor(-1);

            string strValue = Convert.ToString(value);
            bool IsFound = false;

            if (_Type.IsEnum)
            {
                IsFound = true;
            }

            if (!IsFound)
            {
                try
                {
                    bool boolValue = bool.Parse(strValue);
                    Editor = EditorInfo.GetEditor("TrueFalse");
                    IsFound = true;
                }
                catch (Exception)
                {
                }
            }
            if (!IsFound)
            {
                try
                {
                    int intValue = int.Parse(strValue);
                    Editor = EditorInfo.GetEditor("Integer");
                    IsFound = true;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}