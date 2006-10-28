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